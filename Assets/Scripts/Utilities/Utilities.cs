using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IodoShibaUtil
{
    namespace ManualUpdateClass
    {
        public class ManualClock
        {
            float clock;

            public float Clock { get => clock; }

            public void Update()
            {
                clock += Time.deltaTime;
            }

            public void Reset()
            {
                clock = 0;
            }
        }

        /// <summary>
        /// 他のコンポーネントのUpdateから呼び出し毎フレームの処理を実行するManualUpdate()関数を提供する
        /// </summary>
        public interface IManualUpdate
        {
            void ManualUpdate();
        }

        public interface IManualInitialize
        {
            void ManualInitialize();
        }
    }

    namespace RigidbodySetVelocity
    {
        public static class RigidbodySetVelocityExtension
        {
            public static void SetVelocityX(this Rigidbody2D rigidbody,float velocity,float maxForce)
            {
                float f =
                    Mathf.Clamp(rigidbody.mass * (velocity - rigidbody.velocity.x) / Time.deltaTime,
                    -maxForce,
                    maxForce);
                rigidbody.AddForce(f * Vector2.right);
            }
            public static void SetVelocityY(this Rigidbody2D rigidbody,float velocity,float maxForce)
            {
                float f =
                   Mathf.Clamp(rigidbody.mass * (velocity - rigidbody.velocity.y) / Time.deltaTime,
                   -maxForce,
                   maxForce);
                rigidbody.AddForce(f * Vector2.up);
            }
        }
    }

    namespace Bound2DUtility
    {
        public static class Bound2DUtilityExtension
        {
            public static bool Contains2D(this Bounds bounds,Vector2 point)
            {
                var b = bounds;
                b.extents += new Vector3(0, 0, float.PositiveInfinity);
                return b.Contains((Vector3)point);
            }
            
        }
    }

    namespace Tags
    {
        public static class TagsUtility
        {
            public static readonly string untaggedTag = "Untagged";
            static bool IsSuitableForTag(string tag) => !(string.IsNullOrEmpty(tag) || tag == untaggedTag);
        }
    }
}
