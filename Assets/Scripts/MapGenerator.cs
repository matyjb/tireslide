using Assets.Scripts;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startBlockPrefab;
    public List<GameObject> elementsPrefabs;
    public GameObject finishBlockPrefab;

    public int generatorWidth = 200;
    public int generatorHeight = 200;
    public int generatorDepth = 200;

    private Bounds _bounds;

    private Stack<GameObject> t;

    public int maxMapLength = 50;
    public int minMapLength = 10;

    public int lastGeneratedMapLenght;
    public int lastGeneratedMapSeed;

    private System.Diagnostics.Stopwatch _generatorStopWatch = new System.Diagnostics.Stopwatch();
    public long maxGenerationTimeMs = 10000;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(generatorWidth, generatorHeight, generatorDepth));
    }

    public bool GenerateMapDFS(int seed, out int length)
    {
        _bounds = new Bounds(transform.position, new Vector3(generatorWidth, generatorHeight, generatorDepth));

        Random.InitState(seed);

        length = Random.Range(minMapLength, maxMapLength);
        lastGeneratedMapLenght = length;
        lastGeneratedMapSeed = seed;

        if (t != null)
            DestroyMap();
        t = new Stack<GameObject>();
        t.Push(Instantiate(startBlockPrefab, transform));
        t.Peek().transform.localPosition = Vector3.zero;
        t.Peek().transform.localRotation = Quaternion.identity;

        _generatorStopWatch.Restart();
        bool result = GenerateMapDFS(t, elementsPrefabs, length);
        _generatorStopWatch.Stop();


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

        if (!result)
        {
            Debug.LogError("Nie udalo sie wygenerowac trasy!");
            DestroyMap();
        }
        // Debug.Log(string.Format("Czas generowania trasy: {0}ms", _generatorStopWatch.ElapsedMilliseconds));
        return result;
    }

    private bool GenerateMapDFS(Stack<GameObject> t, List<GameObject> elementsPrefabs, int n)
    {
        if (!IsLastElementInBounds(t) || IsLastElementOverlapped(t) || _generatorStopWatch.ElapsedMilliseconds > maxGenerationTimeMs)
            return false;
        if (t.Count == n) return true;

        IEnumerable<Connector> unconnectedLastElementConnectors = t.Peek()
            .GetComponentsInChildren<Connector>()
            .Where(e => e.ConnectedTo == null);

        foreach (var l in unconnectedLastElementConnectors.Shuffle())
        {
            List<GameObject> w;
            if (t.Count == n - 1)
                w = new List<GameObject>() { finishBlockPrefab };
            else
                w = elementsPrefabs.Where(e => e.GetComponentsInChildren<Connector>().Any(c => c.type == l.type)).ToList();

            foreach (var item in w.Shuffle())
            {
                GameObject candidate = Instantiate(item, transform);
                t.Push(candidate);
                foreach (var ll in candidate.GetComponentsInChildren<Connector>().Where(c => c.type == l.type).Shuffle())
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

    private bool IsLastElementInBounds(Stack<GameObject> t)
    {
        BoxCollider blockBox = t.Peek().GetComponent<BoxCollider>();
        Vector3 blockBoxCenterWorldPosition = blockBox.transform.position + blockBox.center;
        if (_bounds != null)
            return _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(1, 1, 1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(1, 1, -1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(1, -1, 1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(1, -1, -1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(-1, 1, 1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(-1, 1, -1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(-1, -1, 1), blockBox.size)) &&
                   _bounds.Contains(blockBoxCenterWorldPosition + Vector3.Scale(new Vector3(-1, -1, -1), blockBox.size));
        else
        {
            Debug.LogError("Boundsy nullem!");

            return true;
        }
    }

    private bool IsLastElementOverlapped(Stack<GameObject> t)
    {
        BoxCollider box1 = t.Peek().GetComponent<BoxCollider>();
        foreach (var item in t)
        {
            if (item == t.Peek())
                continue;
            BoxCollider box2 = item.GetComponent<BoxCollider>();
            bool hasCollided = Physics.ComputePenetration(
                box1, box1.transform.position, box1.transform.rotation,
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

    public int CountCubesStacks()
    {
        int count = 0;
        foreach (Transform child0 in transform)
        {
            foreach (Transform child1 in child0)
                if (child1.name.Contains("CubesStack")) count++;
        }
        return count;
    }
    public int CountScoreGates()
    {
        int count = 0;
        foreach (Transform child0 in transform)
        {
            foreach (Transform child1 in child0)
                if (child1.name.Contains("ScoreGate")) count++;
        }
        return count;
    }
    public int CountComboGates()
    {
        int count = 0;
        foreach (Transform child0 in transform)
        {
            foreach (Transform child1 in child0)
                if (child1.name.Contains("ComboGate")) count++;
        }
        return count;
    }
}
