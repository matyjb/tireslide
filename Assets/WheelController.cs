using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class WheelController : MonoBehaviour
{
    public bool IsTouchingGround = true;

    TrailRenderer trail;
    ParticleSystem smoke;
    public float dist = 0.5f;

    private Vector3 steerRotationDelta = new Vector3(-4.5f, 30, 12);
    private Quaternion steerRestingRotationRight;
    private Quaternion steerRestingRotationLeft;

    // Start is called before the first frame update
    void Start()
    {
        trail = gameObject.GetComponentsInChildren<TrailRenderer>()[0];
        smoke = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
    }

    public void FixedUpdate()
    {
        IsTouchingGround = Physics.Raycast(trail.transform.position, -transform.up, dist);
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
        trail.emitting = true;
        EmissionModule emission = smoke.emission;
        emission.enabled = true;
    }

    public void EndSkid()
    {
        trail.emitting = false;
        EmissionModule emission = smoke.emission;
        emission.enabled = false;
    }
}
