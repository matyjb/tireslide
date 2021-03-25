﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float rpm = 0;
    public float maxRPM = 2;

    public float steeringAngle = 0;
    public float maxSteeringAngle = 30;

    public float power = 1f; // rpm gained per input (should be curve)
    public float steersens = 1f;

    private float steer = 0; // ranges -1 1
    private float gasbrake = 0; // ranges -1 1

    private Rigidbody rb;

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
    }

    void FixedUpdate()
    {
        GetInput();
        UpdateCarEngineAndSteering();
        ApplyForces();
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
}