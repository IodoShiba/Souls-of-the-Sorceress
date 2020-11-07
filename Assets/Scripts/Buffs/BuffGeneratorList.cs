using UnityEngine;
using Buffs;

namespace Buffs{

    [CreateAssetMenu(menuName = "Buffs/Buff Generator List")]
    public class BuffCompatibleList : ScriptableObject
    {
        [SerializeField] BuffDestinationGenerator[] buffDestinationGenerators;

        public void InitializeDestinationsArray(BuffDestination[] destinations, Mortal owner)
        {
            for(int i=0; i<buffDestinationGenerators.Length; ++i)
            {
                buffDestinationGenerators[i].SetDestination(out destinations[i], owner);
            }
        }
    }
}