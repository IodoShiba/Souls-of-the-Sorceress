using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SotS.UI
{
    public class UIPager : MonoBehaviour
    {
        [SerializeField] UIPagerPage initialPage;
        [SerializeField] bool InitialPageHiddenFirst;
        [SerializeField] string backButton;

        UIPagerPage currentPage;

        Stack<UIPagerPage> previousStack = new Stack<UIPagerPage>();
        Stack<bool> isShownBeforeEnterStack = new Stack<bool>();

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
            if(InputDaemon.WasPressedThisFrame(backButton))
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
    }
}