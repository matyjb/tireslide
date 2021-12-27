using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

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
            int n;
            generator.GenerateMapDFS(Random.Range(0, int.MaxValue), out n);
            Debug.Log("Length: " + n);
            Debug.Log("Combo gates: " + generator.CountComboGates());
            Debug.Log("Score gates: " + generator.CountScoreGates());
            Debug.Log("Cubes stacks: " + generator.CountCubesStacks());
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
            sw.WriteLine("seed;length;seconds;cubes_stacks;score_gates;combo_gates");
            for (int i = 0; i < benchmarkMapsToGenerate; i++)
            {
                int length, seed = Random.Range(0, int.MaxValue);
                stopWatch.Restart();
                if (generator.GenerateMapDFS(seed, out length))
                {
                    double secondsToGenerate = stopWatch.Elapsed.TotalSeconds;
                    int countCubesStacks = generator.CountCubesStacks();
                    int countScoreGates = generator.CountScoreGates();
                    int countComboGates = generator.CountComboGates();
                    generator.DestroyMap();
                    sw.WriteLine(string.Join(";", new dynamic[] { seed, length, secondsToGenerate, countCubesStacks, countScoreGates, countComboGates }));
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
