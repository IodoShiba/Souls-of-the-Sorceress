using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using DG.Tweening;
using System.Threading;
using System;

public class AscBossMecha : FightActorStateConector
{
    [SerializeField] Grid fieldGrid;
    [SerializeField] RectInt fieldRange;

    [SerializeField] BossMechaDefault defaultState;
    [SerializeField] BombingTackle bombingTackle;
    [SerializeField] Stamping stamping;
    [SerializeField] SettingUpBombs settingUpBombs;
    [SerializeField] SmashedState smashed;
    [SerializeField] BossMechaDead dead;

    ActorState[] ActionOrder => new ActorState[]{bombingTackle, stamping, settingUpBombs};


    public override ActorState DefaultState => defaultState;
    public override SmashedState Smashed => smashed;
    public override DeadState Dead => dead;

    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken GetCancellationToken() => cancellationTokenSource.Token;
    void CancelAction() => cancellationTokenSource.Cancel();

    Rect fieldRect;

    ActorState nextAction = null;

    void OnDestroy()
    {
        cancellationTokenSource.Dispose(); // CancellationTokenSource must be Disposed when you no longer use it.
    }

    protected override void BuildStateConnection()
    {
        ConnectStateFromDefault(
            ()=>{
                var n = nextAction; 
                nextAction = null;
                return n;
                }
                );
        Initialize();
    }

    private void Initialize()
    {
        Vector2 fieldOrigin = fieldGrid.transform.position;
        fieldRect = 
            new Rect(
                fieldOrigin + Vector2.Scale(fieldGrid.cellSize, fieldRange.position), 
                Vector2.Scale(fieldGrid.cellSize, fieldRange.size)
                );

        ((BossMechaDefault)DefaultState).SetActionOrder(ActionOrder);
    }

    public void DropBomb() 
    {

    }

    public void SetFloatingBomb() 
    {
        
    }

    public void IgniteAllBomb()
    {

    }

    void SetNextAction(ActorState actionState)
    {
        nextAction = actionState;
    }

    [System.Serializable]
    class BossMechaDefault : DefaultState
    {
        ActorState[] actionOrder;
        AscBossMecha ascBossMecha;
        AscBossMecha AscBossMecha {get => ascBossMecha==null?(ascBossMecha = (AscBossMecha)Connector) : ascBossMecha;}
        int next = 0;
        
        public void SetActionOrder(ActorState[] order)
        {
            actionOrder = order;
            next = 0;
        }

        protected override void OnInitialize()
        {
            ActorState nextAction = getNextAction();
            AscBossMecha.SetNextAction(nextAction);
        }

        public ActorState getNextAction()
        {
            ActorState nextAction = actionOrder[next];
            next = (next+1)%actionOrder.Length;
            return nextAction;
        }
    }

    [System.Serializable]
    class BombingTackle : ActorState
    {
        [SerializeField] float intervalTime;
        [SerializeField] float endingTime;
        [SerializeField] float bombingMoveSpeed;
        [SerializeField] float bombingMoveCeilGap;
        [SerializeField] float tackleFloorGap;
        [SerializeField] float tackleSpeed;
        [SerializeField] float HorizontalRangeMinShift;
        [SerializeField] float HorizontalRangeMaxShift;

        AscBossMecha ascBossMecha;
        AscBossMecha AscBossMecha {get => ascBossMecha==null?(ascBossMecha = (AscBossMecha)Connector) : ascBossMecha;}
 
        bool ongoing = false;

        protected override bool ShouldCotinue() => ongoing;
        protected override void OnInitialize()
        {
            ongoing = true; 
            Movement(AscBossMecha.GetCancellationToken()).Forget();
        }
        protected override void OnTerminate(bool isNormal) 
        {
            if(!isNormal && ongoing)
            {
                AscBossMecha.CancelAction();
            }
        }
        public override bool IsResistibleTo(Type actorStateType) => typeof(SmashedState).IsAssignableFrom(actorStateType);
        
