using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Skid : MonoBehaviour
{

    TrailRenderer[] trails;
    ParticleSystem[] smokes;

    // Start is called before the first frame update
    void Start()
    {
        trails = gameObject.GetComponentsInChildren<TrailRenderer>();
        smokes = gameObject.GetComponentsInChildren<ParticleSystem>();
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
