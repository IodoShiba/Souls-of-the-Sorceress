using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameLifeCycle : MonoBehaviour
{
    public enum CloseCause { PlayerVictory, GiveUp }
    
    static bool isGameOpen = false;
    static bool isInGameScene = false;

    static UniRx.Subject<UniRx.Unit> subjectOnGameOpen = new UniRx.Subject<UniRx.Unit>();
    static UniRx.Subject<UniRx.Unit> subjectOnGameClose = new UniRx.Subject<UniRx.Unit>();
    static UniRx.Subject<CloseCause> subjectGameClosed = new UniRx.Subject<CloseCause>();

    public static IObservable<UniRx.Unit> observableOnGameOpen { get => subjectOnGameOpen;}
    public static IObservable<UniRx.Unit> observableOnGameClose { get => subjectOnGameClose;}
    public static IObservable<CloseCause> observableGameClosed { get => subjectGameClosed;}

    void Awake()
    {
        StartGameScene();
    }

    void OnDestroy()
    {
        EndGameScene();
    }

    public static void OpenGame()
    {
        if (isGameOpen) {return;}

        isGameOpen = true;
        subjectOnGameOpen.OnNext(new Unit());
    }

    public static void StartGameScene()
    {
        if (isInGameScene){ return; }
        if (!isGameOpen){ OpenGame(); }

        isInGameScene = true;
    }

    public static void EndGameScene()
    {
        if (!isInGameScene){ return; }
        
        isInGameScene = false;
    }

    public static void CloseGame()
    {
        CloseGame(CloseCause.PlayerVictory);
    }

    public static void CloseGame(CloseCause cause)
    {
        if (!isGameOpen){ return; }
        if (isInGameScene){EndGameScene();}

        isGameOpen = false;
        subjectOnGameClose.OnNext(new Unit());
        subjectGameClosed.OnNext(cause);
    }
    
    // for debug purpose
    [SerializeField] bool _isGameOpen = false;
    [SerializeField] bool _isInGameScene = false;
    void Update()
    {
        _isGameOpen = isGameOpen;
        _isInGameScene = isInGameScene;
    }

    public void _CloseGame() 
    {
        CloseGame();
    }
}
