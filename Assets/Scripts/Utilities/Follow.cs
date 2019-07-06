using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
    [SerializeField] Transform target;
    [SerializeField] Vector2 relativePosition;
    private Transform selfT;
    private Vector3 position0;
    private Quaternion theta0;
    private Vector3 scale0;

	// Use this for initialization
	void Start () {
        selfT = transform;
        position0 = selfT.position;
        theta0 = selfT.rotation;
        scale0 = selfT.lossyScale;
        //selfT.position = target.position + new Vector3(relativePosition.x * target.lossyScale.x, relativePosition.y * target.lossyScale.y, transform.position.z);
        selfT.position = new Vector3(target.position.x + relativePosition.x * target.lossyScale.x, target.position.y + relativePosition.y * target.lossyScale.y, transform.position.z);
        selfT.localScale = new Vector3(target.lossyScale.x * scale0.x, target.lossyScale.y * scale0.y);
    }
	
	// Update is called once per frame
	void Update ()
    {
        selfT.position = new Vector3(target.position.x + relativePosition.x * target.lossyScale.x, target.position.y + relativePosition.y * target.lossyScale.y, transform.position.z);
        selfT.localScale = new Vector3(target.lossyScale.x * scale0.x, target.lossyScale.y * scale0.y);

    }
}
