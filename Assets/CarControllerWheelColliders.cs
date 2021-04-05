using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarControllerWheelColliders : MonoBehaviour
{
    private Rigidbody rb;
    public WheelController rightFrontWheel;
    public WheelController leftFrontWheel;
    public WheelController rightRearWheel;
    public WheelController leftRearWheel;
    public WheelCollider rightFrontWheelCollider;
    public WheelCollider leftFrontWheelCollider;
    public WheelCollider rightRearWheelCollider;
    public WheelCollider leftRearWheelCollider;

    float gasbrakeInput = 0;
    float steerInput = 0;
    float handbrakeInput = 0;

    float rpm = 0, maxRPM = 8000;
    float steer = 0;
    float handbrake = 0;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
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
            rpm = Mathf.Lerp(rpm, 0, Time.deltaTime / 1f);
        }
        steer = Mathf.Lerp(steer, steerInput, Time.deltaTime * 10f);
        handbrake = handbrakeInput > 0 ? 1 : 0;

        rightRearWheelCollider.motorTorque = rpm * 100;
        leftRearWheelCollider.motorTorque = rpm * 100;
        rightFrontWheelCollider.steerAngle = steer * 45;
        leftFrontWheelCollider.steerAngle = steer * 45;
    }
}
