using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager instance;

    public TextMeshProUGUI textPoints;
    public TextMeshProUGUI textMultiplier;

    public int Points { get; private set; } = 0;
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

    public void AddScaledPoints(int amount)
    {
        Points += amount * multiplier;
    }

    public void AddUnscaledPoints(int amount)
    {
        Points += amount;
    }

    // Update is called once per frame
    void Update()
    {
        pointsFollower = Mathf.Lerp(pointsFollower, Points, Time.deltaTime / 0.2f);
        textPoints.text = Mathf.RoundToInt(pointsFollower).ToString();
        textMultiplier.text = string.Format("x{0}", multiplier);
    }
}
