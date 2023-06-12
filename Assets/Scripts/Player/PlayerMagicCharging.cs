using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerMagicCharging : HorizontalDirectionDependentState
    {
        [SerializeField] AwakeMutableObject weakMagic;
        [SerializeField] AwakeMutableObject strongMagic;
        [SerializeField] float strongMagicThresholdTime;
        [SerializeField] float magicInitialPositionXShift;
        [SerializeField] float _magicFlySpeed;

        float chargedTime=0;

        public override State Check()
        {
            if (!InputDaemon.IsPressed("Magical Attack"))
            {
                GameObject magicPrefab;
                if (chargedTime > strongMagicThresholdTime)
                {
                    magicPrefab = strongMagic.GetObject();
                }
                else
                {
                    magicPrefab = weakMagic.GetObject();
                }
                var magicInstance = Instantiate(magicPrefab, transform.position + new Vector3(dirSign * magicInitialPositionXShift, 0, 0), Quaternion.identity);
                magicInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(dirSign * _magicFlySpeed, 0);
                return GetComponent<PlayerStates.EndOfAction>();
            }
            return null;
        }

        public override void Initialize()
        {
            chargedTime = 0;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        public override void Execute()
        {
            chargedTime += Time.deltaTime;
        }

        public override void Terminate()
        {

            chargedTime = 0;
        }
        
    }
}
