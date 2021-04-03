﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarControllerNew : MonoBehaviour
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
    public float maxAngle = 60;
    public float steerTorqueFactor = 200;
    public float handbrakeSteerFactor = 25;

    float initDrag;
    float initAngularDrag;

    public AnimationCurve powerCurvePerVelocity;

    public AnimationCurve gripPerVelocity;
    public AnimationCurve gripPerAngle;

    public AnimationCurve steeringWithHandbrakePerAngle;

    public AnimationCurve handbrakePowerPerVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    void FixedUpdate()
    {
        UpdateCarEngineAndSteering();
        UpdateUI();
        ApplyForces();
        UpdateVisuals();
    }

    private void UpdateUI()
    {
        engineRPMBar.currentValue = rpm;
        steeringBar.currentValue = steer;
        handbrakeBar.currentValue = handbrake;
    }

    private void UpdateCarEngineAndSteering()
    {
        if (gearSelected != 0)
        {
            if (gasbrakeInput > 0)
            {
                rpm = Mathf.Lerp(rpm, gasbrakeInput * maxRPM, Time.deltaTime / 0.4f);
            }
            else if (gasbrakeInput < 0)
            {
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / 0.4f / -gasbrakeInput);
            }
            else
            {
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / 2f);
            }
            steer = Mathf.Lerp(steer, steerInput, Time.deltaTime * 10f);
        }
        else
        {
            if (gasbrakeInput < 0)
            {
                rpm = Mathf.Lerp(rpm, -gasbrakeInput * maxRPM, Time.deltaTime / 0.4f);
            }
            else if (gasbrakeInput > 0)
            {
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / 0.4f / gasbrakeInput);
            }
            else
            {
                rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / 2f);
            }
            steer = Mathf.Lerp(steer, -steerInput, Time.deltaTime * 10f);
        }
        
        handbrake = handbrakeInput > 0 ? 1 : 0;

        //gears
        if (Vector3.Dot(rb.velocity.normalized, transform.forward) < 0.1f)
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

    private void ApplyForces()
    {
        // drag
        rb.drag = initDrag;
        rb.angularDrag = initAngularDrag;
        if (!rightRearWheel.IsTouchingGround && !leftRearWheel.IsTouchingGround && !rightFrontWheel.IsTouchingGround && !leftFrontWheel.IsTouchingGround)
        {
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
        }

        // velocity
        if (rightRearWheel.IsTouchingGround && leftRearWheel.IsTouchingGround)
        {
            if (gasbrakeInput != 0)
            {
                float forwardVelocity = Mathf.Abs(Vector3.Dot(rb.velocity, transform.forward));
                rb.velocity += transform.forward * power * powerCurvePerVelocity.Evaluate(forwardVelocity / maxForwardVelocity) * gears[gearSelected].Evaluate(rpm / maxRPM);
                //handbrake* handbrakePowerPerVelocity.Evaluate(rb.velocity.magnitude / maxForwardVelocity);
            }
        }

        // turning
        if (rightFrontWheel.IsTouchingGround && leftFrontWheel.IsTouchingGround)
        {
            // normal
            float steerTorque = steer * maxAngle * gripPerAngle.Evaluate(Vector3.Dot(rb.velocity.normalized, transform.forward));
            // handbrake
            steerTorque += (Vector3.Dot(transform.right,rb.velocity.normalized) > 0 ? -1 : 1) * handbrake * handbrakeSteerFactor * steeringWithHandbrakePerAngle.Evaluate(Vector3.Dot(rb.velocity.normalized, transform.forward));

            steerTorque *= gripPerVelocity.Evaluate(rb.velocity.magnitude / maxForwardVelocity);
            rb.AddTorque(transform.rotation * new Vector3(0, steerTorque * steerTorqueFactor, 0));
        }
    }

    private void UpdateVisuals()
    {
        rightFrontWheel.Turn(steer, false);
        leftFrontWheel.Turn(steer, true);

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