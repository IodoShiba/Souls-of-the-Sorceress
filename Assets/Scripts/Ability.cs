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
    bool activated = false;
    public virtual HashSet<System.Type> MayBeRestrictedBy() { return null; }
    //public virtual HashSet<System.Type> ParallelizableWith() { return null; }
    public virtual bool ContinueUnderBlocked => false;

    public bool Activated { get => activated;}

    //public virtual bool IsExclusive() => true;
    public virtual bool IsAvailable() { return true; }
    public void Activate() { activated = true; ActivateImple(); }
    public virtual void ActivateImple() { }
    public abstract bool CanContinue(bool ordered);
    public virtual void OnActive(bool ordered) { }
    public void OnEnd() { activated = false;OnEndImple(); }
    public virtual void OnEndImple() { }
    
}

[System.Serializable]
public abstract class BasicAbility : Ability {
    
}

[System.Serializable]
public abstract class ArtsAbility : Ability
{
    public virtual IEnumerator<Type> ParallerizableBasics() { yield return null; }
}

[System.Serializable]
public abstract class OneShotArtsAbility : ArtsAbility { }

[System.Serializable]
public abstract class ChargeableArtsAbility : ArtsAbility { }