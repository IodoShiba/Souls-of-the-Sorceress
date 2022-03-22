using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

static class GameLifeCycle
{
    static bool isGameOpen = false;
    static bool isInGameScene = false;

    static UniRx.Subject<UniRx.Unit> subjectOnGameOpen = new UniRx.Subject<UniRx.Unit>();
    static UniRx.Subject<UniRx.Unit> subjectOnGameClose = new UniRx.Subject<UniRx.Unit>();

    public static IObservable<UniRx.Unit> observableOnGameOpen { get => subjectOnGameOpen;}
    public static IObservable<UniRx.Unit> observableOnGameClose { get => subjectOnGameClose;}

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

        isGameOpen = false;
        subjectOnGameClose.OnNext(new Unit());
    }
}
