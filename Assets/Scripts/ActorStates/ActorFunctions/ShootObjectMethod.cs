using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorFunction
{
    [System.Serializable]
    public class ShootObjectFields : ActorFunctionFields
    {
        [SerializeField] Vector2 initialVelocity;
        [SerializeField] Vector2 relativePosition;
        [SerializeField] private Rigidbody2D target;

        public Vector2 InitialVelocity { get => initialVelocity; set => initialVelocity = value; }
        public Vector2 RelativePosition { get => relativePosition; set => relativePosition = value; }


        public class Method : ActorFunctionMethod<ShootObjectFields>
        {
            ShootObjectFields fields;

            public override void ManualUpdate(in ShootObjectFields fields)
            {
                this.fields = fields;
            }

            // Start is called before the first frame update

            public void Use()
            {
                Instantiate(fields.target, gameObject.transform.position + (Vector3)fields.relativePosition, Quaternion.identity).velocity = fields.initialVelocity;
            }
        }
    }

    public class ShootObjectMethod : ShootObjectFields.Method { }

    [System.Serializable]
    public class ShootObject : ActorFunction<ShootObjectFields, ShootObjectMethod> { }
}