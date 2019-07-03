using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static System.Math;
using static UnityEngine.Mathf;

namespace ActorFunction
{
    [System.Serializable]
    public class HorizontalMoveField : ActorFunctionFields
    {
        [SerializeField] float maxSpeed;
        [SerializeField] float pushForceMagnitude;
        [SerializeField] float stopForceMagnitude;
        [SerializeField] AnimationCurve curve;

        public class Method : ActorFunctionMethod<HorizontalMoveField>
        {
            float argSpeedMultiplier;
            HorizontalMoveField fields;
            Rigidbody2D rigidbody;
            float stopTime;
            private void Awake()
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }
            private void FixedUpdate()
            {
                float goalSpeed = argSpeedMultiplier != 0 && fields != null ? argSpeedMultiplier * fields.maxSpeed : 0;
                if (fields == null) { return; }
                //if(argSpeedMultiplier == 0) { return; }
                float maxForce = argSpeedMultiplier == 0 ? fields.stopForceMagnitude : fields.pushForceMagnitude;

                float f = 
                    Clamp(rigidbody.mass * (argSpeedMultiplier * fields.maxSpeed - rigidbody.velocity.x) / Time.deltaTime,
                    -maxForce, 
                    maxForce);
                rigidbody.AddForce(f * Vector2.right);

                if (stopTime > 0)
                {
                    stopTime -= Time.deltaTime;
                    enabled = stopTime > 0;
                }
            }
            public override void ManualUpdate(in HorizontalMoveField fields) { }

            public void ManualUpdate(in HorizontalMoveField fields,float speedMultiplier)
            {
                argSpeedMultiplier = Max(-1, Min(speedMultiplier,1)); ;
                this.fields = fields;
                ManualUpdate(fields);
            }

            public void StopActorOnDisabled(float time)
            {
                argSpeedMultiplier = 0;
                if (!enabled)
                {
                    stopTime = time;
                    enabled = true;
                }

            }

        }
    }

    public class HorizontalMoveMethod : HorizontalMoveField.Method { }

    [System.Serializable]
    public class HorizontalMove : ActorFunction<HorizontalMoveField, HorizontalMoveField.Method> {
        public void ManualUpdate(float speedMultiplier) { Method.ManualUpdate(Fields,speedMultiplier); }
    }
}
