using UnityEngine;
using Buffs;

namespace Buffs{

    [CreateAssetMenu(menuName = "Buffs/Buff Compatible List")]
    public class BuffCompatibleList : ScriptableObject
    {
        [SerializeField] BuffFunctorGenerator[] buffFunctorGenerators;

        void InitializeFunctorsArray(BuffFunctor[] functors, Actor owner)
        {
            for(int i=0; i<buffFunctorGenerators.Length; ++i)
            {
                buffFunctorGenerators[i].SetFunctor(out functors[i], owner);
            }
        }

        public static void InitializeFunctors(BuffCompatibleList compatibleList, BuffFunctor[] functors, Actor owner)
        {
            if(compatibleList == null) 
            {
                DefaultInitialize(functors, owner); 
            }
            else
            {
                compatibleList.InitializeFunctorsArray(functors, owner);
            }
        }

        static void DefaultInitialize(BuffFunctor[] functors, Actor owner)
        {
            functors[(int)BuffTypeID.Stun] = StunFunctorGenerator.GetStunFunctor(owner);
        }
    }
}