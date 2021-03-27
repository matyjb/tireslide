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
    public float steersens = 10f;

    private float steer = 0; // ranges -1 1
    private float gasbrake = 0; // ranges -1 1

    public float steerForceFactor = 1000;

    private Rigidbody rb;

    public WheelController rightFrontWheel;
    public WheelController leftFrontWheel;
    public WheelController rightRearWheel;
    public WheelController leftRearWheel;
    public GameObject body;

    float initDrag;




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
        initDrag = rb.drag;
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), rightFrontWheel.gameObject.GetComponent<Collider>());
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), leftFrontWheel.gameObject.GetComponent<Collider>());
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), rightRearWheel.gameObject.GetComponent<Collider>());
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), leftRearWheel.gameObject.GetComponent<Collider>());
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
        rpm = Mathf.Lerp(rpm, maxRPM * gasbrake, Time.deltaTime * power);
        steeringAngle = Mathf.Lerp(steeringAngle, maxSteeringAngle * steer, Time.deltaTime * steersens);
    }

    void ApplyForces()
    {
        rb.drag = initDrag;
        if (rightRearWheel.IsTouchingGround && leftRearWheel.IsTouchingGround)
        {
            rb.velocity += rpm * transform.forward;
        }
        else if (rightRearWheel.IsTouchingGround || leftRearWheel.IsTouchingGround)
        {
            rb.velocity += rpm / 2 * transform.forward;
        }
        else
        {
            rb.drag = 0.1f;
        }

        if (rightFrontWheel.IsTouchingGround || leftFrontWheel.IsTouchingGround)
        {
            rb.AddTorque(0, steeringAngle * steerForceFactor, 0);
        }
    }

    void UpdateVisuals()
    {
        rightFrontWheel.Turn(steer, false);
        leftFrontWheel.Turn(steer, true);

        UpdateSkid(rightFrontWheel);
        UpdateSkid(leftFrontWheel);
        UpdateSkid(rightRearWheel);
        UpdateSkid(leftRearWheel);

    }

    void UpdateSkid(WheelController wheel)
    {
        if (Mathf.Abs(Vector3.Dot(wheel.transform.forward.normalized, rb.velocity.normalized)) < 0.9)
        {
            wheel.StartSkid();
        }
        else
        {
            wheel.EndSkid();
        }

        if (!wheel.IsTouchingGround)
        {
            wheel.EndSkid();
        }


    }
}