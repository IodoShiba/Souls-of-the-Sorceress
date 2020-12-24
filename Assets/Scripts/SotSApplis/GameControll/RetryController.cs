using UnityEngine;

namespace SotS
{
    public class RetryController
    {
        const int initialRemaining = 2;

        static int remaining;

        public static int Remaining { get => remaining; }
        
        public static bool IsRetriable { get => remaining > 0; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeInitialize()
        {
            remaining = initialRemaining;
        }

        static void Add(int amount)
        {
            if(amount < 0){ throw new System.ArgumentException($"Negative 'amount' is not allowed. given: {amount}"); }

            remaining += amount;
        }

        static bool TryConsume()
        {
            bool isSuccess;
            if(isSuccess = IsRetriable);
            {
                --remaining;
            }
            return isSuccess;
        }
    }
}