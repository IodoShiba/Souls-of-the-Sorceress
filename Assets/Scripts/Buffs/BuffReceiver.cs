using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

[System.Serializable]
public class BuffReceiver
{
    [SerializeField] BuffCompatibleList compatibleList;

    BuffFunctor[] buffFunctors = new BuffFunctor[(int)BuffTypeID.MAX];

    public void Initialize(Actor owner)
    {
        compatibleList.InitializeFunctorsArray(buffFunctors, owner);
    }

    public void Update()
    {
        for(int i=0; i<buffFunctors.Length; ++i)
        {
            if(buffFunctors[i] != null){ buffFunctors[i].Update(); }
        }
    }

    public void Receive(IBuffAffector item)
    {
        if((int)item.buffTypeID >= buffFunctors.Length){ return; }

        item.Affect(buffFunctors[(int)item.buffTypeID]);
    }
}
