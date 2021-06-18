using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using System.Threading;
using System;

public class AscBossMecha : FightActorStateConector
{
    [SerializeField] Rect fieldRect;

    [SerializeField] BossMechaDefault defaultState;
    [SerializeField] SmashedState smashed;
    [SerializeField] BossMechaDead dead;


    public override ActorState DefaultState => defaultState;
    public override SmashedState Smashed => smashed;
    public override DeadState Dead => dead;

    protected override void BuildStateConnection()
    {
        
    }

    public void DropBomb() 
    {

    }

    public void SetFloatingBomb() 
    {
        
    }

    public void InvokeAllBomb()
    {

    }

    [System.Serializable]
    class BossMechaDefault : DefaultState
    {

    }

    [System.Serializable]
    class BombingTackle : ActorState
    {

        bool ongoing = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        AscBossMecha conector = null; AscBossMecha MechaConnector { get => conector == null ? (conector = Connector as AscBossMecha) : conector; }

        protected override bool ShouldCotinue() => ongoing;
        protected override void OnInitialize()
        {
            ongoing = true; 
            Movement(cancellationTokenSource.Token).Forget();
        }
        protected override void OnTerminate(bool isNormal) 
        {
            if(!isNormal && ongoing)
            {
                cancellationTokenSource.Cancel();
            }
        }
        public override bool IsResistibleTo(Type actorStateType) => typeof(SmashedState).IsAssignableFrom(actorStateType);
        
        async UniTaskVoid Movement(CancellationToken cancellationToken)
        {
            do
            {
                if(cancellationToken.IsCancellationRequested || await DroppingBomb(cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await Tackle(cancellationToken)) { break; }
            } while(false);

            ongoing = false;
        }

        async UniTask<bool> DroppingBomb(CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }
        
        async UniTask<bool> Tackle(CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }
    }

    [System.Serializable]
    class Stamping : ActorState
    {
        bool ongoing = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        AscBossMecha conector = null; AscBossMecha MechaConnector { get => conector == null ? (conector = Connector as AscBossMecha) : conector; }

        protected override bool ShouldCotinue() => ongoing;
        protected override void OnInitialize()
        {
            ongoing = true; 
            Movement(cancellationTokenSource.Token).Forget();
        }
        protected override void OnTerminate(bool isNormal) 
        {
            if(!isNormal && ongoing)
            {
                cancellationTokenSource.Cancel();
            }
        }
        public override bool IsResistibleTo(Type actorStateType) => typeof(SmashedState).IsAssignableFrom(actorStateType);
        
        async UniTaskVoid Movement(CancellationToken cancellationToken)
        {
            do
            {
                if(cancellationToken.IsCancellationRequested || await Util.Aligning(Vector2.zero, Vector2.one, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await Drop(cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await Fade(cancellationToken)) { break; }
            } while(false);

            ongoing = false;
        }
        
        async UniTask<bool> Drop(CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }

        
        async UniTask<bool> Fade(CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }
    }

    [System.Serializable]
    class SettingUpBombs : ActorState
    {
        [SerializeField] float bombSetUpGap;

        bool ongoing = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        AscBossMecha conector = null; AscBossMecha MechaConnector { get => conector == null ? (conector = Connector as AscBossMecha) : conector; }

        protected override bool ShouldCotinue() => ongoing;
        protected override void OnInitialize()
        {
            ongoing = true; 
            Movement(cancellationTokenSource.Token).Forget();
        }
        protected override void OnTerminate(bool isNormal) 
        {
            if(!isNormal && ongoing)
            {
                cancellationTokenSource.Cancel();
            }
        }
        public override bool IsResistibleTo(Type actorStateType) => typeof(SmashedState).IsAssignableFrom(actorStateType);
        
        async UniTaskVoid Movement(CancellationToken cancellationToken)
        {
            Rect fieldRect = MechaConnector.fieldRect;
            Vector2 alignXOrigin = new Vector2(fieldRect.xMin, fieldRect.yMax);
            Vector2 alignXEnd = alignXOrigin + UnityEngine.Vector2.right * fieldRect.width;
            Vector2 alignYOrigin = new Vector2(fieldRect.xMax, fieldRect.yMax);
            Vector2 alignYEnd = alignYOrigin + UnityEngine.Vector2.down * fieldRect.height;

            do
            {
                if(cancellationToken.IsCancellationRequested || await Util.Aligning(alignXOrigin, alignXEnd, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await SetUpBomb(new Vector2(0, -10), fieldRect.height, bombSetUpGap, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await Util.Aligning(alignYOrigin, alignYEnd, cancellationToken)) { break; }
                if(cancellationToken.IsCancellationRequested || await SetUpBomb(new Vector2(-10, 0), fieldRect.width, bombSetUpGap, cancellationToken)) { break; }
            } while(false);

            ongoing = false;
        }

        async UniTask<bool> Following(CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }
        
        async UniTask<bool> SetUpBomb(Vector2 distance, float setRange, float settingGap, CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }
    }

    static class Util
    {
        public static async UniTask<bool> Aligning(Vector2 origin, Vector2 end, CancellationToken cancellationToken)
        {
            while(false /* cond */)
            {
                if(cancellationToken.IsCancellationRequested){ return true; }
                
                // process

                await UniTask.Yield();
            }
            return false;
        }
    }

    class BossMechaDead : DeadState
    {
        protected override void OnInitialize()
        {
            
        }
    }
}
