using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorInspector : Editor
{
    int benchmarkMapsToGenerate = 100;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator generator = (MapGenerator)target;
        if (GUILayout.Button("Generate map (random seed)"))
        {
            generator.GenerateMapDFS(Random.Range(0, int.MaxValue), out var _);
        }
        if (GUILayout.Button("Destroy map"))
        {
            generator.DestroyMap();
        }
        benchmarkMapsToGenerate = EditorGUILayout.IntField("Maps to generate in benchmark: ", benchmarkMapsToGenerate);
        if (GUILayout.Button("Benchmark"))
        {
            Stopwatch stopWatch = new Stopwatch();
            StreamWriter sw = new StreamWriter("benchmarkData.csv");
            sw.WriteLine("seed;length;seconds");
            for (int i = 0; i < benchmarkMapsToGenerate; i++)
            {
                int length, seed = Random.Range(0, int.MaxValue);
                stopWatch.Restart();
                if (generator.GenerateMapDFS(seed, out length))
                {
                    double secondsToGenerate = stopWatch.Elapsed.TotalSeconds;
                    generator.DestroyMap();
                    sw.WriteLine(string.Join(";", new dynamic[] { seed, length, secondsToGenerate }));
                }
                else
                {
                    i--; // try again
                }
            }
            UnityEngine.Debug.Log(string.Format("Run benchmark of {0} maps", benchmarkMapsToGenerate));
            sw.Close();
        }
    }
}
