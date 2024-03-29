﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActorManager : MonoBehaviour
{
    const float REIGN_MARGIN = 0.2f;

    [SerializeField] bool keepInvisibleActorActive;
    [SerializeField] Vector2 activeReignExtension;

    HashSet<Actor> actors = new HashSet<Actor>();
    Camera camera;

    static ActorManager instance;
    static Actor playerActor;

    /// <summary>
    /// このクラスのインスタンスを返す
    /// Awake()より後で有効
    /// </summary>
    public static ActorManager Instance { get => instance; }

    /// <summary>
    /// PlayerのGameObjectにアタッチされているActorコンポーネントを取得する
    /// Update()内なら有効なインスタンスを返すことを保証すべきである
    /// </summary>
    public static Actor PlayerActor { get => playerActor; }
    //public ActorManager() { if (instance == null) instance = this; }
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (this != instance)
        {
            Debug.LogError($"Two or more {this.GetType().Name} cannot exist in one scene. GameObject '{name}' has been Deleted because it has second {this.GetType().Name}.");
            Destroy(gameObject);
            return;
        }
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(camera == null)
        {
            camera = Camera.main;
            if(camera == null)
            {
                return;
            }
        }
        Camera mainCam = camera;
        Vector2 mainCamPos = mainCam.transform.position;
        Vector2 size = new Vector2(mainCam.orthographicSize * mainCam.aspect * (1 + REIGN_MARGIN + activeReignExtension.x), mainCam.orthographicSize * (1 + REIGN_MARGIN + activeReignExtension.y));
        Rect activeReign = new Rect(mainCamPos.x - size.x, mainCamPos.y - size.y, 2 * size.x, 2 * size.y);


        DrawRect(activeReign);

        foreach (var a in actors)
        {
            if (a == null) { continue; }
            else
            {
                if (a.gameObject.activeSelf && a.enabled)
                {
                    a.ManualUpdate();
                }

                if (keepInvisibleActorActive || a.IgnoreActiveReign) { continue; }

                bool isToBeActivated = activeReign.Contains(a.transform.position);
                if (a.gameObject.activeSelf != isToBeActivated)
                {
                    a.gameObject.SetActive(isToBeActivated);
                }
            }
        }

        void DrawRect(in Rect rect)
        {
            Vector3 tr = new Vector3(rect.xMax, rect.yMax, -2);
            Vector3 tl = new Vector3(rect.xMin, rect.yMax, -2);
            Vector3 br = new Vector3(rect.xMax, rect.yMin, -2);
            Vector3 bl = new Vector3(rect.xMin, rect.yMin, -2);
            Debug.DrawLine(tr, tl, Color.cyan);
            Debug.DrawLine(br, bl, Color.cyan);
            Debug.DrawLine(tr, br, Color.cyan);
            Debug.DrawLine(tl, bl, Color.cyan);
        }
    }

    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
        if(actor.transform.tag == "Player")
        {
            playerActor = actor;
        }

        //Debug.Log($"registered Actor '{actor.name}'");
    }

    public void RemoveActor(Actor actor)
    {
        //Debug.Log($"'{actor.gameObject.name}' was removed from ActorManager.");
        actors.Remove(actor);
    }

    static void SceneInitialize() { }
}
