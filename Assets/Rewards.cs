using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour,Mortal.IDyingCallbackReceiver
{
    public RewardData rewardData;
    //ActionAwake playerActionAwake;

    public void OnSelfDying(Mortal.DealtAttackInfo causeOfDeath)
    {
        foreach (RewardData.RewardDatum rd in rewardData.Rewards)
        {
            switch (rd.Type)
            {
                case RewardData.RewardType.addAwakeGaugeOnDead:

                    var enemy = GetComponent<Enemy>();
                    if (ActorManager.PlayerActor.GetComponent<Mortal>() == causeOfDeath.attacker)
                    {
                        //ActorManager.PlayerActor.GetComponent<ActionAwake>().AddAwakeGauge(rd.FloatAmount);
                    }
                    break;
            }
        }
    }

    //private void Awake()
    //{
    //    foreach(RewardData.RewardDatum rd in rewardData.Rewards)
    //    {
    //        switch (rd.Type)
    //        {
    //            case RewardData.RewardType.addAwakeGaugeOnDead:
    //                var enemy = GetComponent<Enemy>();
                    
    //                enemy.DyingCallbacks.AddListener(
    //                    () =>
    //                    (playerActionAwake == null ? (playerActionAwake = ActorManager.PlayerActor.GetComponent<ActionAwake>()) : playerActionAwake)
    //                        .AddAwakeGauge(rd.FloatAmount)
    //                    );
    //                break;
    //        }
    //    }
    //}


}
