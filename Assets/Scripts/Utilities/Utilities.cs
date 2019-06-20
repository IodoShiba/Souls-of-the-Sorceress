using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IodoShiba
{
    namespace Utilities
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
    }
}
