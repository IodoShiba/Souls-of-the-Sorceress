using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    namespace Awakening
    {
        public class Ordinary : State
        {
            [SerializeField] InputA inputA;
            [SerializeField] Player player;
            [SerializeField] ActionAwake playerAwake;
            bool initialized = false;

            public override State Check()
            {
                /*
                if (initialized&&inputA.GetButtonShortDownUp("Awake"))
                {
                    if (player.AwakeGauge >= 1.0)
                    {
                        return GetComponent<PlayerStates.Awakening.BlueAwaken>();
                    }
                    else if (player.AwakeGauge >= 0.5)
                    {
                        return GetComponent<PlayerStates.Awakening.Awaken>();
                    }
                }
                */
                if (initialized)
                {
                    switch (playerAwake.AwakeGrade)
                    {
                        case ActionAwake.AwakeGrades.awaken:
                            return GetComponent<PlayerStates.Awakening.Awaken>();
                        case ActionAwake.AwakeGrades.blueAwaken:
                            return GetComponent<PlayerStates.Awakening.BlueAwaken>();
                    }
                }
                return null;
            }

            public override void Initialize()
            {
                initialized = true;
            }

            public override void Execute()
            {
            }

            public override void Terminate()
            {
                initialized = false;
            }
        }
    }
}