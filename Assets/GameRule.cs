using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRule : MonoBehaviour
{
    public enum GameStateEnum
    {
        None,
        OnGame,
        Victory,
        Defeated
    }

    static GameRule instance;
    public static GameRule Instance { get => instance; }

    [SerializeField] UnityEngine.Events.UnityEvent onVictory;
    [SerializeField] UnityEngine.Events.UnityEvent onDefeated;

    GameStateEnum gameState;
    public GameStateEnum GameState { get => gameState;
        set
        {
            switch(value)
            {
                case GameStateEnum.Victory:
                    onVictory.Invoke();
                    break;

                case GameStateEnum.Defeated:
                    onDefeated.Invoke();
                    break;
            }
            gameState = value;
        }
    }

    ActorSarah.ActorStateConnectorSarah ascSarah;
    ActorSarah.ActorStateConnectorSarah AscSarah { get => ascSarah == null ? (ascSarah = ActorManager.PlayerActor.GetComponent<ActorSarah.ActorStateConnectorSarah>()) : ascSarah; }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        GameState = GameStateEnum.OnGame;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameState) {
            case GameStateEnum.OnGame:
                if (AscSarah.IsDead)
                {
                    GameState = GameStateEnum.Defeated;
                }
                break;
        }
    }

    public void SetVictory()
    {
        GameState = GameStateEnum.Victory;
    }
    public void SetDefeated()
    {
        GameState = GameStateEnum.Defeated;
    }

    public void GoGameOver()
    {
        float orthoCamSize = Camera.main.orthographicSize;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(ActorManager.PlayerActor.transform.position);
        int playerDir = ActorManager.PlayerActor.GetComponent<ActorFunction.Directionable>().CurrentDirectionInt;
        GetComponent<SceneChanger>().ChangeScene("GameOver",
            s =>
            {
                GameObject[] roots = s.GetRootGameObjects();
                for(int i = 0; i < roots.Length; ++i)
                {
                    GameOverScene gameOverScene = roots[i].GetComponent<GameOverScene>();
                    if (gameOverScene != null)
                    {
                        gameOverScene.SetInitializeData(orthoCamSize, playerScreenPoint,playerDir);
                    }
                }
            }
            );
    }
}
