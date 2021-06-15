using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIntersect : MonoBehaviour
{
    public BoxCollider go;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<BoxCollider>().bounds.Intersects(go.bounds))
        {
            Debug.Log("INTERSECT" + Random.Range(0,10).ToString());
        }
    }
}
