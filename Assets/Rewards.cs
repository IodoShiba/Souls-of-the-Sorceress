using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public RewardData rewardData;
    ActionAwake playerActionAwake;

    private void Awake()
    {
        foreach(RewardData.RewardDatum rd in rewardData.Rewards)
        {
            switch (rd.Type)
            {
                case RewardData.RewardType.addAwakeGaugeOnDead:
                    var enemy = GetComponent<Enemy>();
                    
                    enemy.DyingCallbacks.AddListener(
                        () =>
                        (playerActionAwake == null ? (playerActionAwake = ActorManager.PlayerActor.GetComponent<ActionAwake>()) : playerActionAwake)
                            .AddAwakeGauge(rd.FloatAmount)
                        );
                    break;
            }
        }
    }
}
