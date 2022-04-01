using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameLifeCycle : MonoBehaviour
{
    static bool isGameOpen = false;
    static bool isInGameScene = false;

    static UniRx.Subject<UniRx.Unit> subjectOnGameOpen = new UniRx.Subject<UniRx.Unit>();
    static UniRx.Subject<UniRx.Unit> subjectOnGameClose = new UniRx.Subject<UniRx.Unit>();
    static UniRx.Subject<UniRx.Unit> subjectGameClosed = new UniRx.Subject<UniRx.Unit>();

    public static IObservable<UniRx.Unit> observableOnGameOpen { get => subjectOnGameOpen;}
    public static IObservable<UniRx.Unit> observableOnGameClose { get => subjectOnGameClose;}
    public static IObservable<UniRx.Unit> observableGameClosed { get => subjectOnGameClose;}

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
        if (!isGameOpen){ return; }
        if (isInGameScene){EndGameScene();}

        isGameOpen = false;
        subjectOnGameClose.OnNext(new Unit());
        subjectGameClosed.OnNext(new Unit());
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
