using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startBlockPrefab;
    public List<GameObject> blocksPrefabs;

    private Stack<GameObject> mapObjects;

    public int length = 3;

    public void Generate(int seed)
    {
        if (mapObjects != null)
            DestroyMap();
        mapObjects = new Stack<GameObject>();
        Random.InitState(seed);
        mapObjects.Push(Instantiate(startBlockPrefab, transform));
        mapObjects.Peek().transform.localPosition = Vector3.zero;
        mapObjects.Peek().transform.localRotation = Quaternion.identity;

        Connector usedConnector = null;

        for (int i = 0; i < length; i++)
        {
            Stack<Connector> connectors = new Stack<Connector>();
            List<Connector> tmp = new List<Connector>(mapObjects.Peek().GetComponentsInChildren<Connector>());
            tmp.Remove(usedConnector);
            ShuffleList(tmp);
            //Debug.Log("Liczba connectorow ostatniego klocka: " + tmp.Count.ToString());
            foreach (Connector connector in tmp)
            {
                connectors.Push(connector);
            }

            List<GameObject> nextPossibleBlocks = blocksPrefabs.Where((GameObject g) => g.GetComponentsInChildren<Connector>().Where((Connector c) => c.type == connectors.Peek().type).Count() > 0).ToList();
            //Debug.Log("Liczba mo¿liwych nastêpnych klockow: " + nextPossibleBlocks.Count.ToString());

            GameObject selectedNextGameObject = Instantiate(nextPossibleBlocks[Random.Range(0, nextPossibleBlocks.Count)],transform);
            //Debug.Log("Wybrany: " + selectedNextGameObject.name);

            Vector3 lastBlockConnectorPos = connectors.Peek().transform.position;
            Quaternion lastBlockConnectorRot = connectors.Peek().transform.rotation;

            List<Connector> nextConnectors = selectedNextGameObject.GetComponentsInChildren<Connector>().ToList();
            //Debug.Log("Liczba connectorow nastepnego klocka: " + nextConnectors.Count.ToString());
            List<Connector> tmpWhereConnector = nextConnectors.Where((Connector c) => c.type == connectors.Peek().type).ToList();
            //Debug.Log("Liczba connectorow kompatybilnych z poprzednim connectorem: " + tmpWhereConnector.Count.ToString());
            Connector nextConnector = tmpWhereConnector[Random.Range(0, tmpWhereConnector.Count)];

            Vector3 nextBlockConnectorPos = nextConnector.transform.position;
            Quaternion nextBlockConnectorRot = nextConnector.transform.rotation * Quaternion.Euler(0, 180f, 0);

            usedConnector = nextConnector;

            // TODO: check if no clipping

            mapObjects.Push(selectedNextGameObject);
            selectedNextGameObject.transform.position = selectedNextGameObject.transform.position + lastBlockConnectorPos - nextBlockConnectorPos;
            RotateAround(selectedNextGameObject.transform, connectors.Peek().transform.position, selectedNextGameObject.transform.rotation * lastBlockConnectorRot * Quaternion.Inverse(nextBlockConnectorRot));
        }

        //gen meshes
        foreach (var item in mapObjects)
        {
            item.GetComponent<RoadMeshCreator>().TriggerUpdate();
        }
    }

    public void DestroyMap()
    {
        if (mapObjects != null)
        {
            foreach (var item in mapObjects)
            {
                if (item != null)
                {
                    DestroyImmediate(item.GetComponent<RoadMeshCreator>().meshHolder.gameObject);
                    DestroyImmediate(item);
                }
            }
            mapObjects.Clear();
        }
    }

    private static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    static void RotateAround(Transform transform, Vector3 pivotPoint, Quaternion rot)
    {
        transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
        transform.rotation = rot * transform.rotation;
    }
}
