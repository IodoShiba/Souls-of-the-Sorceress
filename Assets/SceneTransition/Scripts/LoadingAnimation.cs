using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] float showAnimationTime = 2;
    [SerializeField] GameObject animationObject;

    bool isLoading = false;

    float loadingTime = 0;

    void Update()
    {
        if (!isLoading) {return;}

        loadingTime += Time.deltaTime;
        Debug.Log($"loadingTime:{loadingTime}");

        if(!animationObject.activeSelf && loadingTime > showAnimationTime)
        {
            animationObject.SetActive(true);
        }
    }

    public void OnLoadStateChanged(bool isLoading)
    {
        this.isLoading = isLoading;
        if (!isLoading)
        {
            loadingTime = 0;
            animationObject.SetActive(false);
        }
    }
}
