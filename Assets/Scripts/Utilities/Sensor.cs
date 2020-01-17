using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour,ISerializationCallbackReceiver
{
    [SerializeField] string monitoredTagName;
    [SerializeField] bool useTargetGameObject;
    [SerializeField] List<GameObject> targetGameObjects;
    [SerializeField] bool useCountUpdateCycle;
    [SerializeField] float countUpdateCycle;
    [SerializeField,DisabledField] int detectCount;
    HashSet<GameObject> hsTargetGameObjects = new HashSet<GameObject>();
    Collider2D collider;
    Collider2D[] overlapRes = new Collider2D[MAX_DETECT];
    HashSet<Collider2D> detected = new HashSet<Collider2D>();

    const int MAX_DETECT = 32;

    public bool IsDetecting
    {
        get
        {
            return detectCount > 0;
        }

    }

    public int DetectCount { get => detectCount; }

    private void Awake()
    {
        detectCount = 0;
        collider = GetComponent<Collider2D>();
        overlapRes = new Collider2D[MAX_DETECT];
    }
    public void Reset()
    {
        detectCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsTarget(collision))
        {
            detected.Add(collision);
            detectCount = detected.Count;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsTarget(collision))
        {
            detected.Remove(collision);
            detectCount = detected.Count;
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

    void Update()
    {
         ForceUpdate();
    }


    public void ForceUpdate()
    {
        detected.RemoveWhere(c => c == null); 
        //string log = "ForceUpdate\n";

        //Collider2D collider = this.collider;
        //Collider2D[] overlapRes = new Collider2D[MAX_DETECT];
        //for (int i = 0; i < overlapRes.Length; ++i)
        //{
        //    overlapRes[i] = null;
        //}
        //ContactFilter2D contactFilter = new ContactFilter2D();
        //contactFilter.layerMask.value = LayerMask.NameToLayer("Enemys");
        //contactFilter.useTriggers = true;

        //log += "overlap:";

        //detectCount = UnityEngine.Physics2D.OverlapCollider(collider, contactFilter, overlapRes);
        //Debug.Log(detectCount);
        //for (int i = 0; i < overlapRes.Length; ++i)
        //{
        //    if (overlapRes[i] != null && IsTarget(overlapRes[i]))
        //    {
        //        log += overlapRes[i].name + ',';
        //        Debug.Log(detectCount);
        //        overlapRes[i] = null;
        //    }
        //}

        //Debug.Log(log);
    }
}
