﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUmbrellaParameterViewer : UmbrellaParameters.Viewer
{
    [SerializeField] UDPointElement elementPrefab;
    [SerializeField] List<Vector2> shakeRPositionsOnBreak;
    [SerializeField] float shakeCycleOnBreak;
    [SerializeField] float blinkCycleOnBreak;

    List<UDPointElement> gaugeElements = new List<UDPointElement>();
    Vector3 initPos;
    Sequence sequence;
    bool umbrellaWorking = true;
    bool UmbrellaWorking
    {
        set
        {
            if (!umbrellaWorking && value)
            {
                sequence.Restart();
                sequence.Pause();
                transform.localPosition = initPos;
            }
            if(umbrellaWorking && !value)
            {
                sequence.Play();
            }
            umbrellaWorking = value;
            for(int i=0;i< gaugeElements.Count; ++i)
            {
                gaugeElements[i].SetUmbrellaWork(value);
            }
        }
    }

    private void Start()
    {

        gaugeElements.Clear();
        initPos = transform.localPosition;

        sequence = DOTween.Sequence();
        sequence.OnStart(() => transform.localPosition = initPos);
        for(int i=0; i < shakeRPositionsOnBreak.Count; ++i)
        {
            sequence.Append(gameObject.transform.DOLocalMove(initPos + (Vector3)shakeRPositionsOnBreak[i], shakeCycleOnBreak / shakeRPositionsOnBreak.Count));
        }
        sequence.SetLoops(-1, LoopType.Restart);

        sequence.Restart();
        sequence.Pause();
    }

    void Update()
    {
        AdjustElements();

        for (int i = 0; i < target.MaxDurability; ++i)
        {
            gaugeElements[i].Usable = i < target.Durability;
        }

        UmbrellaWorking = target.DoesUmbrellaWork();
    }

    void AdjustElements()
    {
        if (gaugeElements.Count < target.MaxDurability)
        {
            var selfRTrans = GetComponent<RectTransform>();
            for(int i = gaugeElements.Count; i < target.MaxDurability; ++i)
            {
                UDPointElement ins = Instantiate(elementPrefab, transform.position + Vector3.right * selfRTrans.sizeDelta.x * 2 * i, Quaternion.identity);
                ins.transform.SetParent(transform);
                var rTrans = ins.GetComponent<RectTransform>();
                ins.SetSize(new Vector2((rTrans.sizeDelta.x / rTrans.sizeDelta.y) * selfRTrans.sizeDelta.y, selfRTrans.sizeDelta.y));
                ins.SetBlinkCycle(blinkCycleOnBreak);
                gaugeElements.Add(ins);
            }
        }
        else if (gaugeElements.Count > target.MaxDurability)
        {
            for (int i = gaugeElements.Count-1; i >= target.MaxDurability; --i)
            {
                Destroy(gaugeElements[i].gameObject);
                gaugeElements.RemoveAt(i);
            }
        }
    }
}
