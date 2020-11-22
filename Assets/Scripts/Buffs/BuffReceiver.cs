using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

namespace Buffs{
    [System.Serializable]
    public class BuffReceiver
    {
        [SerializeField] BuffCompatibleList compatibleList;

        BuffFunctor[] buffFunctors;

        public void Initialize(Actor owner)
        {
            buffFunctors = new BuffFunctor[(int)BuffTypeID.MAX];
            BuffCompatibleList.InitializeFunctors(compatibleList, buffFunctors, owner);
        }

        public void Update()
        {
            for(int i=0; i<buffFunctors.Length; ++i)
            {
                if(buffFunctors[i] != null)
                {
                    buffFunctors[i].Update(); 
                }
            }
        }

        public void Receive(IBuffAffector item)
        {
            if(item == null || (int)item.buffTypeID >= buffFunctors.Length){ return; }
            BuffFunctor func = buffFunctors[(int)item.buffTypeID];
            if(func == null){ return; }
            item.Affect(buffFunctors[(int)item.buffTypeID]);
        }

        public bool IsActive(BuffTypeID typeID)
        {
            int i = (int)typeID;
            return i < buffFunctors.Length && buffFunctors[i] != null && buffFunctors[i].IsActive;
        }

        public void GetView(BuffTypeID typeID, out BuffView view)
        {
            int i = (int)typeID;
            if(buffFunctors[i] == null || i >= buffFunctors.Length)
            {
                view = default(BuffView);
                return;
            }
            buffFunctors[i].GetView(out view);
        }
    }
}