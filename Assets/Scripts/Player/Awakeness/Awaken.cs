using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    namespace Awakening
    {
        public class Awaken : State
        {
            [SerializeField] InputA inputA;
            [SerializeField] Player player;
            [SerializeField] ActionAwake playerAwake;
            bool initialized = false;

            public override State Check()
            {
                /*
                if (initialized && (inputA.GetButtonShortDownUp("Awake") || player.AwakeGauge <= 0))
                {
                    return GetComponent<Ordinary>();
                }
                */
                if (initialized && !playerAwake.IsActive)
                {
                    return GetComponent<Ordinary>();
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
