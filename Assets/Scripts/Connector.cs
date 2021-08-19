using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorType
{
    RoadWide,
    RoadNarrow,
    Platform
}

public class Connector : MonoBehaviour
{
    public ConnectorType type;
    [SerializeField]
    private Connector _connectedTo;
    public Connector ConnectedTo { get=>_connectedTo; private set=>_connectedTo=value; }

    public void ConnectTo(Connector connector)
    {
        if(ConnectedTo != connector)
        {
            if(connector.type == type)
            {
                ConnectedTo = connector;
                connector.ConnectTo(this);
            }
        }
    }

    public void Unconnect()
    {
        if(ConnectedTo != null)
        {
            Connector c = ConnectedTo;
            ConnectedTo = null;
            if(c.ConnectedTo == this)
            {
                c.Unconnect();
            }
        }
    }

    public void AlignParentToConnected()
    {
        if(ConnectedTo != null)
        {
            Quaternion rotDiff = ConnectedTo.transform.rotation * Quaternion.Inverse(transform.rotation);
            transform.parent.rotation *= rotDiff * Quaternion.Euler(0, 180, 0);

            Vector3 posDiff = transform.position - ConnectedTo.transform.position;
            transform.parent.position -= posDiff;
        }
    }

    static void RotateAround(Transform transform, Vector3 pivotPoint, Quaternion rot)
    {
        transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
        transform.rotation = rot * transform.rotation;
    }
}
