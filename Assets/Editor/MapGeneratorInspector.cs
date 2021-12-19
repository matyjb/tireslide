using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator generator = (MapGenerator)target;
        if(GUILayout.Button("Generate map (random seed)"))
        {
            generator.GenerateMapDFS(Random.Range(0,int.MaxValue), out var _);
        }
        if (GUILayout.Button("Destroy map"))
        {
            generator.DestroyMap();
        }
    }
}
