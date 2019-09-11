using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUmbrellaParameterViewer : UmbrellaParameters.Viewer
{
    [SerializeField] UDPointElement elementPrefab;

    List<UDPointElement> gaugeElements = new List<UDPointElement>();

    private void Start()
    {
        gaugeElements.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        AdjustElements();

        for (int i = 0; i < target.MaxDurability; ++i)
        {
            gaugeElements[i].Usable = i < target.Durability;
        }
    }

    void AdjustElements()
    {
        if (gaugeElements.Count < target.MaxDurability)
        {
            var selfRTrans = GetComponent<RectTransform>();
            for(int i = gaugeElements.Count; i < target.MaxDurability; ++i)
            {
                UDPointElement ins = Instantiate(elementPrefab, transform.position + Vector3.right * selfRTrans.sizeDelta.x * 2 * i, Quaternion.identity);
                ins.transform.parent = transform;
                var rTrans = ins.GetComponent<RectTransform>();
                ins.SetSize(new Vector2((rTrans.sizeDelta.x / rTrans.sizeDelta.y) * selfRTrans.sizeDelta.y, selfRTrans.sizeDelta.y));
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
