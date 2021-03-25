using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    private Vector3 relPos;
    // Start is called before the first frame update
    void Start()
    {
        relPos = transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = relPos + target.transform.position;
    }
}