using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float steerSens = 1f;
    public float velocityDelta = 1f;
    public float steeringThreshhold = 1f;
    private Rigidbody rb;

    public GameObject rightFrontWheel;
    public GameObject leftFrontWheel;
    public GameObject rightRearWheel;
    public GameObject leftRearWheel;
    public GameObject body;

    public float steeringTime = 10f;

    private Vector3 steerRotationDelta = new Vector3(-4.5f, 30, 12);
    private Quaternion steerRestingRotationRight;
    private Quaternion steerRestingRotationLeft;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(),rightFrontWheel.GetComponent<WheelCollider>());
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(),leftFrontWheel.GetComponent<WheelCollider>());
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(),rightRearWheel.GetComponent<WheelCollider>());
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(),leftRearWheel.GetComponent<WheelCollider>());
        //steerRestingRotationRight = rightFrontWheel.transform.localRotation;
        //steerRestingRotationLeft = leftFrontWheel.transform.localRotation;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity += Input.GetAxis("Vertical") * velocityDelta * transform.forward;
        //if(rb.velocity.magnitude > steeringThreshhold)
        //{
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Input.GetAxis("Horizontal") * steerSens, 0));
        float input = Input.GetAxis("Horizontal");
        if (input != 0)
        {
            //steering
            rightFrontWheel.transform.localRotation = Quaternion.Lerp(rightFrontWheel.transform.localRotation, Quaternion.Euler(steerRestingRotationRight.eulerAngles + steerRotationDelta * input), steeringTime * Time.deltaTime);
            leftFrontWheel.transform.localRotation = Quaternion.Lerp(leftFrontWheel.transform.localRotation, Quaternion.Euler(steerRestingRotationLeft.eulerAngles + steerRotationDelta * input), steeringTime * Time.deltaTime);
        }
        else
        {
            //not steering
            rightFrontWheel.transform.localRotation = Quaternion.Lerp(rightFrontWheel.transform.localRotation, steerRestingRotationRight, steeringTime * Time.deltaTime);
            leftFrontWheel.transform.localRotation = Quaternion.Lerp(leftFrontWheel.transform.localRotation, steerRestingRotationLeft, steeringTime * Time.deltaTime);


        }
        //rightFrontWheel.transform.localRotation = Quaternion.Lerp(steerRestingRotationRight, steerRotationRight, Time.deltaTime * Input.GetAxis("Horizontal"));
        //}
    }
}
