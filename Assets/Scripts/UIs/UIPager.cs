using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

namespace SotS.UI
{
    public class UIPager : MonoBehaviour
    {
        static UIPager activeInstance;

        [SerializeField] UIPagerPage initialPage;
        [SerializeField] bool InitialPageHiddenFirst;
        [SerializeField] string backButton;

        UIPagerPage currentPage;

        bool isBackAllowed = true;
        Stack<UIPagerPage> previousStack = new Stack<UIPagerPage>();
        Stack<bool> isShownBeforeEnterStack = new Stack<bool>();

        public static UIPager ActiveInstance { get => activeInstance; }

        private void Awake()
        {
            activeInstance = this;
        }

        void Start()
        {
            if(InitialPageHiddenFirst)
            {
                ChangePage(initialPage);
            }
            else
            {
                currentPage = initialPage;
            }
        }

        void Update()
        {
            if(isBackAllowed && InputDaemon.WasPressedThisFrame(backButton))
            {
                PopStackAndReturnPage();
            }
        }

        public void ChangePage(UIPagerPage page)
        {
            if(currentPage != null)
            {
                currentPage.ExitSelection();
            }
            page.EnterSelection();
            currentPage = page;
        }

        public void PushStackAndChangePage(UIPagerPage page)
        {
            if(currentPage != null)
            {
                previousStack.Push(currentPage);
            }
            isShownBeforeEnterStack.Push(page.IsShown);
            ChangePage(page);
        }


        public void PopStackAndReturnPage()
        {
            if(previousStack.Count == 0)
            {
                return;
            }

            var isShownBeforeEnter = isShownBeforeEnterStack.Pop();
            if(currentPage != null && !isShownBeforeEnter)
            {
                currentPage.Hide();
            }

            ChangePage(previousStack.Pop());
        }

        public void AllowBack(bool does)
        {
            isBackAllowed = does;
        }

        public async void AsyncAllowBackDeffered(bool does)
        {
            int wait = 3;
            for (int i = 0; i < wait; ++i)
            {
                await UniTask.Yield();
            }
            AllowBack(does);
        }
    }
}