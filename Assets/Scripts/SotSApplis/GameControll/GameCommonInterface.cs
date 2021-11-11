using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SotS
{

    public static class GameCommonInterface
    {
        public static class Player
        {
            static bool stored = false;
            static float playerHealth;
            static float playerMaxHealth;
            static float playerAwakeGauge;
            static int playerProgressLevel;

            public static void Reset()
            {
                stored = false;
                playerMaxHealth = 100;
                playerHealth = playerMaxHealth;
                playerAwakeGauge = 0;
                playerProgressLevel = 0;
            }

            public static void StorePlayerData(global::Player player)
            {
                ExecuteEvents.Execute<SaveData.IPlayerHealthCareer>(player.gameObject, null, 
                    (h, _) => h.Store(
                        null,
                        saves => {
                            playerHealth = saves.health;
                            playerMaxHealth = saves.maxHealth;
                        })
                    );
                ExecuteEvents.Execute<SaveData.IPlayerAwakeCareer>(player.gameObject, null,
                    (h, _) => h.Store(
                        null,
                        saves => {
                            playerAwakeGauge = saves;
                        })
                    );
                ExecuteEvents.Execute<SaveData.IPlayerProgressLevelCareer>(player.gameObject, null,
                    (h, _) => h.Store(
                        null,
                        saves => {
                            playerProgressLevel = saves;
                        })
                    );

                stored = true;
            }

            public static void TryRestorePlayer(global::Player player)
            {
                if(!stored){return;}

                ExecuteEvents.Execute<SaveData.IPlayerHealthCareer>(player.gameObject, null,
                    (h, _) => h.Restore((playerHealth, playerMaxHealth)));
                ExecuteEvents.Execute<SaveData.IPlayerAwakeCareer>(player.gameObject, null,
                    (h, _) => h.Restore(playerAwakeGauge));
                ExecuteEvents.Execute<SaveData.IPlayerProgressLevelCareer>(player.gameObject, null,
                    (h, _) => h.Restore(playerProgressLevel));

                stored = false;
            }

            public static void RecoverHealth(float amount)
            {
                if(ActorManager.PlayerActor?.Mortal is global::Player player)
                {
                    player.RecoverHealth(amount);
                }
                else
                {
                    playerHealth = Mathf.Clamp(playerHealth + amount, 0, playerMaxHealth);
                }
            }
            public static void RecoverHealthComplete()
            {
                if(ActorManager.PlayerActor?.Mortal is global::Player player)
                {
                    player.RecoverHealthComplete();
                }
                else
                {
                    playerHealth = Mathf.Clamp(playerHealth + playerMaxHealth, 0, playerMaxHealth);
                }
            }


        }


        static void ResetStorages()
        {
            Player.Reset();
        }

        public static void ResetEntireGame()
        {
            ResetStorages();
            SaveData.Instance.Init();
            SceneTransitionManager.Initialize();
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.title);
        }


    }
}
