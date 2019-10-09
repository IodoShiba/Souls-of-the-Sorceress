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
            const float lossDisallowedProp = 0;

            bool activatable = false;
            private bool isActive = false;
            JumpFields fields = null;
            float limitHeight=0;
            Rigidbody2D rigidbody;
            [SerializeField] GroundSensor groundSensor;
            [SerializeField] UnityEngine.Events.UnityEvent onJumpStart;
            [SerializeField] UnityEngine.Events.UnityEvent onActivatablized;
            [DisabledField] public bool IsActivated;

            float velocityQuota;

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

            public bool IsActive
            {
                get => isActive;
                set {
                    if (!value)
                    {
                        velocityQuota = float.NegativeInfinity;
                    }
                    isActive = value;
                }
            }

            private void Awake()
            {
                rigidbody = GetComponent<Rigidbody2D>();
                velocityQuota = float.NegativeInfinity;
            }

            private void FixedUpdate()
            {
                if (!IsActive || fields == null) { return; }
                if (transform.position.y > limitHeight || rigidbody.velocity.y < velocityQuota)
                {
                    Activatable = false;
                    IsActive = false;
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
                force = UnityEngine.Mathf.Clamp(
                            force,
                            0,
                            fields.jumpForce
                        );

                velocityQuota = rigidbody.velocity.y;

                rigidbody.AddForce(force * Vector2.up);

            }

            public override void ManualUpdate(in JumpFields fields)
            {
                this.fields = fields;
            }

            public void ManualUpdate(in JumpFields fields,bool isActive)
            {
                if(Activatable && !this.IsActive && isActive)
                {
                    limitHeight = fields.jumpHeight + rigidbody.transform.position.y;
                    onJumpStart.Invoke();
                }

                Activatable = groundSensor.IsOnGround && !this.IsActive && (Activatable || !isActive);

                IsActivated = !this.IsActive && ((Activatable || this.IsActive) && isActive);
                this.IsActive = (Activatable || this.IsActive) && isActive;
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