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
public abstract class ActorBehaviour : MonoBehaviour
{
    public interface Following<FollowingAbility> where FollowingAbility : ActorBehaviour { }
    public interface IParamableWith<T> { ActorBehaviour SetParams(T value); }
    bool activated = false;
    public virtual HashSet<System.Type> MayBeRestrictedBy() { return null; }
    //public virtual HashSet<System.Type> ParallelizableWith() { return null; }
    public virtual bool ContinueUnderBlocked => false;

    public bool Activated { get => activated;}

    //public virtual bool IsExclusive() => true;
    public virtual bool IsAvailable() { return true; }
    private void Activate() { activated = true; ActivateImple(); }
    protected virtual void ActivateImple() { }
    protected abstract bool CanContinue(bool ordered);
    protected virtual void OnActive(bool ordered) { }
    protected void OnEnd() { activated = false;OnEndImple(); }
    protected virtual void OnEndImple() { }

    public void SendSignal() { owner.SendSignal(selfId); }

    ActorBehavioursManager owner;
    int selfId = -1;

    //public abstract class ActorBehavioursManager
    //
    //Actor(主人公や敵など、ステージ上にあってAIによって動くもの)のふるまいを記述するコンポーネントクラス。
    //1つのAIから判断を仰ぎキャラクターごとの能力を発動する。
    //ダメージを受けたときに体力を減らす、ノックバックを受けるなどの受動的挙動を開始する。
    //ある意味このゲームの中枢というべき重要なクラスである。
    //
    //使用法
    //このクラスは"AI"クラスと"ActorBehaviour"クラスに依存する。
    //Actorの種類ごとにこのクラスを継承したクラスを定義する必要がある。
    //新たな種類のActorを作る手順…
    // 1 : Actorを表すGameObjectに、そのActorが発動するAbilityのコンポーネントを取り付ける
    // 2 : このクラスを継承し、Structure()メソッドを実装する。
    //   ・このメソッド内でAllow<AbilityType>()を呼び出すと、当クラスがAbilityTypeに指定したAbilityを実行対象に追加する。
    //     このメソッドを呼び出すことは、当クラスの継承クラスを取り付けたGameObjectが表すActorが
    //     そのAbilityを発動できることを表す。
    //   ・Allow<_>().Follow<AbilityType>(受付秒数)でコンボ攻撃を定義できる。
    //   　Allow<_>().Follow<_>().Follow<_>()...と続けることもできる。
    //   ・using(IfScope(Func<bool>)) ブロック内でAllow<AbilityType>()を呼び出すと、
    //     Actorが指定したAbilityを発動するために必要な条件を指定できる。
    //     これを入れ子にして条件を複数付けることもできる。
    //     ・条件指定でusingの()内に入れるのは内部クラスConditionのインスタンスであるが、
    //       Conditionのインスタンスを直接()の中に入れてはならない。
    //       必ずIfScope(Condition)を通すように。
    // 3 : 2で実装した当クラスの継承クラスのためのAIを用意するため、AIクラスを継承し、AskDecision()メソッドを実装する。
    //   ・AskDecision()メソッドは毎フレーム呼ばれるメソッドであり、
    //     この中でAbilityの継承クラスのインスタンスを直接扱い、Actorの意志決定を行う。
    //     Awake()メソッド内で自分のGameObjectに付けられているAbilityのインスタンスを取得し、
    //     AskDecision()メソッド内で「これを行う」と判断したAbilityに対応するインスタンスのSendSignal()メソッドを呼ぶとよい。
    // 4 : 2で実装した当クラスの継承クラスと、3で実装したAIクラスの継承クラスをActorを表すGameObjectに取り付ける。
    // 5 : Inspectorから、4で取り付けた当クラスの継承クラスのインスタンスのAIフィールドに、4で取り付けたAIを入れる。
    //
    public abstract class ActorBehavioursManager : MonoBehaviour
    {
        
        protected class BehaviourMed
        {
            public ActorBehavioursManager owner;
            public ActorBehaviour target;
            private List<Condition> conditions = new List<Condition>();
            private float lastCompletedTime;
            private bool isCompletedLastTime = false;
            //private bool activated = false;
            private bool signaled = false;

            public bool IsTargetActivated { get => target.Activated; }
            public float LastCompletedTime { get => lastCompletedTime; }
            public bool IsCompletedLastTime { get => isCompletedLastTime; }
            public bool Signaled { get => signaled; }

            public void SendSignal() { signaled = true; }
            public void ResetSignal() { signaled = false; }

            public void Update()
            {
                if (IsTargetActivated)
                {
                    OnActive(Signaled);
                }
                else
                {
                    if (Signaled)
                    {
                        Attempt();
                    }
                }
            }

            public bool Attempt()
            {
                bool cond = conditions.TrueForAll(c => c.Call()) && target.IsAvailable();
                if (cond)
                {
                    target.Activate();
                    //activated = true;
                }
                return cond;
            }

