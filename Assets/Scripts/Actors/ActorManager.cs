using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActorManager : MonoBehaviour
{
    HashSet<Actor> actors = new HashSet<Actor>();

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
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var a in actors)
        {
            if (a == null) { continue; }
            a.ManualUpdate();
            
        }
    }

    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
        if(actor.transform.tag == "Player")
        {
            playerActor = actor;
        }
    }

    public void RemoveActor(Actor actor)
    {
        Debug.Log($"'{actor.gameObject.name}' was removed from ActorManager.");
        actors.Remove(actor);
    }

    static void SceneInitialize() { }
}
