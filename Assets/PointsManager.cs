using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour, IResetable
{
    public static PointsManager instance;

    public TextMeshProUGUI textPoints;
    public TextMeshProUGUI textMultiplier;

    public float Points { get; private set; } = 0;
    private float pointsFollower = 0;


    public int multiplier = 1;

    private void Awake()
    {
        instance = this;
    }

    public void AddMultiplier(int multiplier)
    {
        this.multiplier += multiplier;
        StartCoroutine(WaitAndSubtractMultiplier(multiplier));
    }

    private IEnumerator WaitAndSubtractMultiplier(int divider)
    {
        yield return new WaitForSeconds(10);
        multiplier -= divider;
    }

    public void AddScaledPoints(float amount)
    {
        Points += amount * multiplier;
        Points = Mathf.Max(Points, 0);
    }

    public void AddUnscaledPoints(float amount)
    {
        Points += amount;
        Points = Mathf.Max(Points, 0);
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
