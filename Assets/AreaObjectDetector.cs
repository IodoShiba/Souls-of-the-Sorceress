using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaObjectDetector : MonoBehaviour
{
    public const int NO_DETECT = -1;

    [SerializeField] List<Collider2D> colliders;
    [SerializeField] Transform target;
    //private void Update()
    //{
    //    int i = GetDetectingIndex();
    //    Debug.Log(i == -1 ? "No detection" : i.ToString());
    //}

    public int GetDetectingIndex()
    {
        for(int i = 0; i < colliders.Count; ++i)
        {
            if (colliders[i].OverlapPoint(target.position))
            {
                return i;
            }
        }
        return NO_DETECT;
    }

    public List<int> GetDetectingIndexes()
    {
        List<int> retArray = new List<int>();
        GetDetectingIndexes(retArray);
        return retArray;
    }

    public void GetDetectingIndexes(List<int> outArray)
    {
        outArray.Clear();
        for (int i = 0; i < colliders.Count; ++i)
        {
            if (colliders[i].OverlapPoint(target.position))
            {
                outArray.Add(i);
            }
        }
    }
}
