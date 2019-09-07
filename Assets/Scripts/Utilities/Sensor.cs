using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour,ISerializationCallbackReceiver
{
    [SerializeField] private bool isDetecting;
    [SerializeField] string monitoredTagName;
    [SerializeField] bool useTargetGameObject;
    [SerializeField] List<GameObject> targetGameObjects;
    HashSet<GameObject> hsTargetGameObjects = new HashSet<GameObject>();

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
        if (IsTarget(collision))
        {
            isDetecting = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsTarget(collision))
        {
            isDetecting = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsTarget(collision))
        {
            isDetecting = false;
        }
    }

    bool IsTarget(Collider2D collision)
        =>
        (string.IsNullOrEmpty(monitoredTagName) || collision.gameObject.tag == monitoredTagName) &&
        (hsTargetGameObjects.Count == 0 || hsTargetGameObjects.Contains(collision.gameObject));

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        hsTargetGameObjects.Clear();
        if(targetGameObjects==null || targetGameObjects.Count == 0) { return; }
        for(int i = 0; i < targetGameObjects.Count; ++i)
        {
            hsTargetGameObjects.Add(targetGameObjects[i]);
        }
    }
}
