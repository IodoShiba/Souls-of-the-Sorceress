using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaObjectDetector : MonoBehaviour
{
    [SerializeField] List<Sensor> sensors;

    //private void Update()
    //{
    //    int i = GetDetectingIndex();
    //    Debug.Log(i == -1 ? "No detection" : i.ToString());
    //}

    public int GetDetectingIndex()
    {
        for(int i = 0; i < sensors.Count; ++i)
        {
            if (sensors[i].IsDetecting)
            {
                return i;
            }
        }
        return -1;
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
        for (int i = 0; i < sensors.Count; ++i)
        {
            if (sensors[i].IsDetecting)
            {
                outArray.Add(i);
            }
        }
    }
}
