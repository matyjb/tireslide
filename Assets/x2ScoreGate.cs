using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class x2ScoreGate : MonoBehaviour, IResetable
{
    private new BoxCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        collider.enabled = false;
        // add multiplyier in PointsManager
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetToInitial()
    {
        collider.enabled = true;
    }
}
