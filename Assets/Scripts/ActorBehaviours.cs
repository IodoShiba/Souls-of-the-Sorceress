using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endif

public class ActorBehaviours : MonoBehaviour
{

    [System.Serializable]
    class AbilitySet
    {
        [Header("----------------")]
        [UnityEngine.Serialization.FormerlySerializedAs("whenAbilityIs")]
        public string whenStateIs = "";
        public List<Ability> abilities = new List<Ability>();
        [Header("----------------")]
        public StateManager2 dependentStateManager;
        public List<AbilitySet> subAbilitySets = new List<AbilitySet>();

        private HashSet<Ability> realAbilities;
        //private Dictionary<string,Skill> realSkills;
        private Dictionary<string, AbilitySet> realSubAbilitySets;

        public void Validate()
        {
            if (abilities.Count > 0)
            {
                realAbilities = new HashSet<Ability>(abilities);//new Dictionary<string, Skill>();
                /*
                foreach (var s in skills)
                {
                    realSkills.Add(s.GetType().Name, s);
                }
                */
            }

            if (subAbilitySets.Count > 0)
            {
                realSubAbilitySets = new Dictionary<string, AbilitySet>();
                foreach (var sss in subAbilitySets)
                {
                    realSubAbilitySets.Add(sss.whenStateIs, sss);
                    sss.Validate();
                }
            }

        }

        public void _RegisterUpdateAvilableCallback(UnityEngine.Events.UnityAction f)
        {
            if (dependentStateManager != null) { dependentStateManager.RegisterStateChangeCallback(f); }
            if (subAbilitySets != null)
            {
                foreach(var sabs in subAbilitySets)
                {
                    sabs._RegisterUpdateAvilableCallback(f);
                }
            }
        }

        public HashSet<Ability> GetAvailableAbilities()
        {
            var retset = new HashSet<Ability>();
            //var retset = new Dictionary<string,Skill>();
            GetAvailableAbilities(retset);
            return retset;
        }

        private HashSet<Ability> GetAvailableAbilities(HashSet<Ability> retset)
        {
            retset.UnionWith(realAbilities);
            if (dependentStateManager != null && realSubAbilitySets.ContainsKey(dependentStateManager.CurrentStateName))
            {
                realSubAbilitySets[dependentStateManager.CurrentStateName].GetAvailableAbilities(retset);
            }
            return retset;
        }

        public HashSet<Ability> GetAbilitiesAll()
        {
            var retset = new HashSet<Ability>();
            GetAbilitiesAll(retset);
            return retset;
        }

        private HashSet<Ability> GetAbilitiesAll(HashSet<Ability> retset)
        {
            retset.UnionWith(this.abilities);
            if (subAbilitySets != null)
            {
                foreach (var abis in subAbilitySets)
                {
                    abis.GetAbilitiesAll(retset);
                }
            }
            return retset;
        }

        public List<_MediatorBlocker> ConstructMediatorBlocker(Dictionary<System.Type, _AbilityMediator> allAbilitymediator)
        {
            var retlist = new List<_MediatorBlocker>();
            foreach (var sas in subAbilitySets)
            {
                sas.ConstructMediatorBlocker(dependentStateManager, retlist, allAbilitymediator);
            }
            return retlist;
        }

        private void ConstructMediatorBlocker(
            StateManager2 dependentStateManager,
            List<_MediatorBlocker> mediatorBlockers, 
            Dictionary<System.Type, _AbilityMediator> allAbilitymediator)
        {
            _MediatorBlocker newMedb = new _MediatorBlocker(() => dependentStateManager.CurrentStateName == whenStateIs);
            mediatorBlockers.Add(newMedb);
            foreach(var s in GetAbilitiesAll())
            {
                newMedb.AddTarget(allAbilitymediator[s.GetType()]);
            }
            if (dependentStateManager != null)
            {
                foreach (var sas in subAbilitySets)
                {
                    sas.ConstructMediatorBlocker(dependentStateManager, mediatorBlockers, allAbilitymediator);
                }
            }
        }
    }

    private class _AbilityMediator
    {
        public enum ActivateState
        {
            inactivated,
            activated,
        }

        private Ability target;
        private int blockCount = 0;
        bool ordered = false;
        List<_AbilityMediator> priorTo = new List<_AbilityMediator>();
        ActivateState activateState = ActivateState.inactivated;

        public Ability Target { get => target; }

        public _AbilityMediator(Ability target) { this.target = target; }

        public void ConstructPriorTo(Dictionary<System.Type, _AbilityMediator> allMediator)
        {
            HashSet<System.Type> mayBeCanceledBy = target.MayBeRestrictedBy();
            if (mayBeCanceledBy != null)
            {
                foreach (var st in mayBeCanceledBy)
                {
                    foreach (var abt in allMediator.Keys) 
                    {
                        if (st.IsAssignableFrom(abt)&&abt!=this.target.GetType())
                        {
                            allMediator[abt].priorTo.Add(this);
                        }
                    }
                }
            }
        }

        public bool Order()
        {
            if (blockCount == 0 && activateState == ActivateState.inactivated)
            {
                target.Activate();
                activateState = ActivateState.activated;
                foreach(var abim in priorTo)
                {
                    abim.Block(true);
                }
            }
            ordered = true;
            return activateState == ActivateState.activated;
        }

        //trueでブロック開始　falseでブロック解除
        public void Block(bool option)
        {
            if (option)
            {
                ++blockCount;
                if (!target.ContinueUnderBlocked)
                {
                    Inactivate();
                }
            }
            else
            {
                --blockCount;
            }
        }

