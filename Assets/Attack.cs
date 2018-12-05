using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack : MonoBehaviour {
    [SerializeField] AttackData data;
    [SerializeField] string targetTag;
    [SerializeField] float lifespan;
    // Use this for initialization
    void Start () {
        if (data.Sprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = data.Sprite;
        }
        if (lifespan > 0)
        {
            Destroy(gameObject, lifespan);
        }
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject hit = collider.gameObject;
        if (hit.tag == targetTag)
        {
            hit.GetComponent<Mortal>().OnAttackedInternal(gameObject,data);
        }
        else if(hit.tag == "Ground")
        {
            //仮
            Destroy(gameObject);
        }
    }
    
}