            public bool OnActive(bool ordered)
            {
                bool stillActivated = false;
                if (target.activated)
                {
                    if (stillActivated = target.CanContinue(ordered))
                    {
                        target.OnActive(ordered);
                    }
                    else
                    {
                        Inactivate(true);
                    }
                }
                return stillActivated;
            }

            public void Inactivate() { Inactivate(false); }

            private void Inactivate(bool completed)
            {
                target.OnEnd();
                if (completed)
                {
                    lastCompletedTime = Time.time;
                }
                isCompletedLastTime = completed;
                //activated = false;
            }

            public BehaviourMed(ActorBehavioursManager owner, ActorBehaviour target)
            {
                this.owner = owner;
                this.target = target;
                target.owner = owner;
                target.selfId = owner.allAbilityMeds.Count;

                owner.allAbilityMeds.Add(this);
                switch (target)
                {
                    case BasicAbility _target:
                        owner.basicAbilityMeds.Add(this);
                        break;
                    case ArtsAbility _target:
                        owner.artsAbilityMeds.Add(this);
                        break;
                    case PassiveBehaviour _target:
                        owner.passiveBehaviourMeds.Add(this);
                        break;
                }
                lastCompletedTime = float.MinValue;
            }

            public void AddCondition(Condition item)
            {
                conditions.Add(item);
            }

            public FollowCondition Follow<AbilityType>(float followingActivateTimeLimit) where AbilityType : Ability
            {
                var am = owner.Allow<AbilityType>();
                var fc0 = new FollowCondition(null, this, 0);
                var fc1 = new FollowCondition(this, am, followingActivateTimeLimit);
                this.conditions.Add(fc0);
                am.conditions.Add(fc1);
                new FollowCondition.FollowInstructor(am, fc0, fc1);
                return fc1;
            }

        }
        

        protected class Condition : System.IDisposable
        {
            protected System.Func<bool> cond;
            bool calledResult;
            float lastCalled;
            ActorBehavioursManager owner;

            public Condition( System.Func<bool> cond, ActorBehavioursManager owner=null)
            {
                this.owner = owner;
                this.cond = cond;
                if(owner !=null) owner.settingConditions.Add(this);
                lastCalled = Time.time;
            }
            /*
            public Condition(System.Func<bool> cond)
            {
                this.cond = cond;
                lastCalled = Time.time;
            }
            */
            public Func<bool> Cond { get => cond; }
            public bool IsCalledResultObsolete { get => lastCalled != Time.time; }

            public bool Call()
            {
                if (IsCalledResultObsolete)
                {
                    lastCalled = Time.time;
                    return calledResult = cond();
                }
                else
                {
                    return calledResult;
                }
            }

            void System.IDisposable.Dispose()
            {
                owner.lastEscapedCondition = owner.settingConditions[owner.settingConditions.Count - 1];
                owner.settingConditions.RemoveAt(owner.settingConditions.Count - 1);
            }

            public Condition MakeDenial()
            {
                return new Condition( () => !Call(), this.owner);
            }

            public Condition MakeDenial(ActorBehavioursManager owner)
            {
                return new Condition(() => !Call(), owner);
            }
        }

        protected class FollowCondition : Condition
        {
            private FollowInstructor instructor;
            private Func<bool> isWaitingInput;
            private bool entitled = false;
            private BehaviourMed owner;

            public FollowCondition(BehaviourMed basis, BehaviourMed owner, float followingActivateTimeLimit)
                : base(() => false)
            {
                if (basis == null)
                {
                    isWaitingInput = () => true;
                    base.cond = () => { instructor.StartCombo(); return entitled; };
                }
                else
                {
                    isWaitingInput = () => Time.time < basis.LastCompletedTime + followingActivateTimeLimit;
                    base.cond = () =>
                    {
                        instructor.UpdateEntitles();
                        return entitled;
                    };
                }
                this.owner = owner;
            }

            public FollowCondition Follow<AbilityType>(float followingActivateTimeLimit) where AbilityType : Ability
            {
                return instructor.Follow<AbilityType>(followingActivateTimeLimit);
            }

            public class FollowInstructor
            {
                BehaviourMed lastAm;
                List<FollowCondition> fcs = new List<FollowCondition>();
                int entitledIndex = 0;
                float comboStartedTime = 0;
                float lastUpdateTime = float.MinValue;

                public FollowInstructor(BehaviourMed last, FollowCondition first, FollowCondition second)
                {
                    this.lastAm = last;
                    fcs.Add(first);
                    fcs.Add(second);
                    first.entitled = true;
                    first.instructor = second.instructor = this;
                }

                public FollowCondition Follow<AbilityType>(float followingActivateTimeLimit) where AbilityType : Ability
                {
                    var newAm = lastAm.owner.Allow<AbilityType>();
                    var newFc = new FollowCondition(lastAm, newAm, followingActivateTimeLimit);
                    newFc.instructor = this;
                    newAm.AddCondition(newFc);
                    fcs.Add(newFc);

                    lastAm = newAm;
                    return newFc;
                }