        async UniTaskVoid Movement(CancellationToken cancellationToken)
        {
            do
            {
                if(cancellationToken.IsCancellationRequested || await DroppingBomb(cancellationToken)) { break; }
                await UniTask.Delay(TimeSpan.FromSeconds(intervalTime), false, PlayerLoopTiming.Update, cancellationToken);
                if(cancellationToken.IsCancellationRequested || await Tackle(cancellationToken)) { break; }
                await UniTask.Delay(TimeSpan.FromSeconds(endingTime), false, PlayerLoopTiming.Update, cancellationToken);
            } while(false);

            ongoing = false;
        }

        async UniTask<bool> DroppingBomb(CancellationToken cancellationToken)
        {
            float bombingMoveHeight = AscBossMecha.fieldRect.yMax + bombingMoveCeilGap;
            float beginx = AscBossMecha.fieldRect.xMin + HorizontalRangeMinShift;
            float endx = AscBossMecha.fieldRect.xMax + HorizontalRangeMaxShift;
            float time = Mathf.Abs(endx-beginx) / bombingMoveSpeed;
            float t=0;
            float z=GameObject.transform.position.z;

            GameObject.transform.position = new Vector3(beginx, bombingMoveHeight, z);
            while(t<time)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                Vector3 pos = GameObject.transform.position;
                GameObject.transform.position = new Vector3(Mathf.Lerp(beginx, endx, t/time), bombingMoveHeight, z);

                t += Time.deltaTime;

                await UniTask.Yield();
            }
            return false;
        }
        
