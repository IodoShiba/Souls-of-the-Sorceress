﻿using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeSceneToTitle : MonoBehaviour
{
    private static readonly string TitleSceneName = "Title";
    private static readonly string HelperName = "InitializeHelper";

    [SerializeField] private bool noTrigger = false;
    [SerializeField] private string[] _stackedPageNames;
    
    // Dependencies
    [SerializeField] private StorePlayerAndChangeScene changeScene;
    [SerializeField] private SceneChanger sceneChanger;
    
    private Action<Scene> _initializeScene;

    private void Start()
    {
        _initializeScene = scene => { InitializeTitleFunc(scene, _stackedPageNames); };
    }

    public void ChangeScene()
    {
        sceneChanger.ChangeScene(TitleSceneName, _initializeScene);
    }

    public void StoreAndChangeScene()
    {
        Player player = null;

        Actor actor = ActorManager.PlayerActor;
        if (actor != null)
        {
            player = actor.GetComponent<Player>();
        }
        
        StoreAndChangeScene(player);
    }
    
    public void StoreAndChangeScene(Player player)
    {
        changeScene.StoreAndChangeSene(player, TitleSceneName, _initializeScene);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(noTrigger){ return; }
        
        if(collision.tag == TagName.player)
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
            {
                throw new System.NullReferenceException($"Detected GameObject '{collision.name}' does not have 'Player' Component.");
            }

            StoreAndChangeScene(player);
        }
    }
    
    
    static void InitializeTitleFunc(Scene titleScene, string[] stackedPageNames)
    {
        GameObject[] sceneGameObjects;
        // Find helper GameObject
        sceneGameObjects = titleScene.GetRootGameObjects();

        GameObject initializeHelperGameObject = null;
        for (int i = 0; i < sceneGameObjects.Length; ++i)
        {
            if (sceneGameObjects[i].name == HelperName)
            {
                initializeHelperGameObject = sceneGameObjects[i];
                break;
            }
        }

        if (initializeHelperGameObject == null)
        {
            return;
        }
        
        Debug.Log(initializeHelperGameObject);

        CallInitializeHelpFuncDelayed(stackedPageNames, initializeHelperGameObject).Forget();
    }

    static async UniTask<Unit> CallInitializeHelpFuncDelayed(string[] stackedPageNames, GameObject initializeHelperGameObject)
    {
        await UniTask.Yield();
        TitleInitializeHelper initializeHelper;
        initializeHelper = initializeHelperGameObject.GetComponent<TitleInitializeHelper>();
        // And be helped
        try
        {
            initializeHelper.InitializeScene(stackedPageNames);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        return Unit.Default;
    }
    
        
}
