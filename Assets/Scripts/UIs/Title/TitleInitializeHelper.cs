using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SotS.UI;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Analytics;

public class TitleInitializeHelper : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIPager _pager;

    private Dictionary<string, UIPagerPage> _pages = new Dictionary<string, UIPagerPage>();
    private List<UIPagerPage> pageStack = new List<UIPagerPage>(8);
    private IReadOnlyList<UIPagerPage> pageStackReadOnlyList = null;

    private bool started = false;
    
    public void InitializeScene(string[] stackedPageNames)
    {
        AsyncInitializeScene(stackedPageNames).Forget(e=>{ Debug.LogError(e); });
    }

    void Start()
    {
        AsyncStart().Forget(e => { Debug.LogError(e); });
    }

    async UniTask<Unit> AsyncStart()
    {
        started = true;

        return Unit.Default;
    }

    async UniTask<Unit> AsyncInitializeScene(string[] stackedPageNames)
    {
        FindUIPagerPages();
        await UniTask.WaitWhile(() => !started, PlayerLoopTiming.PreLateUpdate);
        StackPages(stackedPageNames);
        
        return Unit.Default;
    }
    
    void FindUIPagerPages()
    {
        var pagesArray = _canvas.GetComponentsInChildren<UIPagerPage>(true);

        for (int i = 0; i < pagesArray.Length; ++i)
        {
            var page = pagesArray[i];
            _pages.Add(page.name, page);
        }
    }

    void StackPages(string[] pageNames)
    {
        pageStack.Clear();
        for (int i = 0; i < pageNames.Length; ++i)
        {
            pageStack.Add(_pages[pageNames[i]]);
        }

        if(pageStackReadOnlyList == null)
        {
            pageStackReadOnlyList = pageStack.AsReadOnly();;
        }
        
        _pager.PushMultiplePagesAndChange(pageStackReadOnlyList);
        
    }

}
