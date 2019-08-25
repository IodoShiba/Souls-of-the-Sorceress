using UnityEngine;
using System.Collections;

namespace ActorFunction
{
    [System.Serializable]
    public class CrossableWithAnotherOnSmashedFields : ActorFunctionFields
    {
        [SerializeField] bool enabled = false;

        public bool Enabled { get => enabled; set => enabled = value; }

        public class Method : ActorFunctionMethod<CrossableWithAnotherOnSmashedFields>
        {
            public override void ManualUpdate(in CrossableWithAnotherOnSmashedFields fields)
            {

            }
        }
    }
    public class CrossableWithAnotherOnSmashedMethod : CrossableWithAnotherOnSmashedFields.Method { }

    [System.Serializable]
    public class CrossableWithAnotherOnSmashed : ActorFunction<CrossableWithAnotherOnSmashedFields, CrossableWithAnotherOnSmashedFields.Method>
    {

    }
}
