using System.Collections;
using System.Collections.Generic;
using SotS.UI;
using UnityEngine;

public class UIPagerPageGroup : MonoBehaviour
{
    [SerializeField] private UIPagerPage[] pages;

    public void ShowAlone(UIPagerPage page)
    {
        for (int i = 0; i < pages.Length; ++i)
        {
            if (page == pages[i])
            {
                pages[i].Show();
            }
            else
            {
                pages[i].Hide();
            }
        }
    }

    public void ShowAlone(int index)
    {
        for (int i = 0; i < pages.Length; ++i)
        {
            if (i == index)
            {
                pages[i].Show();
            }
            else
            {
                pages[i].Hide();
            }
        }
    }

    public void HideAll()
    {
        for (int i = 0; i < pages.Length; ++i)
        {
            pages[i].Hide();
        }
    }
}
