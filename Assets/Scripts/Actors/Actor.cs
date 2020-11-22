using System;
using System.Collections;
using System.Collections.Generic;
using Buffs;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Actorを代表し、Actorの諸機能を統合するリーダークラス
/// </summary>
[RequireComponent(typeof(ActorState.ActorStateConnector),typeof(Mortal)),DisallowMultipleComponent]
public class Actor : MonoBehaviour,IodoShibaUtil.ManualUpdateClass.IManualUpdate
{
    //[SerializeField] UnityEngine.Events.UnityEvent onAttacked;
    [SerializeField] bool ignoreActiveReign;
    [SerializeField] Buffs.BuffReceiver buffReceiver;

    private ActorManager manager;

    System.Action stateConnectorUpdate;
    System.Action mortalUpdate;
    FightActorStateConector _fasc;
    Mortal _mortal;

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

    //public UnityEvent OnAttacked { get => onAttacked; }
    public int DirSign { get => dirSign; }
    public bool IgnoreActiveReign { get => ignoreActiveReign; }
    public Mortal Mortal { get => _mortal; }
    public FightActorStateConector FightAsc { get => _fasc; }
    public BuffReceiver BuffReceiver { get => buffReceiver; }

    private void Start()
    {
        Manager.RegisterActor(this);
        stateConnectorUpdate = (_fasc = GetComponent<FightActorStateConector>()).ManualUpdate;
        mortalUpdate = (_mortal = GetComponent<Mortal>()).ManualUpdate;
        buffReceiver.Initialize(this);
    }

    public void ManualUpdate()
    {
        stateConnectorUpdate();
        mortalUpdate();
        BuffReceiver.Update();
    }

    private void OnDestroy()
    {
        manager.RemoveActor(this);
    }
}