using UnityEngine;
using UniRx;
using System;

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

            public void Dispose()
            {
                if(disposed) { return; }

                TransScene();

                disposed = true;
            }
        }

        const int initialRemaining = 2;

        static int remaining;
        static string targetSceneName = "stage0";
        static Subject<int> remainingCountListener = new Subject<int>();
        static ReviveSuspensor suspensor = new ReviveSuspensor();

        public static int Remaining { get => remaining; }
        
        public static bool IsRevivable { get => remaining > 0; }
        public static IObservable<int> RemainingCountListener { get => remainingCountListener; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeInitialize()
        {
            remaining = initialRemaining;
        }

        public static void SetTargetSceneName(string sceneName)
        {
            targetSceneName = sceneName;
        }

        public static void Add(int amount)
        {
            if(amount < 0){ throw new System.ArgumentException($"Negative 'amount' is not allowed. given: {amount}"); }

            remainingCountListener.Publish(remaining += amount);
        }

        public static IDisposable GetReviveSuspensor()
        {
            if(!IsRevivable)
            { 
                return null;
            }
            
            --remaining;
            remainingCountListener.OnNext(remaining);
            return suspensor.Reuse(targetSceneName);
        }
        
    }
}