using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVel : MonoBehaviour, IResetable
{
    Vector3 vel;
    Rigidbody rb;

    public void ResetToInitial()
    {
        rb.velocity = vel;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vel = rb.velocity;
    }
}
