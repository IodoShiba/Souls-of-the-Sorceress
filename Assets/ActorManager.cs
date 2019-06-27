using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    HashSet<Actor> actors = new HashSet<Actor>();

    static ActorManager instance;

    public static ActorManager Instance { get => instance; }

    public ActorManager() { if (instance == null) instance = this; }
    private void Awake()
    {
        if (instance != null && this != instance)
        {
            Debug.LogError($"{this.GetType().Name} cannot exist double or more in one scene. GameObject '{name}' has been Deleted because it has second {this.GetType().Name}.");
            //Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var a in actors)
        {
            
            a.ManualUpdate();
            
        }
    }

    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
    }
}
