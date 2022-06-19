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
            if(Input.GetButtonDown(backButton))
            {
                PopStackAndReturnPage();
            }
        }

        public void ChangePage(UIPagerPage page)
        {
            if(currentPage != null)
            {
                currentPage.Hide();
            }
            page.Show();
            currentPage = page;
        }

        public void PushStackAndChangePage(UIPagerPage page)
        {
            if(currentPage != null)
            {
                previousStack.Push(currentPage);
            }
            ChangePage(page);
        }

        public void PopStackAndReturnPage()
        {
            if(previousStack.Count == 0)
            {
                return;
            }

            ChangePage(previousStack.Pop());
        }
    }
}