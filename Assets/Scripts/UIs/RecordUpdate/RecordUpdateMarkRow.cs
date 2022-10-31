using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordUpdateMarkRow : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text[] marks;

    public void SetMarkVisible(int i)
    {
        marks[i].gameObject.SetActive(true);
    }
}
