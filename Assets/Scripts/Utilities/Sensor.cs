using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] private bool isDetecting;
    [SerializeField] string monitoredTagName;

    public bool IsDetecting
    {
        get
        {
            return isDetecting;
        }

    }

    public void Reset()
    {
        isDetecting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == monitoredTagName)
        {
            isDetecting = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == monitoredTagName)
        {
            isDetecting = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == monitoredTagName)
        {
            isDetecting = false;
        }
    }
}