        public void Inactivate()
        {
            if (activateState == ActivateState.inactivated) { return; }

            target.OnEnd();
            activateState = ActivateState.inactivated;
            foreach(var abim in priorTo)
            {
                abim.Block(false);
            }
        }

        public void Update()
        {
            if (activateState == ActivateState.activated)
            {
                if (target.ContinueCheck(ordered))
                {
                    target.OnActivated(ordered);
                }
                else
                {
                    Inactivate();
                }
            }
            ordered = false;
        }
    }

    class _MediatorBlocker
    {
        bool previousCond = false;
        List<_AbilityMediator> targets = new List<_AbilityMediator>();
        System.Func<bool> condf;

        public _MediatorBlocker(System.Func<bool> condf)
        {
            this.condf = condf;
        }

        public void AddTarget(_AbilityMediator item)
        {
            targets.Add(item);
        }

        public void Start()
        {
            previousCond = condf();
            if (!previousCond)
            {
                targets.ForEach(t => t.Block(true));
            }
        }

        public void Update()
        {
            bool cond = condf();
            if (!previousCond && cond)
            {
                targets.ForEach(t => t.Block(false));
            }
            if (previousCond && !cond)
            {
                targets.ForEach(t => t.Block(true));
            }
            previousCond = cond;
        }
    }

    [SerializeField] AI ai;
    [SerializeField,UnityEngine.Serialization.FormerlySerializedAs("BaseAbilitySet")] AbilitySet baseAbilitySet;

    Ability currentAbility = null;
    Dictionary<System.Type, _AbilityMediator> allAbilitiesMediator = new Dictionary<System.Type, _AbilityMediator>();
    HashSet<System.Type> currentDecisions = new HashSet<System.Type>();
    List<_MediatorBlocker> mediatorBlockers = null; 

    private void Awake()
    {
        baseAbilitySet.Validate();
    }

    private void Start()
    {
        //availableAbilities = baseAbilitySet.GetAvailableAbilities();

        foreach(Ability a in baseAbilitySet.GetAbilitiesAll()) 
        {
            allAbilitiesMediator.Add(a.GetType(), new _AbilityMediator(a));
        }

        foreach(var abim in allAbilitiesMediator.Values)
        {
            abim.ConstructPriorTo(allAbilitiesMediator);
        }

        mediatorBlockers = baseAbilitySet.ConstructMediatorBlocker(allAbilitiesMediator);
        mediatorBlockers.ForEach(mb => mb.Start());
        //baseAbilitySet._RegisterUpdateAvilableCallback(UpdateAvailableAbilities);
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        baseAbilitySet.Validate();
        baseAbilitySet.whenStateIs = "Base";
    }

    private void Update()
    {
        currentDecisions.Clear();
        ai.Decide(currentDecisions);

        mediatorBlockers.ForEach(mb => mb.Update());

        foreach(var ap in allAbilitiesMediator)
        {
            if (currentDecisions.Contains(ap.Key))
            {
                ap.Value.Order();
            }
            ap.Value.Update();
        }
    }
    /*
    public void UpdateAvailableAbilities()
    {
        //availableAbilities = baseAbilitySet.GetAvailableAbilities();
    }
    */
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ActorBehaviours))]
    public class EditorModify : Editor
    {
        SkillSetViewer defaultssv;
        private void OnEnable()
        {
            defaultssv = new SkillSetViewer((target as ActorBehaviours).baseAbilitySet);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ActorBehaviours actorBehaviours = target as ActorBehaviours;

            Undo.RecordObject(actorBehaviours,"Actor Behaviours");
            //PropertyField(serializedObject.FindProperty("ai"));
            actorBehaviours.ai = (AI)ObjectField("AI", actorBehaviours.ai, typeof(AI));
            LabelField("Default Skill Set");
            defaultssv.Call();
        }

        class SkillSetViewer
        {
            //EditorModify owner;
            AbilitySet target;
            bool selfFold;
            bool skillsRootFold;
            bool subsFold;
            List<SkillSetViewer> subs;

            public SkillSetViewer(AbilitySet target) {
                //this.owner = owner;
                this.target = target;
                this.subs = new List<SkillSetViewer>();
                if (target != null && target.subAbilitySets.Count > 0)
                {
                    foreach(var ss in target.subAbilitySets)
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
                            for (int i = 0; i < target.abilities.Count; ++i)
                            {
                                using (new HorizontalScope())
                                {
                                    target.abilities[i] = ObjectField(target.abilities[i], typeof(Ability), true) as Ability;
                                    if (GUILayout.Button("Remove"))
                                    {
                                        target.abilities.RemoveAt(i);
                                    };
                                }
                            }
                            if (GUILayout.Button("Add New Skill")) {
                                target.abilities.Add(default);
                            };
                        }
                        Space();
                        
                        if (subsFold = Foldout(subsFold, "Sub Skill Sets"))
                        {
                            target.dependentStateManager = ObjectField("Dependent State Manager", target.dependentStateManager, typeof(StateManager2), true) as StateManager2;
                            for (int i = 0; i < target.subAbilitySets.Count; ++i)
                            {
                                subs[i].Call();
                                using (new HorizontalScope())
                                {
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button("Remove"))
                                    {
                                        target.subAbilitySets.RemoveAt(i);
                                        subs.RemoveAt(i);
                                    };
                                }
                            }
                            if (GUILayout.Button("Add New Sub Skill Set"))
                            {
                                AbilitySet newSkillSet = new AbilitySet();
                                newSkillSet.Validate();
                                target.subAbilitySets.Add(newSkillSet);
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
