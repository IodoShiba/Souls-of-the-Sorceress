using UnityEngine;
using UniRx;
using System;
using UnityEngine.AddressableAssets;

namespace SotS
{
    public static class ReviveController
    {
        class ReviveSuspensor : IDisposable
        {
            string targetSceneName;
            bool disposed = false;

            public ReviveSuspensor Reuse(string targetSceneName)
            {
                this.targetSceneName = targetSceneName;
                disposed = false;
                return this;
            }

            void TransScene()
            {
                SceneTransitionManager.TransScene(targetSceneName, null);
            }

            void Revive()
            {
                TransScene();
            }

            public void Dispose()
            {
                if(disposed) { return; }

                Revive();

                disposed = true;
            }
        }

        const int initialRemaining = 2;

        static int remaining;
        static int continueCount = 0;
        static string targetSceneName;
        static Subject<int> remainingCountListener = new Subject<int>();
        static ReviveSuspensor suspensor = new ReviveSuspensor();
        static SoundCollection systemSoundCollection;

        public static int Remaining { get => remaining; }
        
        public static bool IsRevivable { get => remaining > 0; }
        public static IObservable<int> RemainingCountListener { get => remainingCountListener; }
        public static int ContinueCount { get => continueCount; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeInitialize()
        {
            Reset();
            // Addressables.LoadAssetAsync<SoundCollection>(AddressableAddresses.soundCollections + "SystemSounds.asset")
            //    .Completed += (op) => {systemSoundCollection = op.Result;};
            systemSoundCollection=Resources.Load<SoundCollection>("SoundCollections/SystemSounds");
            GameLifeCycle.observableOnGameClose.Subscribe(_ => continueCount = 0);
        }

        public static void SetTargetSceneName(string sceneName)
        {
            targetSceneName = sceneName;
        }

        public static void AddRemainingCount(int amount)
        {
            if(amount < 0){ throw new System.ArgumentException($"Negative 'amount' is not allowed. given: {amount}"); }

            remainingCountListener.OnNext(remaining += amount);
            SoundManager.Instance.PlaySE(systemSoundCollection["1Up"]);
        }

        public static IDisposable GetReviveSuspensor()
        {
            if(!IsRevivable)
            { 
                return null;
            }
            
            --remaining;
            ++continueCount;
            remainingCountListener.OnNext(remaining);
            return suspensor.Reuse(targetSceneName);
        }
        
        public static void Reset()
        {
            remaining = initialRemaining;
        }
    }
}