using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class WheelController : MonoBehaviour
{
    public bool IsTouchingGround = true;

    TrailRenderer[] trails;
    ParticleSystem[] smokes;

    private Vector3 steerRotationDelta = new Vector3(-4.5f, 30, 12);
    private Quaternion steerRestingRotationRight;
    private Quaternion steerRestingRotationLeft;

    // Start is called before the first frame update
    void Start()
    {
        trails = gameObject.GetComponentsInChildren<TrailRenderer>();
        smokes = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    public void FixedUpdate()
    {
        IsTouchingGround = Physics.Raycast(transform.position, -transform.up, 0.35f);
    }


    public void Turn(float steer, bool isLeft)
    {
        float steeringTime = 10f;
        if (steer != 0)
        {
            //steering
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler((isLeft ? steerRestingRotationLeft : steerRestingRotationRight).eulerAngles + steerRotationDelta * steer), steeringTime * Time.deltaTime);
        }
        else
        {
            //not steering
            transform.localRotation = Quaternion.Lerp(transform.localRotation, isLeft ? steerRestingRotationLeft : steerRestingRotationRight, steeringTime * Time.deltaTime);
        }
    }

    public void StartSkid()
    {
        foreach (var t in trails)
        {
            t.emitting = true;
        }
        foreach (var s in smokes)
        {
            EmissionModule emission = s.emission;
            emission.enabled = true;
        }
    }

    public void EndSkid()
    {
        foreach (var t in trails)
        {
            t.emitting = false;
        }
        foreach (var s in smokes)
        {
            EmissionModule emission = s.emission;
            emission.enabled = false;
        }
    }
}
