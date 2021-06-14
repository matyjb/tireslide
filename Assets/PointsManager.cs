using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PointsManager : MonoBehaviour, IResetable
{
    public static PointsManager instance;

    public TextMeshProUGUI textPoints;
    public TextMeshProUGUI textMultiplier;

    public float Points { get; private set; } = 0;
    private float pointsFollower = 0;

    public AudioSource audioSourceNormal;
    public AudioSource audioSourceDriftAirCombo;
    public AudioClip bonusPointsAudio;
    public AudioClip multiplierAudio;
    public AudioClip driftAirPointsGainAudio;

    public int multiplier = 1;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        audioSourceNormal = GetComponents<AudioSource>()[1];
        audioSourceDriftAirCombo = GetComponents<AudioSource>()[2];
    }

    public void AddMultiplier(int multiplier)
    {
        this.multiplier += multiplier;
        audioSourceNormal.PlayOneShot(multiplierAudio);
        StartCoroutine(WaitAndSubtractMultiplier(multiplier));
    }

    private IEnumerator WaitAndSubtractMultiplier(int divider)
    {
        yield return new WaitForSeconds(10);
        multiplier -= divider;
    }

    public void AddScaledPoints(float amount, bool playSound = true)
    {
        Points += amount * multiplier;
        Points = Mathf.Max(Points, 0);
        if (amount > 1 && playSound)
            audioSourceNormal.PlayOneShot(bonusPointsAudio);
    }

    public void AddUnscaledPoints(float amount, bool playSound = true)
    {
        Points += amount;
        Points = Mathf.Max(Points, 0);
        if (amount > 1 && playSound)
            audioSourceNormal.PlayOneShot(bonusPointsAudio);
    }

    // Update is called once per frame
    void Update()
    {
        pointsFollower = Mathf.Lerp(pointsFollower, Points, Time.deltaTime / 0.2f);
        textPoints.text = Mathf.RoundToInt(pointsFollower).ToString();
        textMultiplier.text = string.Format("x{0}", multiplier);
    }

    public void ResetToInitial()
    {
        Points = 0;
        pointsFollower = 0;
        multiplier = 1;
    }
}
