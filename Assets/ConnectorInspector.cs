using PathCreation;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Connector))]
public class ConnectorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameObject gameObject = ((Connector)target).gameObject;

        BezierPath path = gameObject.GetComponentInParent<PathCreator>().bezierPath;

        if (GUILayout.Button("Set position to start"))
        {
            Vector3 startPos = path.GetPoint(0);
            gameObject.transform.localPosition = startPos;
        }

        if (GUILayout.Button("Set position to end"))
        {
            Vector3 endPos = path.GetPoint(path.NumPoints-1);
            gameObject.transform.localPosition = endPos;
        }
    }
}
