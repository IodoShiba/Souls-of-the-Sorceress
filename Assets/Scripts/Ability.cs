using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
public abstract class Ability : MonoBehaviour {
    public abstract bool Momential
    {
        get;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract void Act();
}
*/

[System.Serializable]
public abstract class Ability : MonoBehaviour
{
    public interface Following<FollowingAbility> where FollowingAbility : Ability { }

    public virtual HashSet<System.Type> MayBeRestrictedBy() { return null; }
    //public virtual HashSet<System.Type> ParallelizableWith() { return null; }
    public virtual bool ContinueUnderBlocked => false;
    //public virtual bool IsExclusive() => true;
    public virtual void Activate() { }
    public abstract bool ContinueCheck(bool ordered);
    public virtual void OnActivated(bool ordered) { }
    public virtual void OnEnd() { }
}

[System.Serializable]
public abstract class BasicAbility : Ability {
    
}

[System.Serializable]
public abstract class ArtsAbility : Ability { }

[System.Serializable]
public abstract class OneShotArtsAbility : ArtsAbility { }

[System.Serializable]
public abstract class ChargeableArtsAbility : ArtsAbility { }