        async UniTask<bool> Tackle(CancellationToken cancellationToken)
        {
            float tackleHeight = AscBossMecha.fieldRect.yMin + tackleFloorGap;
            float beginx = AscBossMecha.fieldRect.xMax + HorizontalRangeMaxShift;
            float endx = AscBossMecha.fieldRect.xMin + HorizontalRangeMinShift;
            float time = Mathf.Abs(endx-beginx) / tackleSpeed;
            float t=0;
            float z=GameObject.transform.position.z;

            GameObject.transform.position = new Vector3(beginx, tackleHeight, z);
            while(t<time)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                Vector3 pos = GameObject.transform.position;
                GameObject.transform.position = new Vector3(Mathf.Lerp(beginx, endx, t/time), tackleHeight, z);

                t += Time.deltaTime;

                await UniTask.Yield();
            }
            return false;
        }
    }

    [System.Serializable]
    class Stamping : ActorState
    {
        bool ongoing = false;
        AscBossMecha ascBossMecha = null; AscBossMecha MechaConnector { get => ascBossMecha == null ? (ascBossMecha = Connector as AscBossMecha) : ascBossMecha; }


        [SerializeField] float fadeInCeilShift;
        [SerializeField] Util.AligningArgs aligningArgs;
        [SerializeField] float jumpHeight;
        [SerializeField] float jumpTime;
        [SerializeField] float dropAccel;
        [SerializeField] float dropLockTime;
        [SerializeField] float groundFloorGap;

        protected override bool ShouldCotinue() => ongoing;
        protected override void OnInitialize()
        {
            ongoing = true; 
            Movement(MechaConnector.GetCancellationToken()).Forget();
        }
        protected override void OnTerminate(bool isNormal) 
        {
            if(!isNormal && ongoing)
            {
                MechaConnector.CancelAction();
            }
        }
        public override bool IsResistibleTo(Type actorStateType) => typeof(SmashedState).IsAssignableFrom(actorStateType);
        
        async UniTaskVoid Movement(CancellationToken cancellationToken)
        {
            do
            {
                Util.AligningArgs args = aligningArgs;
                args.aligningPos = fadeInCeilShift + MechaConnector.fieldRect.yMax;

                if(cancellationToken.IsCancellationRequested || await Util.AligningX(GameObject, ActorManager.PlayerActor.gameObject, args, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await Drop(cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await Fade(cancellationToken)) { break; }
            } while(false);

            ongoing = false;
        }
        
        async UniTask<bool> Drop(CancellationToken cancellationToken)
        {

            Sequence seq = DOTween.Sequence();
            seq.Append(GameObject.transform.DOMoveY(GameObject.transform.position.y + jumpHeight, jumpTime).SetEase(Ease.OutQuad));
            seq.Play();
            while(seq.IsPlaying())
            {
                if(cancellationToken.IsCancellationRequested){ seq.Kill(); return true; }
                // process
                await UniTask.Yield();
            }
            seq.Kill();

            float groundY = MechaConnector.fieldRect.yMin + groundFloorGap;
            float y0 = GameObject.transform.position.y;
            float t=0;
            while(true)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                float nexty = y0 - 0.5f*dropAccel*t*t;
                t += Time.deltaTime;

                if(nexty < groundY)
                {
                    GameObject.transform.position = new Vector3(GameObject.transform.position.x, groundY, GameObject.transform.position.z);
                    break;
                }
                else
                {
                    GameObject.transform.position = new Vector3(GameObject.transform.position.x, nexty, GameObject.transform.position.z);
                }
                await UniTask.Yield();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(dropLockTime), false, PlayerLoopTiming.Update, cancellationToken);
            return false;
        }

        
        async UniTask<bool> Fade(CancellationToken cancellationToken)
        {
            float t=0;
            Sequence seq = DOTween.Sequence();
            seq.Append(GameObject.transform.DOMoveY(GameObject.transform.position.y - 5, jumpTime).SetEase(Ease.Linear));
            seq.Play();
            while(seq.IsPlaying())
            {
                if(cancellationToken.IsCancellationRequested){ seq.Kill(); return true; }
                // process
                t += Time.deltaTime;
                await UniTask.Yield();
            }
            seq.Kill();
            return false;
        }
    }

    [System.Serializable]
    class SettingUpBombs : ActorState
    {
        [SerializeField] Util.AligningArgs aligningArgs;
        [SerializeField] float fadeInCeilShift;
        [SerializeField] float xAlignRangeMin; 
        [SerializeField] float xAlignRangeMax;
        [SerializeField] float fadeInRightWallShift;
        [SerializeField] float yAlignRangeMin; 
        [SerializeField] float yAlignRangeMax;
        [SerializeField] float jumpHeight;
        [SerializeField] float jumpTime;
        [SerializeField] float bombSetUpSpeed;
        [SerializeField] float bombSetUpRangeX;
        [SerializeField] float bombSetUpRangeY;
        [SerializeField] float bombSetUpGap;

        bool ongoing = false;
        AscBossMecha conector = null; AscBossMecha MechaConnector { get => conector == null ? (conector = Connector as AscBossMecha) : conector; }

        protected override bool ShouldCotinue() => ongoing;
        protected override void OnInitialize()
        {
            ongoing = true; 
            Movement(MechaConnector.GetCancellationToken()).Forget();
        }
        protected override void OnTerminate(bool isNormal) 
        {
            if(!isNormal && ongoing)
            {
                MechaConnector.CancelAction();
            }
        }
        public override bool IsResistibleTo(Type actorStateType) => typeof(SmashedState).IsAssignableFrom(actorStateType);
        
        async UniTaskVoid Movement(CancellationToken cancellationToken)
        {
            Rect fieldRect = MechaConnector.fieldRect;

            do
            {
                aligningArgs.rangeMin = xAlignRangeMin; 
                aligningArgs.rangeMax = xAlignRangeMax;
                aligningArgs.aligningPos = fadeInCeilShift + MechaConnector.fieldRect.yMax;
                if(cancellationToken.IsCancellationRequested || await Util.AligningX(GameObject, ActorManager.PlayerActor.gameObject, aligningArgs, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await SetUpBomb(Vector2.down*bombSetUpRangeY, 4, cancellationToken)) { break; }
                aligningArgs.rangeMin = yAlignRangeMin; 
                aligningArgs.rangeMax = yAlignRangeMax;
                aligningArgs.aligningPos = fadeInRightWallShift + MechaConnector.fieldRect.xMax;
                if(cancellationToken.IsCancellationRequested || await Util.AligningY(GameObject, ActorManager.PlayerActor.gameObject, aligningArgs, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await SetUpBomb(Vector2.left*bombSetUpRangeX, 4, cancellationToken)) { break; }
            } while(false);

            ongoing = false;
        }

        async UniTask<bool> SetUpBomb(Vector2 range, float extraMoveLength, CancellationToken cancellationToken)
        {
            Vector2 origin = GameObject.transform.position;

            Sequence seq = DOTween.Sequence();
            seq.Append(GameObject.transform.DOMove(GameObject.transform.position - (Vector3)range.normalized * jumpHeight, jumpTime).SetEase(Ease.OutQuad));
            seq.Append(GameObject.transform.DOMove(GameObject.transform.position, jumpTime).SetEase(Ease.InQuad));
            
            while(seq.IsPlaying())
            {
                if(cancellationToken.IsCancellationRequested){ seq.Kill(); return true; }
                
                await UniTask.Yield();
            }

            float rangeLength = range.magnitude;
            Vector2 direction = range/rangeLength;
            float d = 0;
            while(d < rangeLength+extraMoveLength)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                float nextd = d+bombSetUpSpeed*Time.deltaTime;


                d = nextd;
                GameObject.transform.position = origin+direction*d;
                await UniTask.Yield();
            }
            return false;
        }
    }

    static class Util
    {
        [System.Serializable]
        public struct AligningArgs
        { 
            public float rangeMin; 
            public float rangeMax;
            public float aligningPos;
            public float appearTime;
            public float alignTime;
            public float alignSpeed;
            public float approximationGap;
        }

        public static async UniTask<bool> AligningX(GameObject obj, GameObject target, AligningArgs args, CancellationToken cancellationToken)
        {
            obj.transform.position = new Vector3((args.rangeMin + args.rangeMax)/2, args.aligningPos + 5, obj.transform.position.z);
            float t = 0;
            while(t < args.appearTime)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                obj.transform.position = new Vector3(obj.transform.position.x, Mathf.Lerp(5, 0, t/args.appearTime)+args.aligningPos, obj.transform.position.z);
                
                t += Time.deltaTime;

                await UniTask.Yield();
            }

            t = 0;
            while(t < args.alignTime)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                float targetx = target.transform.position.x;
                float newx = obj.transform.position.x;
                if(Mathf.Abs(newx-targetx)>args.approximationGap)
                {
                    newx = Mathf.Clamp(newx + Mathf.Sign(targetx-newx)*args.alignSpeed*Time.deltaTime, args.rangeMin, args.rangeMax);
                }
                obj.transform.position = new Vector3(newx, args.aligningPos, obj.transform.position.z);
                
                t += Time.deltaTime;

                await UniTask.Yield();
            }

            return false;


        }
        
        public static async UniTask<bool> AligningY(GameObject obj, GameObject target, AligningArgs args, CancellationToken cancellationToken)
        {
            obj.transform.position = new Vector3(args.aligningPos + 5, (args.rangeMin + args.rangeMax)/2, obj.transform.position.z);
            float t = 0;
            while(t < args.appearTime)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                obj.transform.position = new Vector3(args.aligningPos + Mathf.Lerp(5, 0, t/args.appearTime), obj.transform.position.y, obj.transform.position.z);
                
                t += Time.deltaTime;

                await UniTask.Yield();
            }

            t = 0;
            while(t < args.alignTime)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                float targety = target.transform.position.y;
                float newy = obj.transform.position.y;
                if(Mathf.Abs(newy-targety)>args.approximationGap)
                {
                    newy = Mathf.Clamp(newy + Mathf.Sign(targety-newy)*args.alignSpeed*Time.deltaTime, args.rangeMin, args.rangeMax);
                }
                obj.transform.position = new Vector3(args.aligningPos, newy, obj.transform.position.z);
                
                t += Time.deltaTime;

                await UniTask.Yield();
            }

            return false;


        }
    }

    class BossMechaDead : DeadState
    {
        AscBossMecha conector = null; AscBossMecha MechaConnector { get => conector == null ? (conector = Connector as AscBossMecha) : conector; }

        protected override void OnInitialize()
        {
        }
    }
}
