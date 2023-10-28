using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageSwitcher : MonoBehaviour
{
    [SerializeField] private float switchPeriod = 10.0f;
    [SerializeField] private float transitionTime = 1.0f;
    
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image fore;
    [SerializeField] private Image back;

    private int current = 0;
    private float restTimeToSwitch;
    
    // Start is called before the first frame update
    void Start()
    {
        restTimeToSwitch = switchPeriod;
    }

    // Update is called once per frame
    void Update()
    {
        restTimeToSwitch -= Time.deltaTime;

        if (restTimeToSwitch <= 0.0f)
        {
            SwitchImage();
            restTimeToSwitch = switchPeriod;
        }
    }

    void SwitchImage()
    {
        int next = (current + 1) % sprites.Length;
        fore.color = Color.white;
        fore.sprite = sprites[current];
        back.sprite = sprites[next];
        fore.DOColor(new Color(1f, 1f, 1f, 0), transitionTime);
        current = next;
    }
}
