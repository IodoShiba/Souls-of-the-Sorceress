using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IodoShibaUtil.RigidbodySetVelocity;

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
            public bool isActive = false;
            JumpFields fields = null;
            float limitHeight=0;
            Rigidbody2D rigidbody;
            [SerializeField] GroundSensor groundSensor;
            [SerializeField] UnityEngine.Events.UnityEvent onJumpStart;
            [SerializeField] UnityEngine.Events.UnityEvent onActivatablized;
            [DisabledField] public bool IsActivated;

            public bool Activatable { get => activatable;
                private set
                {
                    if(!activatable && value)
                    {
                        onActivatablized.Invoke();
                    }
                    activatable = value;
                }
            }

            private void Awake()
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }

            private void FixedUpdate()
            {
                if (!isActive || fields == null) { return; }
                if (transform.position.y > limitHeight)
                {
                    Activatable = false;
                    isActive = false;
                    return;
                }

                float forceRequirementProp;
                if (rigidbody.velocity.y > 0)
                {
                    forceRequirementProp = 0;
                    if (Time.deltaTime > 0)
                    {
                        forceRequirementProp = Mathf.Clamp01((limitHeight - transform.position.y) / (rigidbody.velocity.y * Time.deltaTime));
                    }
                }
                else
                {
                    forceRequirementProp = 1;
                }

                float force = forceRequirementProp * rigidbody.mass * (fields.jumpUpSpeed - rigidbody.velocity.y) / Time.deltaTime;

                rigidbody.AddForce(
                    UnityEngine.Mathf.Clamp(
                        force,
                        0, 
                        fields.jumpForce
                        ) * Vector2.up
                        );

            }

            public override void ManualUpdate(in JumpFields fields)
            {
                this.fields = fields;
            }

            public void ManualUpdate(in JumpFields fields,bool isActive)
            {
                if(Activatable && !this.isActive && isActive)
                {
                    limitHeight = fields.jumpHeight + rigidbody.transform.position.y;
                    onJumpStart.Invoke();
                }

                Activatable = groundSensor.IsOnGround && !this.isActive && (Activatable || !isActive);

                IsActivated = !this.isActive && ((Activatable || this.isActive) && isActive);
                this.isActive = (Activatable || this.isActive) && isActive;
                ManualUpdate(fields);
            }

            public void Enable()
            {
                Activatable = true;
            }
        }
    }

    public class JumpMethod : JumpFields.Method
    {
        
    }

    [System.Serializable]
    class Jump : ActorFunction<JumpFields,JumpMethod>
    {
        public void Update(bool isActive) { Method.ManualUpdate(Fields, isActive); }
    }
}