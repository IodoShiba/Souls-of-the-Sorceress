using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

[System.Serializable]
public class BuffReceiver
{
    [SerializeField] BuffCompatibleList compatibleList;

    BuffDestination[] destinations = new BuffDestination[(int)BuffTypeID.MAX];

    public void Initialize(Mortal owner)
    {
        compatibleList.InitializeDestinationsArray(destinations, owner);
    }

    public void Update()
    {
        for(int i=0; i<destinations.Length; ++i)
        {
            if(destinations[i] != null){ destinations[i].Update(); }
        }
    }

    public void Receive(IBuffAffector item)
    {
        if((int)item.buffTypeID >= destinations.Length){ return; }

        item.Affect(destinations[(int)item.buffTypeID]);
    }
}
