using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorFunction
{
    [System.Serializable]
    public class JumpFields : ActorFunctionFields
    {
        [SerializeField] float jumpHeight;
        [SerializeField] float jumpForce;
        [SerializeField] float jumpUpSpeed;

        public class Method : ActorFunctionMethod<JumpFields>
        {
            bool activatable = false;
            bool isActive = false;
            JumpFields fields = null;
            float limitHeight=0;
            Rigidbody2D rigidbody;
            [SerializeField] GroundSensor groundSensor;

            private void Awake()
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }

            private void FixedUpdate()
            {
                if (!isActive || fields == null) { return; }
                if (transform.position.y > limitHeight)
                {
                    activatable = false;
                    isActive = false;
                    return;
                }
                rigidbody.AddForce(
                    UnityEngine.Mathf.Clamp(
                        rigidbody.mass * (fields.jumpUpSpeed - rigidbody.velocity.y) / Time.deltaTime,
                        0, 
                        fields.jumpForce
                        ) * Vector2.up
                        );
            }

            public override void CallableUpdate(in JumpFields fields)
            {
                this.fields = fields;
            }

            public void UpdateCall(in JumpFields fields,bool isActive)
            {
                if(activatable && !this.isActive && isActive)
                {
                    limitHeight = fields.jumpHeight + rigidbody.transform.position.y;
                }

                activatable = groundSensor.IsOnGround && !this.isActive && (activatable || !isActive);
                
                this.isActive = (activatable || this.isActive) && isActive;
                CallableUpdate(fields);
            }

            public void Enable()
            {
                activatable = true;
            }
        }
    }

    public class JumpMethod : JumpFields.Method
    {
        
    }

    [System.Serializable]
    class Jump : ActorFunction<JumpFields,JumpMethod>
    {
        public void Update(bool isActive) { Method.UpdateCall(Fields, isActive); }
    }
}