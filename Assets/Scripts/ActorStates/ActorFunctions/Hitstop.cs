using UnityEngine;
using System.Collections;

namespace ActorFunction
{
    public class Hitstop : MonoBehaviour
    {
        float hitstopSpan = 0;

        System.Action<bool> onActiveStateChanged;

        Vector2 rb2dVel;
        float rb2dAngvel;
        [SerializeField] Animator animator;

        RigidbodyConstraints2D defaultConstraints;

        bool isInHitstop = false;
        public bool IsInHitstop
        {
            get => isInHitstop;
            private set
            {
                if(isInHitstop != value)
                {
                    onActiveStateChanged.Invoke(value);
                }
                isInHitstop = value;
            }
        }

        float HitstopSpan
        {
            get => hitstopSpan;
            set
            {
                hitstopSpan = value;
                IsInHitstop = hitstopSpan > 0;
            }
        }

        private void Awake()
        {
            GetComponent<Mortal>().OnHitstopGiven.AddListener(AddTime);

            onActiveStateChanged = _ => { };

            if (animator == null) { animator = GetComponent<Animator>(); }

            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                onActiveStateChanged +=
                    isA =>
                    {
                        if (isA)
                        {
                            rb2dVel = rb2d.velocity;
                            rb2dAngvel = rb2d.angularVelocity;
                            defaultConstraints = rb2d.constraints;
                            rb2d.constraints = defaultConstraints | RigidbodyConstraints2D.FreezePosition;
                        }
                        else
                        {
                            rb2d.velocity = rb2dVel;
                            rb2d.angularVelocity = rb2dAngvel;
                            rb2d.constraints = defaultConstraints;
                        }
                        //rb2d.simulated = !isA;
                    };
            }
            if (animator != null)
            {
                onActiveStateChanged += ControlAnimator;
            }

            isInHitstop = false;
        }

        public void AddTime(float hitstopSpan)
        {
            this.HitstopSpan += hitstopSpan;
        }

        private void Update()
        {
            if(!IsInHitstop)
            {
                HitstopSpan = 0;
                return;
            }

            HitstopSpan -= Time.deltaTime;
        }

        public void ControlAnimator(bool isInHitstop)
        {
            //ヒットストップが起きた/終わった時にアニメーションを何かする
            animator.speed = isInHitstop ? 0 : 1;
        }
    }
}
