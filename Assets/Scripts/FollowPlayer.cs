using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private float y;

    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = player.transform.position - transform.position;
        y = transform.position.y;
    }

    void Update()
    {
        Vector3 tmp = new Vector3(offset.x, 0, offset.z);
        transform.position = player.transform.position + tmp;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
