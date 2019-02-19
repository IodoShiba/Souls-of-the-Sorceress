using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class AI : MonoBehaviour
{
    [SerializeField] protected ActorBehaviours actorBehaviours;
    [SerializeField] protected List<Ability> skills;
    protected Dictionary<System.Type, Ability> realSkill = new Dictionary<System.Type, Ability>();

    private void Start()
    {
        foreach(var s in skills)
        {
            realSkill.Add(s.GetType(), s);
        }
    }

    public virtual void Decide(HashSet<System.Type> decisions) { return ; }
}

public class PlayerManualAI : AI
{
    // Update is called once per frame
    public override void Decide(HashSet<System.Type> decisions)
    {
        int sign = 0;
        if((sign = Sign(Input.GetAxis("Horizontal"))) != 0)
        {
            decisions.Add(typeof(HorizontalMove));
        }
        if (Input.GetButton("Jump"))
        {
            decisions.Add(typeof(Jump));
        }
    }
}
