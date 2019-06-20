using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actorを代表し、Actorの諸機能を統合するリーダークラス
/// </summary>
public class Actor : MonoBehaviour,IodoShiba.Utilities.IManualUpdate
{
    private ActorManager manager;

    System.Action stateConnectorUpdate;
    System.Action mortalUpdate;

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
                stateConnectorUpdate = value;
        }
    }

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