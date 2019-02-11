using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endif

[System.Serializable]
public abstract class Skill : MonoBehaviour
{
    public string a;
    public abstract bool IsExclusive();
    public abstract void Activate();
    public abstract bool ContinueCheck();
    public abstract void OnActivated();
    public abstract void OnEnd();
    public abstract void OnInactivated();
}

[System.Serializable]
public abstract class ExclusiveSkill : Skill
{
    public sealed override bool IsExclusive() { return true; }
}

[System.Serializable]
public abstract class InexclusiveSkill : Skill
{
    public sealed override bool IsExclusive() { return false; }
}

/*
[System.Serializable]
public  class Skill : MonoBehaviour
{
    public string a;
    public virtual bool IsExclusive() { return false; }
    public virtual void Activate() { }
    public virtual bool ContinueCheck() { return false; }
    public virtual void OnActivated() { }
    public virtual void OnEnd() { }
    public virtual void OnInactivated() { }
}

[System.Serializable]
public  class ExclusiveSkill : Skill
{
    public  override bool IsExclusive() { return true; }
}

[System.Serializable]
public  class InexclusiveSkill : Skill
{
    public  override bool IsExclusive() { return false; }
}
*/
public class ActorBehaviours : MonoBehaviour
{

    [System.Serializable]
    class SkillSet
    {
        [Header("----------------")]
        public string whenStateIs = "";
        public List<Skill> skills = new List<Skill>();
        [Header("----------------")]
        public StateManager2 dependentStateManager;
        public List<SkillSet> subSkillSets = new List<SkillSet>();

        private HashSet<Skill> realSkills;
        private Dictionary<string, SkillSet> realSubSkillSets;

        public void Validate()
        {
            if (skills.Count > 0)
            {
                realSkills = new HashSet<Skill>(skills);
            }

            if (subSkillSets.Count > 0)
            {
                realSubSkillSets = new Dictionary<string, SkillSet>();
                foreach (var sss in subSkillSets)
                {
                    realSubSkillSets.Add(sss.whenStateIs, sss);
                    sss.Validate();
                }
            }

        }

        public HashSet<Skill> GetAvailableSkills()
        {
            var retset = new HashSet<Skill>();
            retset.UnionWith(realSkills);
            if (realSubSkillSets.ContainsKey(dependentStateManager.CurrentStateName))
            {
                retset.UnionWith(realSubSkillSets[dependentStateManager.CurrentStateName].GetAvailableSkills());
            }
            return retset;
        }
    }

    [SerializeField] SkillSet BaseSkillSet;

    Skill currentSkill;
    HashSet<Skill> availableSkills;

    private void Awake()
    {
        BaseSkillSet.Validate();
    }

    private void Start()
    {
        availableSkills = BaseSkillSet.GetAvailableSkills();
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        BaseSkillSet.Validate();
        BaseSkillSet.whenStateIs = "Base";
    }

    void DecideBehavior(Skill targetSkill)
    {
        if (currentSkill != null && currentSkill.IsExclusive()) { return; }
        if (targetSkill != null)
        {
            availableSkills = BaseSkillSet.GetAvailableSkills();
            if (availableSkills.Contains(targetSkill))
            {
                currentSkill.OnEnd();
                targetSkill.Activate();
                currentSkill = targetSkill;
            }
        }

        if (currentSkill.ContinueCheck())
        {
            currentSkill.OnActivated();
        }
        else
        {
            currentSkill.OnEnd();
            currentSkill = null;
        }
    }


    
#if UNITY_EDITOR
    [CustomEditor(typeof(ActorBehaviours))]
    public class EditorModify : Editor
    {
        SkillSetViewer defaultssv;
        private void OnEnable()
        {
            defaultssv = new SkillSetViewer((target as ActorBehaviours).BaseSkillSet);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ActorBehaviours actorBehaviours = target as ActorBehaviours;

            Undo.RecordObject(actorBehaviours,"Actor Behaviours");

            LabelField("Default Skill Set");
            defaultssv.Call();
        }

        class SkillSetViewer
        {
            //EditorModify owner;
            SkillSet target;
            bool selfFold;
            bool skillsRootFold;
            bool subsFold;
            List<SkillSetViewer> subs;

            public SkillSetViewer(SkillSet target) {
                //this.owner = owner;
                this.target = target;
                this.subs = new List<SkillSetViewer>();
                if (target != null && target.subSkillSets.Count > 0)
                {
                    foreach(var ss in target.subSkillSets)
                    {
                        subs.Add(new SkillSetViewer(ss));
                    }
                }
            }

            public void Call() {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (selfFold = Foldout(selfFold, target.whenStateIs))
                    {
                        EditorGUI.indentLevel++;
                        
                        target.whenStateIs = TextField("When the state is:",target.whenStateIs);
                        if (skillsRootFold = Foldout(skillsRootFold, "Skills"))
                        {
                            for (int i = 0; i < target.skills.Count; ++i)
                            {
                                using (new HorizontalScope())
                                {
                                    target.skills[i] = ObjectField(target.skills[i], typeof(Skill), true) as Skill;
                                    if (GUILayout.Button("Remove"))
                                    {
                                        target.skills.RemoveAt(i);
                                    };
                                }
                            }
                            if (GUILayout.Button("Add New Skill")) {
                                target.skills.Add(default);
                            };
                        }
                        Space();
                        
                        if (subsFold = Foldout(subsFold, "Sub Skill Sets"))
                        {
                            target.dependentStateManager = ObjectField("Dependent State Manager", target.dependentStateManager, typeof(StateManager2), true) as StateManager2;
                            for (int i = 0; i < target.subSkillSets.Count; ++i)
                            {
                                subs[i].Call();
                                using (new HorizontalScope())
                                {
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button("Remove"))
                                    {
                                        target.subSkillSets.RemoveAt(i);
                                        subs.RemoveAt(i);
                                    };
                                }
                            }
                            if (GUILayout.Button("Add New Sub Skill Set"))
                            {
                                SkillSet newSkillSet = new SkillSet();
                                newSkillSet.Validate();
                                target.subSkillSets.Add(newSkillSet);
                                subs.Add(new SkillSetViewer(newSkillSet));
                            };
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }

            
        }
    }
#endif

}
