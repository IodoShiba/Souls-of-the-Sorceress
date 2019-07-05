using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Actorを代表し、Actorの諸機能を統合するリーダークラス
/// </summary>
public class Actor : MonoBehaviour,IodoShiba.ManualUpdateClass.IManualUpdate
{
    [SerializeField] UnityEngine.Events.UnityEvent onAttacked;

    private ActorManager manager;

    System.Action stateConnectorUpdate;
    System.Action mortalUpdate;

    int dirSign = 1;

    public ActorManager Manager { get => manager == null ? (manager = ActorManager.Instance) : manager; }
    public Action StateConnectorUpdate
    {
        get => stateConnectorUpdate;
        set
        {
            if (stateConnectorUpdate==null)
                stateConnectorUpdate = value;
        }
    }
    public Action MortalUpdate
    {
        get => mortalUpdate;
        set
        {
            if (mortalUpdate == null)
                mortalUpdate = value;
        }
    }

    public UnityEvent OnAttacked { get => onAttacked; }
    public int DirSign { get => dirSign; }

    private void Start()
    {
        Manager.RegisterActor(this);
    }

    public void ManualUpdate()
    {
        stateConnectorUpdate();
        mortalUpdate();
    }

}