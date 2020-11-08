using UnityEngine;
using Buffs;

namespace Buffs{

    [CreateAssetMenu(menuName = "Buffs/Buff Compatible List")]
    public class BuffCompatibleList : ScriptableObject
    {
        [SerializeField] BuffFunctorGenerator[] buffFunctorGenerators;

        public void InitializeFunctorsArray(BuffFunctor[] functors, Actor owner)
        {
            for(int i=0; i<buffFunctorGenerators.Length; ++i)
            {
                buffFunctorGenerators[i].SetFunctor(out functors[i], owner);
            }
        }
    }
}