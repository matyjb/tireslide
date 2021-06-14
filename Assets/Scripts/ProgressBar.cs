using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float maxValue = 1;
    public float minValue = 0;

    public float currentValue = 0;
    public float scaleDivider = 1;

    private RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float cl = minValue < maxValue ? Mathf.Clamp(currentValue, minValue, maxValue) : Mathf.Clamp(currentValue, maxValue, minValue);
        rt.localScale = new Vector3(rt.localScale.x, cl/(maxValue-minValue), rt.localScale.z);
    }
}
