using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayBlockedBy : MonoBehaviour {

    [SerializeField] string targetTagName;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == targetTagName)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == targetTagName)
        {
            Destroy(gameObject);
        }
    }
}
