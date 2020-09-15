using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSendBackAttack : MonoBehaviour
{
    [System.Serializable] public class UnityEvent_Mortal : UnityEngine.Events.UnityEvent<Mortal> { }

    [SerializeField] Mortal defender;
    [SerializeField] AttackData baseData;
    [SerializeField] AttackConverter[] attackConverters;
    [SerializeField] public UnityEvent_Mortal onAttackSucceeded; // カスが代
    AttackData buf = new AttackData();
    
    public void OnGuarded(Mortal attacker, AttackData dealt)
    {
        AttackData.Copy(buf, baseData);
        for(int i=0;i<attackConverters.Length; ++i)
        {
            attackConverters[i].Convert(buf);
        }
        attacker.TryAttack(
            defender, buf, transform.position - attacker.transform.position, 
            (isSuccess, subjectMortal)=> 
            {
                if (isSuccess) {
                    onAttackSucceeded.Invoke(subjectMortal);
                    Actor actor;
                    if(subjectMortal != null && subjectMortal.TryGetActor(out actor))
                    {
                        var fasc = actor.FightAsc;
                        fasc.InterruptWith(fasc.Smashed);
                    }
                }
            });
    }
}
