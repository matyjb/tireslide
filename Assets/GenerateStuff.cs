using PathCreation;
using PathCreation.Examples;
using System.Collections.Generic;
using UnityEngine;

public class GenerateStuff : MonoBehaviour
{
    VertexPath path;
    RoadMeshCreator rmc;

    public GameObject[] prefabsToSpawnAlongPathPickups;
    public GameObject[] prefabsToSpawnAlongPathGates;

    private void Start()
    {
        path = GetComponent<PathCreator>().path;
        rmc = GetComponent<RoadMeshCreator>();
    }
    public void Generate()
    {
        if (path == null)
            path = GetComponent<PathCreator>().path;
        if (rmc == null)
            rmc = GetComponent<RoadMeshCreator>();

        List<float> alreadySpawnedAt = new List<float>();
        // generate along path
        if (prefabsToSpawnAlongPathPickups.Length > 0)
            for (int i = 0; i < Random.Range(0, 20); i++)
            {
                float distance = Random.Range(0f, 1f);
                Vector3 pos = path.GetPointAtTime(distance);
                Quaternion rot = path.GetRotation(distance) * Quaternion.Euler(90,0,90);
                Vector3 normal = path.GetNormal(distance);
                float widthAtDistance = rmc.roadWidthStart + (rmc.roadWidthStart - rmc.roadWidthEnd) * distance;
                pos += normal * Random.Range(-widthAtDistance, widthAtDistance);

                Instantiate(prefabsToSpawnAlongPathPickups[Random.Range(0, prefabsToSpawnAlongPathPickups.Length)], pos, rot, transform);
            }

        if (prefabsToSpawnAlongPathGates.Length > 0)
            for (int i = 0; i < Random.Range(0, 3); i++)
            {
                float distance = Random.Range(0f, 1f);
                Vector3 pos = path.GetPointAtTime(distance);
                Quaternion rot = path.GetRotation(distance) * Quaternion.Euler(90, 0, 90);
                Vector3 normal = path.GetNormal(distance);
                float widthAtDistance = rmc.roadWidthStart + (rmc.roadWidthStart - rmc.roadWidthEnd) * distance;
                pos += normal * Random.Range(-widthAtDistance, widthAtDistance);

                Instantiate(prefabsToSpawnAlongPathGates[Random.Range(0, prefabsToSpawnAlongPathGates.Length)], pos, rot, transform);
            }
    }
}
