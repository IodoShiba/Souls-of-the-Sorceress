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
                if (initialized && (inputA.GetButtonShortDownUp("Awake") || player.AwakeGauge <= 0))
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
