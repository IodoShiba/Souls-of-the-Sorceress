using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUmbrellaParameterViewer : UmbrellaParameters.Viewer
{
    [SerializeField] List<UnityEngine.UI.Toggle> gaugeElements;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < target.MaxDurability; ++i)
        {
            gaugeElements[i].isOn = i < target.Durability;
        }
    }
}
