using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    private bool isOnGround=false;
    public string groundTagName;

    public bool IsOnGround
    {
        get
        {
            return isOnGround;
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == groundTagName)
        {
            isOnGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == groundTagName)
        {
            isOnGround = false;
        }
    }
}
