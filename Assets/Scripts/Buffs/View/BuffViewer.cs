using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Buffs
{
    public class BuffViewer : MonoBehaviour
    {
        [SerializeField] Actor target;
        [SerializeField] Text text;

        // Update is called once per frame
        void Update()
        {
            string outstr = "";
            
            BuffReceiver buffReceiver = target.BuffReceiver;

            for(int i = 0; i<(int)BuffTypeID.MAX; ++i)
            {
                BuffTypeID tyi = (BuffTypeID)i;
                bool isActive = buffReceiver.IsActive(tyi);
                if(!isActive){ continue; }
                BuffView v;
                buffReceiver.GetView(tyi, out v);
                outstr += v.ToString();
            }

            if(string.IsNullOrEmpty(outstr)){ outstr = "No buff"; }

            text.text = outstr;
        }
    }
}