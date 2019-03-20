using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDealer : MonoBehaviour
{
    [SerializeField] List<AttackConverter> dealingAttackConverters;

    public void ConvertDealingAttack(AttackData subject)
    {
        dealingAttackConverters.ForEach(dac => dac.Convert(subject));
    }
}
