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
    public AnimationCurve turningCurve;

    private Rigidbody rb;

    public WheelController rightFrontWheel;
    public WheelController leftFrontWheel;
    public WheelController rightRearWheel;
    public WheelController leftRearWheel;
    public GameObject body;

    float initDrag;
    float initAngularDrag;

    public float toGroundForceMultiplier = 2000;

    public Vector3 centerOfMass;

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
        initAngularDrag = rb.angularDrag;
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), rightFrontWheel.gameObject.GetComponent<Collider>());
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), leftFrontWheel.gameObject.GetComponent<Collider>());
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), rightRearWheel.gameObject.GetComponent<Collider>());
        //Physics.IgnoreCollision(body.GetComponent<MeshCollider>(), leftRearWheel.gameObject.GetComponent<Collider>());
    }

    public void GasBrake_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        gasbrake = obj.ReadValue<float>();
    }
    public void Steer_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        steer = obj.ReadValue<float>();
    }

    void FixedUpdate()
    {
        UpdateCarEngineAndSteering();
        ApplyForces();
        UpdateVisuals();
        rb.centerOfMass = centerOfMass;
    }


    void UpdateCarEngineAndSteering()
    {
        rpm = Mathf.Lerp(rpm, maxRPM * gasbrake, Time.deltaTime * power);
        steeringAngle = Mathf.Lerp(steeringAngle, (maxSteeringAngle * steer)* turningCurve.Evaluate(rpm/maxRPM), Time.deltaTime * steersens);
    }

    void ApplyForces()
    {
        rb.drag = initDrag;
        rb.angularDrag = initAngularDrag;
        int wheelsTouching = 0;
        if (rightRearWheel.IsTouchingGround) wheelsTouching++;
        if (leftRearWheel.IsTouchingGround) wheelsTouching++;
        switch (wheelsTouching)
        {
            case 1:
                rb.velocity += rpm / 2 * transform.forward;
                break;
            case 2:
                rb.velocity += rpm * transform.forward;
                break;
        }
        if (rightFrontWheel.IsTouchingGround) wheelsTouching++;
        if (leftFrontWheel.IsTouchingGround) wheelsTouching++;
        if(wheelsTouching == 0)
        {
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
        }
        if(wheelsTouching != 4)
        {
            rb.AddForce(transform.rotation * Vector3.down * toGroundForceMultiplier);
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