                public void UpdateEntitles()
                {
                    if (Time.time == lastUpdateTime) { return; }
                    lastUpdateTime = Time.time;

                    int formerEntitledIndex = entitledIndex;
                    while (entitledIndex < fcs.Count && fcs[entitledIndex].owner.LastCompletedTime > comboStartedTime)
                    {
                        entitledIndex++;
                        if (entitledIndex >= fcs.Count)
                        {
                            entitledIndex = 0;
                            break;
                        }
                    }

                    if (!(fcs[entitledIndex].isWaitingInput() && lastAm.owner.lastDisabledTime <= comboStartedTime))
                    {
                        entitledIndex = 0;
                    }
                    fcs[formerEntitledIndex].entitled = false;
                    fcs[entitledIndex].entitled = true;
                }

                public void StartCombo()
                {
                    UpdateEntitles();
                    if (entitledIndex == 0)
                    {
                        comboStartedTime = Time.time;
                    }
                }
            }
        }

        [SerializeField] AI ai;
        //Dictionary<System.Type, AbilityMed> allAbilityMeds = new Dictionary<System.Type, AbilityMed>();
        List<BehaviourMed> allAbilityMeds = new List<BehaviourMed>();
        List<BehaviourMed> basicAbilityMeds = new List<BehaviourMed>();
        List<BehaviourMed> artsAbilityMeds = new List<BehaviourMed>();
        List<BehaviourMed> passiveBehaviourMeds = new List<BehaviourMed>();
        BehaviourMed currentArtMed;
        int currentArtMedIndex = -1;
        HashSet<System.Type> currentDecisions = new HashSet<System.Type>();
        //List<PassiveBehaviour> passiveBehaviours = new List<PassiveBehaviour>();

        List<Condition> settingConditions = new List<Condition>();
        Condition lastEscapedCondition = null;
        float lastDisabledTime = -1;

        protected Condition IfScope(System.Func<bool> condf)
        {
            lastEscapedCondition = null;
            return new Condition(condf, this);
        }
        protected Condition IfScope(Condition condition)
        {
            return IfScope(condition.Call);
        }
        protected Condition ElseScope()
        {
            if (lastEscapedCondition != null)
            {
                return lastEscapedCondition.MakeDenial();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        protected Condition DenyIfScope(Condition condition) => condition.MakeDenial(this);

        protected abstract void Structure();

        private void Start()
        {
            List<PassiveBehaviour> passiveBehaviours = new List<PassiveBehaviour>();
            GetComponents<PassiveBehaviour>(passiveBehaviours);
            foreach(var pb in passiveBehaviours)
            {
                new BehaviourMed(this, pb);
            }
            settingConditions.Add(new Condition(() => passiveBehaviourMeds.TrueForAll(pbm => !pbm.IsTargetActivated)));
            Structure();
            settingConditions = null;
        }

        private void Update()
        {
            ai.AskDecision();

            passiveBehaviourMeds.ForEach(pbm => pbm.Update());
            basicAbilityMeds.ForEach(bam => bam.Update());
            
            if(currentArtMedIndex == -1)
            {
                for(int i=0; i < artsAbilityMeds.Count; ++i)
                {
                    if (artsAbilityMeds[i].Signaled && artsAbilityMeds[i].Attempt())
                    {
                        currentArtMedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                if (!artsAbilityMeds[currentArtMedIndex].OnActive(artsAbilityMeds[currentArtMedIndex].Signaled))
                {
                    currentArtMedIndex = -1;
                }
                basicAbilityMeds.ForEach(m => m.Inactivate());
            }

            allAbilityMeds.ForEach(am => am.ResetSignal());

            Debug.Log(Input.GetButton("Jump"));
        }

        protected BehaviourMed Allow<AbilityType>() where AbilityType : Ability
        {
            Ability ability = GetComponent<AbilityType>();
            BehaviourMed newAbilityMed = new BehaviourMed(this, ability);

            foreach (var c in settingConditions)
            {
                newAbilityMed.AddCondition(c);
            }
            return newAbilityMed;
        }

        private void OnDisable()
        {
            lastDisabledTime = Time.time;
        }

        public void SendSignal(int id)
        {
            allAbilityMeds[id].SendSignal();
        }
    }

}

public abstract class Ability : ActorBehaviour { }

public abstract class PassiveBehaviour : ActorBehaviour { }

[System.Serializable]
public abstract class BasicAbility : Ability { }

[System.Serializable]
public abstract class ArtsAbility : Ability
{
    public virtual IEnumerator<Type> ParallerizableBasics() { yield return null; }
}

[System.Serializable]
public abstract class OneShotArtsAbility : ArtsAbility { }

[System.Serializable]
public abstract class ChargeableArtsAbility : ArtsAbility { }
