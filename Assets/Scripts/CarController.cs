using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float rpm = 0;
    public float maxRPM = 1.5f;
    public ProgressBar rpmPlusProgressBar;
    public ProgressBar rpmMinusProgressBar;

    public float steeringAngle = 0;
    public float maxSteeringAngle = 30;
    public ProgressBar steeringAngleProgressBar;

    public float power = 0.5f; // rpm gained per input (should be curve)
    public float steersens = 10f;

    private float steer = 0; // ranges -1 1
    public ProgressBar steerProgressBar;
    private float gasbrake = 0; // ranges -1 1
    public ProgressBar gasbrakePlusProgressBar;
    public ProgressBar gasbrakeMinusProgressBar;

    private bool handbrake = false;
    public ProgressBar handbrakeProgressBar;

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

        gasbrakePlusProgressBar.minValue = 0;
        gasbrakePlusProgressBar.maxValue = 1;

        gasbrakeMinusProgressBar.minValue = 0;
        gasbrakeMinusProgressBar.maxValue = -1;

        steerProgressBar.minValue = -1;
        steerProgressBar.maxValue = 1;

        rpmPlusProgressBar.minValue = 0;
        rpmPlusProgressBar.maxValue = maxRPM;

        rpmMinusProgressBar.minValue = 0;
        rpmMinusProgressBar.maxValue = -maxRPM;

        steeringAngleProgressBar.minValue = -maxSteeringAngle;
        steeringAngleProgressBar.maxValue = maxSteeringAngle;

        handbrakeProgressBar.minValue = 0;
        handbrakeProgressBar.maxValue = 1;
    }

    public void GasBrake_performed(InputAction.CallbackContext obj)
    {
        gasbrake = obj.ReadValue<float>();
        gasbrakeMinusProgressBar.currentValue = gasbrake;
        gasbrakePlusProgressBar.currentValue = gasbrake;
    }
    public void Steer_performed(InputAction.CallbackContext obj)
    {
        steer = obj.ReadValue<float>();
        steerProgressBar.currentValue = steer;
    }

    public void Handbrake_performed(InputAction.CallbackContext obj)
    {
        handbrake = obj.ReadValue<float>() == 1;
        handbrakeProgressBar.currentValue = handbrake ? 1 : 0;
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
        rpmPlusProgressBar.currentValue = rpm;
        rpmMinusProgressBar.currentValue = rpm;
        steeringAngle = Mathf.Lerp(steeringAngle, (maxSteeringAngle * steer) * turningCurve.Evaluate(rpm / maxRPM), Time.deltaTime * steersens);
        steeringAngleProgressBar.currentValue = steeringAngle;
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
        if (wheelsTouching == 0)
        {
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
        }
        if (wheelsTouching != 4)
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
        rightFrontWheel.Turn(steeringAngle / maxSteeringAngle, false);
        leftFrontWheel.Turn(steeringAngle / maxSteeringAngle, true);

        UpdateSkid(rightFrontWheel,0.5f);
        UpdateSkid(leftFrontWheel,0.5f);
        UpdateSkid(rightRearWheel);
        UpdateSkid(leftRearWheel);

    }

    void UpdateSkid(WheelController wheel, float skidBound = 0.9f)
    {
        if (Mathf.Abs(Vector3.Dot(wheel.transform.forward.normalized, rb.velocity.normalized)) < skidBound)
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