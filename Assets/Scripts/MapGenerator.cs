using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startBlockPrefab;
    public List<GameObject> elementsPrefabs;
    public GameObject finishBlockPrefab;

    private Stack<GameObject> t;

    public int n = 3;

    public bool GenerateMapDFS(int seed)
    {
        Random.InitState(seed);
        if (t != null)
            DestroyMap();
        t = new Stack<GameObject>();
        t.Push(Instantiate(startBlockPrefab, transform));
        t.Peek().transform.localPosition = Vector3.zero;
        t.Peek().transform.localRotation = Quaternion.identity;

        bool result = GenerateMapDFS(t, elementsPrefabs, n);

        foreach (var item in t)
        {
            RoadMeshCreator rmc;
            if (item.TryGetComponent(out rmc))
                rmc.TriggerUpdate();
            GenerateStuff sg;
            if (item.TryGetComponent(out sg))
                sg.Generate();
        }

        // fix for road mesh colliders
        StartCoroutine(SetAllCollidersConvex());

        return result;
    }

    private bool GenerateMapDFS(Stack<GameObject> t, List<GameObject> elementsPrefabs, int n)
    {
        if (IsLastElementOverlapped(t)) return false;
        if (t.Count == n) return true;

        List<Connector> unconnectedLastElementConnectors = t.Peek().GetComponentsInChildren<Connector>().Where(e => e.ConnectedTo == null).ToList();
        ShuffleList(unconnectedLastElementConnectors);
        foreach (var l in unconnectedLastElementConnectors)
        {
            List<GameObject> w;
            if (t.Count == n - 1)
                w = new List<GameObject>() { finishBlockPrefab };
            else
                w = elementsPrefabs.Where(e => e.GetComponentsInChildren<Connector>().Any(c => c.type == l.type)).ToList();

            ShuffleList(w);
            foreach (var item in w)
            {
                GameObject candidate = Instantiate(item, transform);
                t.Push(candidate);
                foreach (var ll in ShuffleList(candidate.GetComponentsInChildren<Connector>().Where(c=>c.type == l.type).ToList()))
                {
                    l.ConnectTo(ll);
                    ll.AlignParentToConnected();
                    if (GenerateMapDFS(t, elementsPrefabs, n)) 
                        return true;
                    l.Unconnect();
                }
                t.Pop();
                DestroyImmediate(candidate);
            }
        }
        return false;

    }

    private bool IsLastElementOverlapped(Stack<GameObject> t)
    {
        BoxCollider box1 = t.Peek().GetComponent<BoxCollider>();
        foreach (var item in t)
        {
            if (item == t.Peek())
                continue;
            BoxCollider box2 = item.GetComponent<BoxCollider>(); //second collider
            bool hasCollided = Physics.ComputePenetration(box1, box1.transform.position, box1.transform.rotation,
                                                    box2, box2.transform.position, box2.transform.rotation,
                                                    out _, out _);
            if (hasCollided) 
                return true;
        }
        return false;
    }

    private IEnumerator SetAllCollidersConvex()
    {
        foreach (var item in t)
        {
            RoadMeshCreator rmc;
            if (item.TryGetComponent(out rmc))
                rmc.meshHolder.GetComponent<MeshCollider>().convex = true;
        }
        yield return new WaitForSeconds(0f);
    }

    public void DestroyMap()
    {
        if (t != null)
        {
            foreach (var item in t)
            {
                if (item != null)
                {
                    if (item.GetComponent<RoadMeshCreator>() != null)
                        DestroyImmediate(item.GetComponent<RoadMeshCreator>().meshHolder);
                    DestroyImmediate(item);
                }
            }
            t.Clear();
        }
    }

    private static List<T> ShuffleList<T>(List<T> list)
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
        return list;
    }
}
