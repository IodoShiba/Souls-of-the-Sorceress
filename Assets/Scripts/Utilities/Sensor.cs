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

    const int MAX_DETECT = 8;

    public bool IsDetecting
    {
        get
        {
            return detectCount>0;
        }

    }

    public int DetectCount { get => detectCount; }

    private void Awake()
    {
        detectCount = 0;
        StartCoroutine(UpdateCo());
    }
    public void Reset()
    {
        detectCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsTarget(collision))
        {
            detectCount++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsTarget(collision))
        {
            detectCount--;
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

    IEnumerator UpdateCo()
    {
        Collider2D collider = GetComponent<Collider2D>();
        Collider2D[] overlapRes = new Collider2D[MAX_DETECT];
        while (true)
        {
            for(int i = 0; i < overlapRes.Length; ++i)
            {
                overlapRes[i] = null;
            }
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
            UnityEngine.Physics2D.OverlapCollider(collider,contactFilter,overlapRes);

            detectCount = 0;
            for (int i = 0; i < overlapRes.Length; ++i)
            {
                if(overlapRes[i] != null && IsTarget(overlapRes[i]))
                {
                    ++detectCount;
                }
            }

            yield return new WaitForSeconds(useCountUpdateCycle ? countUpdateCycle : 2); 
        }
    }
}
