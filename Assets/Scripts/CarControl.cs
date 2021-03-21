using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float steerSens = 1f;
    public float velocityDelta = 1f;
    public float steeringThreshhold = 1f;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity += Input.GetAxis("Vertical") * velocityDelta * (transform.rotation * Vector3.forward);
        //if(rb.velocity.magnitude > steeringThreshhold)
        //{
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Input.GetAxis("Horizontal") * steerSens, 0));
        //}
    }
}
