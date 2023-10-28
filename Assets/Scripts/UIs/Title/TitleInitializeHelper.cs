using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SotS.UI;
using UnityEngine;

public class TitleInitializeHelper : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIPager _pager;

    private Dictionary<string, UIPagerPage> _pages = new Dictionary<string, UIPagerPage>();
    
    public void InitializeScene(string[] stackedPageNames)
    {
        FindUIPagerPages();
        StackPages(stackedPageNames);
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
        for (int i = 0; i < pageNames.Length; ++i)
        {
            _pager.PushStackAndChangePage(_pages[pageNames[i]]);
        }
    }

}
