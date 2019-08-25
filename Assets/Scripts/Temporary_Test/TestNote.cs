using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNote : MonoBehaviour
{
    [TagField,SerializeField] string tagSerField;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Given Tag:{tagSerField},IsNullOrEmpty({nameof(tagSerField)})=={string.IsNullOrEmpty(tagSerField)}");
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
