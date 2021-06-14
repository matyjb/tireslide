using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosRot : MonoBehaviour, IResetable
{
    Quaternion rot;
    Vector3 pos;

    public void ResetToInitial()
    {
        transform.rotation = rot;
        transform.position = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        rot = transform.rotation;
        pos = transform.position;
    }
}
