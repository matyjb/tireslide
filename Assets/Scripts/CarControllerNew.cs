using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarControllerNew : MonoBehaviour, IResetable
{
    private Rigidbody rb;
    public WheelController rightFrontWheel;
    public WheelController leftFrontWheel;
    public WheelController rightRearWheel;
    public WheelController leftRearWheel;

    float gasbrakeInput = 0;
    float steerInput = 0;
    float handbrakeInput = 0;

    float rpm = 0, maxRPM = 8000;
    float steer = 0;
    float handbrake = 0;
    public ProgressBar engineRPMBar;
    public ProgressBar steeringBar;
    public ProgressBar handbrakeBar;

    public AnimationCurve[] gears; // list of gears where [0] gear is reverse gear
    public int gearSelected = 1;

    public float power = 0.7f;
    public float maxForwardVelocity = 32;
    public float maxAngle = 45;
    public float steerTorqueFactor = 180;
    public float handbrakeSteerFactor = 25;

    float initDrag;
    float initAngularDrag;

    public AnimationCurve powerCurvePerVelocity;

    public AnimationCurve gripPerVelocity;
    public AnimationCurve gripPerAngle;

    public AnimationCurve steeringWithHandbrakePerAngle;

    public AnimationCurve handbrakePowerPerVelocity;

    private AudioSource engineSound;
    private AudioSource tiresSound;
    public AudioClip wallBonkClip;
    public bool isSkidding = false;
    public bool isInAir = false;
    public bool ControlsEnabled = true;
    //public AudioClip engineLow;
    //public AudioClip engineMed;
    //public AudioClip engineHigh;
    //public AudioClip engineFull;
    //int currentAudioClip = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponents<AudioSource>()[0];
        tiresSound = GetComponents<AudioSource>()[1];
        initDrag = rb.drag;
        initAngularDrag = rb.angularDrag;

        engineRPMBar.maxValue = maxRPM;
        engineRPMBar.minValue = 0;

        steeringBar.maxValue = 1;
        steeringBar.minValue = -1;

        handbrakeBar.maxValue = 1;
        handbrakeBar.minValue = 0;
    }

    public void GasBrake_performed(InputAction.CallbackContext obj)
    {
        gasbrakeInput = obj.ReadValue<float>();
    }
    public void Steer_performed(InputAction.CallbackContext obj)
    {
        steerInput = obj.ReadValue<float>();
    }
    public void Handbrake_performed(InputAction.CallbackContext obj)
    {
        handbrakeInput = obj.ReadValue<float>();
    }

    public void ResetToInitial()
    {
        rb.velocity *= 0;
        rb.angularVelocity *= 0;

        rpm = steer = handbrake = 0;
        gearSelected = 1;
    }

    void FixedUpdate()
    {
        isSkidding = false;
        if (!ControlsEnabled)
        {
            handbrakeInput = 0;
            steerInput = 0;
            gasbrakeInput = 0;
        }
        UpdateCarEngineAndSteering();
        UpdateUI();
        ApplyForces();
        UpdateVisuals();
        UpdateEngineSounds();
        UpdateTiresSounds();
    }

    private void UpdateTiresSounds()
    {
        float velocityClamped = Mathf.Clamp(rb.velocity.magnitude, 0, 32);
        float volume = isSkidding && velocityClamped > 6 ? 1 : 0;

        tiresSound.volume = Mathf.Lerp(tiresSound.volume, volume, Time.deltaTime / 0.2f);
        tiresSound.pitch = 0.9f + 0.2f * velocityClamped / maxForwardVelocity;
    }

    private void UpdateEngineSounds()
    {
        float pitchDeltaLow = 0.6f;
        float pitchDeltaHigh = 0.9f;
        float rpmFactor = rpm / maxRPM;

        float newPitch = 1 - pitchDeltaLow + (1 + pitchDeltaHigh - pitchDeltaLow) * rpmFactor;
        if (handbrake > 0)
            newPitch *= 0.9f;
        if (rpm > maxRPM - 500)
            newPitch -= Mathf.PingPong(Time.time, 0.2f);

        engineSound.pitch = newPitch;
    }

    private void UpdateUI()
    {
        engineRPMBar.currentValue = rpm;
        steeringBar.currentValue = steer;
        handbrakeBar.currentValue = handbrake;
    }

    private void UpdateCarEngineAndSteering()
    {
        float brakeTime = 0.7f;
        float accelTime = 1f;

        isInAir = !(rightFrontWheel.IsTouchingGround ||
                  leftFrontWheel.IsTouchingGround ||
                  rightRearWheel.IsTouchingGround ||
                  leftRearWheel.IsTouchingGround);

        if (gearSelected != 0)
        {
            if (gasbrakeInput > 0)
            {
                rpm = Mathf.Lerp(rpm, gasbrakeInput * maxRPM, Time.deltaTime / accelTime);
            }
            else if (gasbrakeInput < 0)
            {
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / brakeTime / -gasbrakeInput);
            }
            else
            {
                float sidewaysAngle = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.forward));
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / Mathf.Pow(brakeTime * sidewaysAngle + brakeTime, 2));
            }
            steer = Mathf.Lerp(steer, steerInput, Time.deltaTime * 10f);
        }
        else
        {
            if (gasbrakeInput < 0)
            {
                rpm = Mathf.Lerp(rpm, -gasbrakeInput * maxRPM, Time.deltaTime / accelTime);
            }
            else if (gasbrakeInput > 0)
            {
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / brakeTime / gasbrakeInput);
            }
            else
            {
                float sidewaysAngle = Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.forward));
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / Mathf.Pow(brakeTime * sidewaysAngle + brakeTime, 2));
            }
            steer = Mathf.Lerp(steer, -steerInput, Time.deltaTime * 10f);
        }

        handbrake = handbrakeInput > 0 ? 1 : 0;

        //gears
        if (Vector3.Dot(rb.velocity, transform.forward) < 3f)
        {
            //can change gear
            if (gearSelected == 0)
            {
                if (gasbrakeInput > 0) gearSelected = 1;
            }
            else
            {
                if (gasbrakeInput < 0) gearSelected = 0;
            }
        }
    }

    public float handbrakeVelPenaltyFactor = 4f;

    private void ApplyForces()
    {
        rb.angularDrag = initAngularDrag;
        // drag
        if (!rightRearWheel.IsTouchingGround && !leftRearWheel.IsTouchingGround && !rightFrontWheel.IsTouchingGround && !leftFrontWheel.IsTouchingGround)
        {
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
        }
        else
        {
            rb.drag = initDrag * Mathf.Clamp(Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.forward)), 0.6f, 1);
        }

        bool isPlayerAccelerating = (gasbrakeInput > 0 && gearSelected > 0) || (gasbrakeInput < 0 && gearSelected == 0);
        // velocity
        if (rightRearWheel.IsTouchingGround && leftRearWheel.IsTouchingGround)
        {
            float forwardVelocity = Mathf.Abs(Vector3.Dot(rb.velocity, transform.forward));
            float vel = power * powerCurvePerVelocity.Evaluate(forwardVelocity / maxForwardVelocity) * gears[gearSelected].Evaluate(rpm / maxRPM);
            // grip of tyres
            if (rb.velocity.magnitude > 0.1f)
            {
                vel *= Mathf.Clamp(Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.forward)), 0.7f, 1);
            }
            //handbrake
            if (handbrake != 0)
            {
                vel *= 1 - handbrakePowerPerVelocity.Evaluate(rb.velocity.magnitude / maxForwardVelocity);
                rb.velocity *= 0.995f;
            }
            // brakes
            if ((gasbrakeInput > 0 && gearSelected == 0) || (gasbrakeInput < 0 && gearSelected > 0))
            {
                rb.velocity *= 0.99f;
            }

            if (isPlayerAccelerating)
            {
                rb.velocity += transform.forward * vel;
            }
            // when car is freewheeling
            else
            {
                rb.velocity += transform.forward * vel * 0.4f * Mathf.Abs(Vector3.Dot(transform.right, rb.velocity.normalized));
            }

        }
        // turning
        if (rightFrontWheel.IsTouchingGround && leftFrontWheel.IsTouchingGround)
        {
            // normal
            float steerTorque = steer * maxAngle * gripPerAngle.Evaluate(Vector3.Dot(rb.velocity.normalized, transform.forward));
            if (!isPlayerAccelerating)
            {
                steerTorque *= Mathf.Clamp(Mathf.Abs(Vector3.Dot(rb.velocity.normalized, transform.forward)), 0.3f, 1f);
            }
            // handbrake
            steerTorque += (Vector3.Dot(transform.right, rb.velocity.normalized) > 0 ? -1 : 1) * handbrake * handbrakeSteerFactor * steeringWithHandbrakePerAngle.Evaluate(Vector3.Dot(rb.velocity.normalized, transform.forward));

            steerTorque *= gripPerVelocity.Evaluate(rb.velocity.magnitude / maxForwardVelocity);
            rb.AddTorque(transform.rotation * new Vector3(0, steerTorque * steerTorqueFactor, 0));
        }
    }

    private void UpdateVisuals()
    {
        if (gearSelected != 0)
        {
            rightFrontWheel.Turn(steer, false);
            leftFrontWheel.Turn(steer, true);
        }
        else
        {
            rightFrontWheel.Turn(-steer, false);
            leftFrontWheel.Turn(-steer, true);
        }

        UpdateSkid(rightFrontWheel, true, 0.5f);
        UpdateSkid(leftFrontWheel, true, 0.5f);
        UpdateSkid(rightRearWheel);
        UpdateSkid(leftRearWheel);
    }

    private void UpdateSkid(WheelController wheel, bool isFront = false, float skidBound = 0.9f)
    {
        if (Mathf.Abs(Vector3.Dot(wheel.transform.forward.normalized, rb.velocity.normalized)) < skidBound || (!isFront && handbrake != 0))
        {
            wheel.StartSkid();
            isSkidding = true;
        }
        else
        {
            wheel.EndSkid();
            isSkidding = false;
        }

        if (!wheel.IsTouchingGround)
        {
            wheel.EndSkid();
            isSkidding = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            engineSound.PlayOneShot(wallBonkClip);
        }
    }
}