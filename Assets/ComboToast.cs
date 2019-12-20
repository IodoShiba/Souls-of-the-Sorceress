using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static IodoShibaUtil.Vector2DUtility.Vector2DUtilityClass;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ComboToast : MonoBehaviour
{
    const float maxSlideDistance = 2;
    const float jumpUpHeight = 1;
    const int jumpCount = 2;
    const float jumpDuration = 0.4f;
    const float extraDuration = 0.5f;
    const int showBGThreshold = 3;
    const float textBeatScale = 2f;
    const float textBeatTime = 0.05f;
    const string format = "{0}<color=#BFBFBF><size=12>Hit</size></color>";

    [SerializeField] Color[] colors;
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] UnityEngine.UI.Image bgImage;

    int comboCount;
    float entireDuration;
    Vector3 textOriginalScale;

    Sequence bounceSeq;

    Vector3 dest;

    bool isInCombo = false;


    public bool IsInCombo { get => isInCombo; set => isInCombo = value; }

    private void Awake()
    {
        textOriginalScale = text.transform.localScale;
    }
    private void Update()
    {
        entireDuration = Mathf.Max(entireDuration - Time.deltaTime, 0);
        
        if(!IsInCombo && entireDuration == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnComboIncrement(in Vector2 initialPosition, int comboCount)
    {
        this.comboCount = comboCount;

        if (comboCount >= 2)
        {
            if (comboCount == 2)
            {
                Begin(initialPosition);
            }
            else
            {
                entireDuration = Mathf.Max(entireDuration, extraDuration);
            }
            text.text = string.Format(format, comboCount);
            text.color = colors[Mathf.Min(comboCount - 1, colors.Length - 1)];
            bgImage.enabled = comboCount >= showBGThreshold;

            text.transform.localScale = textBeatScale * textOriginalScale;
            text.transform.DOScale(textOriginalScale, textBeatTime);
        }

    }

    void Begin(in Vector2 initialPosition)
    {
        transform.position = ModifiedXY(transform.position, initialPosition);
        if(bounceSeq != null) { bounceSeq.Kill(); }
        dest = transform.position + CircleRandom();
        bounceSeq = transform.DOJump(dest, jumpUpHeight, jumpCount, jumpDuration).SetEase(Ease.Linear);
        entireDuration = jumpDuration;
        IsInCombo = true;
        gameObject.SetActive(true);
    }

    Vector3 CircleRandom()
    {
        float rad = Random.Range(0, 2 * Mathf.PI);
        float mag = Random.Range(0, maxSlideDistance);
        return new Vector3(mag * Mathf.Cos(rad), mag * Mathf.Sin(rad), 0);
    }

    public void EndCombo()
    {
        isInCombo = false;
    }

    private void OnDisable()
    {
        comboCount = 0;
        bgImage.enabled = false;
    }
}

