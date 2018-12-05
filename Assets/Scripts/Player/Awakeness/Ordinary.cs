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
            bool initialized = false;
            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public override State Check()
            {
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