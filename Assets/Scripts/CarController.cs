using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float rpm = 0;
    public float maxRPM = 1.5f;

    public float steeringAngle = 0;
    public float maxSteeringAngle = 30;

    public float power = 0.5f; // rpm gained per input (should be curve)
    public float steersens = 1f;

    private float steer = 0; // ranges -1 1
    private float gasbrake = 0; // ranges -1 1

    private Rigidbody rb;

    public GameObject rightFrontWheel;
    public GameObject leftFrontWheel;
    public GameObject rightRearWheel;
    public GameObject leftRearWheel;
    public GameObject body;

    public Skid skid;

    private Vector3 steerRotationDelta = new Vector3(-4.5f, 30, 12);
    private Quaternion steerRestingRotationRight;
    private Quaternion steerRestingRotationLeft;

    /*
     * samochod ma kule (sphere) w srodku dotykajaca podloza ktora jest mniejsza od samochodu
     * gdy koła samochodu:
     * dotykaja ziemi - mozna sterowac samochodem, samochod kopiuje pozycje z kuli i oblicza kąt nachylenia do podloza (by skopiowac)
     * nie dotykają - nie mozna sterowac samochodem, samochodem steruje fizyka
     * ----
     * center of mass musi byc nisko (by samochod automatycznie sie stabilizowal)
     * ----
     * sterowanie:
     * gasbrake modyfikuje rpm = Lerp(rpm,maxRPM*gasbrake,)
     * steer modyfikuje steeringAngle = Lerp(steeringAngle,maxSteeringAngle*steer,)
     * ----
     * rpm modyfikuje velocity kuli w kierunku w ktorym jest obrócony samochod (body.forward)
     * steeringAngle modyfikuje rotation samochodu
     * ----
     * 
     */

    void Start()
    {
        rb = GetComponent<Rigidbody>();    
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), rightFrontWheel.GetComponent<Collider>());
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), leftFrontWheel.GetComponent<Collider>());
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), rightRearWheel.GetComponent<Collider>());
        Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), leftRearWheel.GetComponent<Collider>());
    }

    void FixedUpdate()
    {
        GetInput();
        UpdateCarEngineAndSteering();
        ApplyForces();
        UpdateVisuals();
    }

    void GetInput()
    {
        steer = Input.GetAxis("Horizontal");
        gasbrake = Input.GetAxis("Vertical");
    }

    void UpdateCarEngineAndSteering()
    {
        rpm = Mathf.Lerp(rpm, maxRPM * gasbrake,Time.deltaTime * power);
        steeringAngle = Mathf.Lerp(steeringAngle, maxSteeringAngle * steer, Time.deltaTime * steersens);
    }

    void ApplyForces()
    {
        rb.velocity += rpm * transform.forward;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, steeringAngle, 0));
    }

    void UpdateVisuals()
    {
        float steeringTime = 10f;
        if (steer != 0)
        {
            //steering
            rightFrontWheel.transform.localRotation = Quaternion.Lerp(rightFrontWheel.transform.localRotation, Quaternion.Euler(steerRestingRotationRight.eulerAngles + steerRotationDelta * steer), steeringTime * Time.deltaTime);
            leftFrontWheel.transform.localRotation = Quaternion.Lerp(leftFrontWheel.transform.localRotation, Quaternion.Euler(steerRestingRotationLeft.eulerAngles + steerRotationDelta * steer), steeringTime * Time.deltaTime);
        }
        else
        {
            //not steering
            rightFrontWheel.transform.localRotation = Quaternion.Lerp(rightFrontWheel.transform.localRotation, steerRestingRotationRight, steeringTime * Time.deltaTime);
            leftFrontWheel.transform.localRotation = Quaternion.Lerp(leftFrontWheel.transform.localRotation, steerRestingRotationLeft, steeringTime * Time.deltaTime);
        }

        Debug.Log(Vector3.Dot(transform.forward.normalized, rb.velocity.normalized));
        if(Mathf.Abs(Vector3.Dot(transform.forward.normalized, rb.velocity.normalized)) < 0.9)
        {
            skid.StartSkid();
        }
        else
        {
            skid.EndSkid();
        }
    }
}