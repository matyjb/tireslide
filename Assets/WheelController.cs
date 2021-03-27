using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public bool IsTouchingGround = true;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        if(collision.gameObject.tag == "ground")
        {
            IsTouchingGround = true;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
        if (collision.gameObject.tag == "ground")
        {
            IsTouchingGround = false;
        }
    }
}
