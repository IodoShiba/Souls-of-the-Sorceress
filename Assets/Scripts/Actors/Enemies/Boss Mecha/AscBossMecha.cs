using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using System.Threading;
using System;

public class AscBossMecha : FightActorStateConector
{
    [SerializeField] BossMechaDefault defaultState;
    [SerializeField] SmashedState smashed;
    [SerializeField] BossMechaDead dead;


    public override ActorState DefaultState => defaultState;
    public override SmashedState Smashed => smashed;
    public override DeadState Dead => dead;


    [System.Serializable]
    class BossMechaDefault : DefaultState
    {

    }

    [System.Serializable]
    class BombingTackle : ActorState
    {
        bool ongoing = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
                if(await DroppingBomb(cancellationToken)) { break; }
                if(await Tackle(cancellationToken)) { break; }
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
                if(await DroppingBomb(cancellationToken)) { break; }
                if(await Tackle(cancellationToken)) { break; }
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
    class SettingUpBombs : ActorState
    {
        bool ongoing = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
                if(await DroppingBomb(cancellationToken)) { break; }
                if(await Tackle(cancellationToken)) { break; }
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

    class BossMechaDead : DeadState
    {
        protected override void OnInitialize()
        {
            
        }
    }
}
