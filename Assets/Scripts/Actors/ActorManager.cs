using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    HashSet<Actor> actors = new HashSet<Actor>();


    static ActorManager instance;
    static Actor playerActor;
    public static ActorManager Instance { get => instance; }
    public static Actor PlayerActor { get => playerActor; }
    //public ActorManager() { if (instance == null) instance = this; }
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (this != instance)
        {
            Debug.LogError($"{this.GetType().Name} cannot exist double or more in one scene. GameObject '{name}' has been Deleted because it has second {this.GetType().Name}.");
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
        Debug.Log(actor.gameObject.name + "was removed from ActorManager.");
        actors.Remove(actor);
    }

    static void SceneInitialize() { }
}
