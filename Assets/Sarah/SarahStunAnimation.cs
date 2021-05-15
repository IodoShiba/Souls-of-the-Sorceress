using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarahStunAnimation : MonoBehaviour
{

    [SerializeField] ActorSarah.ActorStateConnectorSarah asc;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        asc = GetComponentInParent<ActorSarah.ActorStateConnectorSarah>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(typeof(SarahStunAnimation).ToString());
        Debug.Log(asc.Current.GetType());
        if(asc.Current.GetType() == typeof(Buffs.StunFunctor.StunState))
        {
            sr.enabled = true;
        }
        else
        {
            sr.enabled = false;
        }
    }
}
