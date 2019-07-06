using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerTackle : State
    {
        [SerializeField] float speed;
        [SerializeField] float proceedDistance;
        [SerializeField] float unguardTime;
        [SerializeField] float _motionLength;
        [SerializeField] Rigidbody2D playerRb;
        [SerializeField] Sensor wallSensor;
        private int dirSign = 0;
        //private float t=0;
        private Vector2 v;
        private bool initialized = false;
        private float borderXmin = 0;
        private float borderXmax = 0;
        private float t = 0;
        private bool complete = false;
        

        public override State Check()
        {
            if (initialized)
            {
                if (complete) {
                    complete = false;
                    StopCoroutine(ExecuteProceed());
                    return GetComponent<PlayerStates.PlayerOnGround>();
                }
            }
            return null;
        }

        public override void Initialize()
        {
            borderXmin = transform.position.x - proceedDistance;
            borderXmax = transform.position.x + proceedDistance;
            t = 0;
            v = new Vector2(dirSign * speed, 0);
            //wallSensor.Reset();
            initialized = true;
            complete = false;
            StartCoroutine(ExecuteProceed());
        }

        public override void Execute()
        {
        }

        private IEnumerator ExecuteProceed()
        {
            while (true) { 
                float x = transform.position.x;
                if (/*t > _motionLength*/(x <= borderXmin || borderXmax <= x) || wallSensor.IsDetecting)
                {
                    playerRb.velocity = Vector2.zero;
                    break;
                    //return GetComponent<PlayerStates.PlayerGuard>();
                }
                playerRb.velocity = v;
                yield return null;
            }
            playerRb.velocity = Vector2.zero;
            yield return StartCoroutine(ExecuteUnguard());
        }
        private IEnumerator ExecuteUnguard()
        {
            t = 0;
            while (true)
            {
                t += Time.deltaTime;
                if (t >= unguardTime)
                {
                    complete = true;
                    break;
                }
                yield return null;
            }
            yield break;
        }

        public override void Terminate()
        {
            wallSensor.Reset();
            //playerRb.velocity = new Vector2(0, 0);
            borderXmin = borderXmax = 0;
            t = 0;
            initialized = false;
            complete = false;
        }

        public void SetDirection(int sign)
        {
            dirSign = sign;
        }
        
        /*private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.transform.tag == "Ground")
            {
                expired = true;
            }
        }*/
    }
}

public class Tackle : ArtsAbility
{
    [SerializeField] float speed;
    [SerializeField] float proceedDistance;
    [SerializeField] float unguardTime;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Sensor wallSensor;
    private int dirSign = 0;
    //private float t=0;
    private Vector2 v;
    private float borderXmin = 0;
    private float borderXmax = 0;
    private float t = 0;
    private bool complete = false;
    private float processTime;
    IEnumerator cof;
    
    protected override bool ShouldContinue(bool ordered)
    {
        return !complete;
    }
    protected override void OnInitialize()
    {
        borderXmin = transform.position.x - proceedDistance;
        borderXmax = transform.position.x + proceedDistance;
        t = 0;
        dirSign = System.Math.Sign(playerRb.transform.localScale.x);
        v = Vector2.right * dirSign * speed;
        complete = false;
        StartCoroutine(cof=ExecuteProceed());
    }
    private IEnumerator ExecuteProceed()
    {
        while (true)
        {
            float x = transform.position.x;
            if ((x <= borderXmin || borderXmax <= x) || wallSensor.IsDetecting)
            {
                playerRb.velocity = Vector2.zero;
                break;
            }
            playerRb.velocity = v;
            yield return null;
        }
        playerRb.velocity = Vector2.zero;
        yield return StartCoroutine(ExecuteUnguard());
    }
    private IEnumerator ExecuteUnguard()
    {
        t = 0;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= unguardTime)
            {
                complete = true;
                break;
            }
            yield return null;
        }
        yield break;
    }
    protected override void OnTerminate()
    {
        StopCoroutine(cof);
        complete = false;
    }
}
