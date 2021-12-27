using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DriftPoints : MonoBehaviour, IResetable
{
    CarControllerNew ccn;
    Rigidbody rb;

    public TextMeshProUGUI driftAirText;

    float pointsDrift = 0;
    float pointsAir = 0;

    float pointsDriftLerp = 0;
    float pointsAirLerp = 0;

    float lastPointsGained = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        ccn = GetComponent<CarControllerNew>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        pointsDriftLerp = Mathf.Lerp(pointsDriftLerp, pointsDrift, Time.deltaTime / 0.1f);
        pointsAirLerp = Mathf.Lerp(pointsAirLerp, pointsAir, Time.deltaTime / 0.1f);

        if (pointsDriftLerp > 1)
        {
            driftAirText.text = Mathf.RoundToInt(pointsDriftLerp).ToString();
            driftAirText.alpha = Mathf.Lerp(driftAirText.alpha, 1, Time.deltaTime / 0.1f);
        }
        else if (pointsAirLerp > 1)
        {
            driftAirText.text = Mathf.RoundToInt(pointsAirLerp).ToString();
            driftAirText.alpha = Mathf.Lerp(driftAirText.alpha, 1, Time.deltaTime / 0.1f);
        }
        else
        {
            driftAirText.text = Mathf.RoundToInt(lastPointsGained).ToString();
            driftAirText.alpha = Mathf.Lerp(driftAirText.alpha, 0, Time.deltaTime / 0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            ResetToInitial();
        }
    }

    void FixedUpdate()
    {
        bool pointsGained = false;
        float pitchFactor = 1;
        // dodawanie punktów jesli IsSkidding jest true
        if (ccn.isSkidding && GameManager.instance.GameState == GameState.Playing)
        {
            // faster you go = more points
            float velFactor = Mathf.Clamp(rb.velocity.magnitude, 0, ccn.maxForwardVelocity) / ccn.maxForwardVelocity;

            // bigger angle = more points
            float angleFactor = 1 - Mathf.Clamp(Vector3.Dot(rb.velocity.normalized, transform.forward), 0, 1);

            float points = velFactor * angleFactor * PointsManager.instance.multiplier;
            pointsGained = points > 0.1f;
            pitchFactor = 0.9f + velFactor * angleFactor * 0.2f;

            RaycastHit[] hits = Physics.RaycastAll(transform.position+transform.up*2, -transform.up);
            if (hits.Any((hit)=>hit.collider.tag == "driftable"))
            {
                pointsDrift += points;
            }
        }
        else
        {
            if (pointsDrift >= 1)
            {
                PointsManager.instance.AddUnscaledPoints((int)pointsDrift, false);
                PointsManager.instance.audioSourceNormal.PlayOneShot(PointsManager.instance.driftAirPointsGainAudio, 0.5f);
                lastPointsGained = pointsDrift;
                pointsDrift = pointsDriftLerp = 0;
            }
        }
        // dodawanie pkt jesli jest w powietrzu
        if (ccn.isInAir && GameManager.instance.GameState == GameState.Playing)
        {
            float points = PointsManager.instance.multiplier;
            pointsGained = points > 0.1f;
            pitchFactor = 1 + points / 10f;
            pointsAir += points;
        }
        else
        {
            if (pointsAir >= 1)
            {
                PointsManager.instance.AddUnscaledPoints((int)pointsAir, false);
                PointsManager.instance.audioSourceNormal.PlayOneShot(PointsManager.instance.driftAirPointsGainAudio, 0.5f);
                lastPointsGained = pointsAir;
                pointsAir = pointsAirLerp = 0;
            }
        }

        PointsManager.instance.audioSourceDriftAirCombo.volume = pointsGained && PointsManager.instance.multiplier != 0 ? 1 : 0;
        PointsManager.instance.audioSourceDriftAirCombo.pitch = pitchFactor;

    }

    public void ResetToInitial()
    {
        pointsDrift = pointsAir = pointsDriftLerp = pointsAirLerp = lastPointsGained = 0;
    }
}
