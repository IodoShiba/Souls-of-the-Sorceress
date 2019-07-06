using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalActivator : MonoBehaviour
{
    Mortal mortal;
    private void Awake()
    {
        mortal = GetComponent<Mortal>();
    }

    // Update is called once per frame
    void Update()
    {
        mortal.ManualUpdate();
    }
}
