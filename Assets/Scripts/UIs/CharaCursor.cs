using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorCommanderUtility;
using static IodoShibaUtil.Vector2DUtility.Vector2DUtilityClass;
using DG.Tweening;
using UniRx;

public class CharaCursor : MonoBehaviour
{
    [SerializeField] SelectableUI current;
    [SerializeField] float moveSpeed;
    [SerializeField] bool locked;

    public SelectableUI Current
    {
        get => current;
        set
        {
            current = value;
        }
    }
    public void MoveTo(SelectableUI target)
    {
        float time = ((Vector2)(target.transform.position - transform.position)).magnitude / moveSpeed;
        transform.DOMove(ModifiedXY(transform.position, target.transform.position), time).SetEase(Ease.Linear);
        locked = true;
        UniRx.Observable.Timer(System.TimeSpan.FromSeconds(time)).Subscribe(_ => locked = false);
        Current = target;
    }

    void Start()
    {
        transform.position = ModifiedXY(transform.position, current.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) { return; }

        Vector2Int input
            = new Vector2Int(
                System.Math.Sign(Input.GetAxisRaw(InputName.Axis.horizontal)),
                System.Math.Sign(Input.GetAxisRaw(InputName.Axis.vertical))
                );

        if(input != Vector2Int.zero)
        {
            SelectableUI neighbor = current.GetNeighbor(input);
            if (neighbor != null)
            {
                MoveTo(neighbor);
            }
        }

        if (Input.GetButtonDown(InputName.Button.attack))
        {
            current.Select();
        }
    }
}
