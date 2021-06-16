using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startBlockPrefab;
    public List<GameObject> blocksPrefabs;
    public GameObject finishBlockPrefab;

    private Stack<GameObject> mapObjects;

    public int length = 3;

    public void Generate(int seed)
    {
        if (mapObjects != null)
            DestroyMap();
        mapObjects = new Stack<GameObject>();
        Stack<Connector> connectors = new Stack<Connector>();
        Connector usedConnector = null;

        Random.InitState(seed);
        mapObjects.Push(Instantiate(startBlockPrefab, transform));
        mapObjects.Peek().transform.localPosition = Vector3.zero;
        mapObjects.Peek().transform.localRotation = Quaternion.identity;

        for (int i = 0; i <= length; i++)
        {
            List<Connector> tmp = mapObjects.Peek().GetComponentsInChildren<Connector>().ToList();
            tmp.Remove(usedConnector);
            ShuffleList(tmp);
            //Debug.Log("Liczba connectorow ostatniego klocka: " + tmp.Count.ToString());
            foreach (Connector connector in tmp)
            {
                connectors.Push(connector);
            }

            List<GameObject> nextPossibleBlocks;
            if (i == length)
                nextPossibleBlocks = new List<GameObject>() { finishBlockPrefab };
            else
                nextPossibleBlocks = blocksPrefabs.Where((GameObject g) => g.GetComponentsInChildren<Connector>().Where((Connector c) => c.type == connectors.Peek().type).Count() > 0).ToList();
            //Debug.Log("Liczba mo¿liwych nastêpnych klockow: " + nextPossibleBlocks.Count.ToString());
            bool isOverlapping = false;
            do
            {
                if (nextPossibleBlocks.Count == 0)
                {
                    Connector c = connectors.Pop();
                    if (c.gameObject != connectors.Peek().gameObject)
                    {
                        // to byl ostatni connector z ostatnie klocka
                        // ostatni klocek okazal sie komplemtnie nieprawid³owy
                        // trzeba go usunaæ
                        DestroyImmediate(mapObjects.Pop());
                    }
                    i--;
                    if (i < 0)
                    {
                        Debug.LogError("Could not generate map");
                        throw new System.Exception("Could not generate map");
                    }
                    break;
                }
                int selectedNextGameObjectIndex = Random.Range(0, nextPossibleBlocks.Count);
                GameObject selectedNextGameObject = Instantiate(nextPossibleBlocks[selectedNextGameObjectIndex], transform);
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


                selectedNextGameObject.transform.position = selectedNextGameObject.transform.position + lastBlockConnectorPos - nextBlockConnectorPos;
                RotateAround(selectedNextGameObject.transform, connectors.Peek().transform.position, selectedNextGameObject.transform.rotation * lastBlockConnectorRot * Quaternion.Inverse(nextBlockConnectorRot));

                // TODO: check if no overlap
                isOverlapping = false;
                foreach (var mo in mapObjects)
                {
                    BoxCollider box1 = mo.GetComponent<BoxCollider>(); //first collider
                    BoxCollider box2 = selectedNextGameObject.GetComponent<BoxCollider>(); //second collider
                    float distance; //how far they need to move apart
                    Vector3 direction; //which direction they need to move apart in
                    bool hasCollided = Physics.ComputePenetration(box1, box1.transform.position, box1.transform.rotation,
                                                            box2, box2.transform.position, box2.transform.rotation,
                                                            out direction, out distance);
                    //if (mo.GetComponent<BoxCollider>().bounds.Intersects(selectedNextGameObject.GetComponent<BoxCollider>().bounds))
                    if (hasCollided)
                    {
                        isOverlapping = true;
                        //Debug.Log("OVERLAP!");
                        //Debug.Log(mo.name);
                        //Debug.Log(selectedNextGameObject.name);

                        break;
                    }

                }
                //isOverlapping = false;
                if (isOverlapping)
                {
                    nextPossibleBlocks.RemoveAt(selectedNextGameObjectIndex);
                    DestroyImmediate(selectedNextGameObject);
                }
                else
                {
                    usedConnector = nextConnector;
                    mapObjects.Push(selectedNextGameObject);
                }
            } while (isOverlapping);
        }

        //gen meshes
        foreach (var item in mapObjects)
        {
            RoadMeshCreator rmc;
            if (item.TryGetComponent(out rmc))
                rmc.TriggerUpdate();
            GenerateStuff sg;
            if (item.TryGetComponent(out sg))
                sg.Generate();
        }

        // dirty fix for road meshcolliders
        StartCoroutine(SetAllCollidersConvex());
    }

    private IEnumerator SetAllCollidersConvex()
    {
        foreach (var item in mapObjects)
        {
            RoadMeshCreator rmc;
            if (item.TryGetComponent(out rmc))
                rmc.meshHolder.GetComponent<MeshCollider>().convex = true;
        }
        yield return new WaitForSeconds(0f);
    }

    public void DestroyMap()
    {
        if (mapObjects != null)
        {
            foreach (var item in mapObjects)
            {
                if (item != null)
                {
                    if (item.GetComponent<RoadMeshCreator>() != null)
                        DestroyImmediate(item.GetComponent<RoadMeshCreator>().meshHolder);
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
