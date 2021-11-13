using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SotS
{

    public class GameCommonInterface : ScriptableObject
    {
        [System.Serializable]
        public class CategoryPlayer
        {
            bool stored = false;
            float playerHealth;
            float playerMaxHealth;
            float playerAwakeGauge;
            int playerProgressLevel;

            public void Reset()
            {
                stored = false;
                playerMaxHealth = 100;
                playerHealth = playerMaxHealth;
                playerAwakeGauge = 0;
                playerProgressLevel = 0;
            }

            public void StorePlayerData(global::Player player)
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

            public void TryRestorePlayer(global::Player player)
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

            public void RecoverHealth(float amount)
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
            public void RecoverHealthComplete()
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

        static readonly string resourcesPath = "SotsApplis/GameCommonInterface";
        static GameCommonInterface instance;
        public static GameCommonInterface Instance 
        {
            get => instance == null ? (instance = Resources.Load<GameCommonInterface>(resourcesPath)) : instance;
        }
        public CategoryPlayer Player { get => player; set => player = value; }

        [SerializeField] CategoryPlayer player;


        public void ResetGameData()
        {
            player.Reset();
        }

        public void ResetEntireGame()
        {
            ResetGameData();
            SaveData.Instance.Init();
            SceneTransitionManager.Initialize();
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.title);
        }


    }
}
