using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMove : MonoBehaviour {
    [SerializeField] float speed;
    [SerializeField] KeyCode temp_rightKey;
    [SerializeField] KeyCode temp_leftKey;

    public float Speed
    {
        set
        {
            speed = value;
        }
    }

    private void Update()
    {
        float d = 0;
        if (Input.GetKey(temp_rightKey))
        {
            d += speed;
        }
        if (Input.GetKey(temp_leftKey))
        {
            d -= speed;
        }
        gameObject.transform.position += new Vector3(d * Time.deltaTime, 0, 0);
    }

    void Act()
    {
        gameObject.transform.position += new Vector3(speed*Time.deltaTime, 0, 0);
    }
}
