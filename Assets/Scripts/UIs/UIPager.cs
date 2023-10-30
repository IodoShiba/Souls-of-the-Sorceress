using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Async;

namespace SotS.UI
{
    public class UIPager : MonoBehaviour
    {
        static UIPager activeInstance;

        [SerializeField] UIPagerPage initialPage;
        [SerializeField] bool InitialPageHiddenFirst;
        [SerializeField] bool enterPageInitially;
        [SerializeField] string backButton;

        UIPagerPage currentPage = null;

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
            Debug.Log("UIPager::Start");
            Initialize();
        }

        void Initialize()
        {
            if (currentPage == null)
            {
                if (enterPageInitially)
                {
                    ChangePage(initialPage);
                }
                else
                {
                    currentPage = initialPage;
                }
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
            ChangePage(page, true);
        }

        void ChangePage(UIPagerPage page, bool isFinal)
        {
            if(currentPage != null)
            {
                currentPage.ExitSelection();
            }
            page.EnterSelection(isFinal);
            currentPage = page;
        }

        public void PushStackAndChangePage(UIPagerPage page)
        {
            if(currentPage != null)
            {
                previousStack.Push(currentPage);
            }
            isShownBeforeEnterStack.Push(page.IsShown);
            var prevPage = currentPage;
            ChangePage(page);
            if (prevPage.HideWhenPushed)
            {
                prevPage.Hide();
            }
        }

        public void PushMultiplePagesAndChange(IReadOnlyList<UIPagerPage> pages)
        {
            for(int i=0; i<pages.Count; ++i)
            {
                var page = pages[i];
                PushStackAndChangePage(page);
            }
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