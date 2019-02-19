using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerForStandings : StateManager2
{
    [Tooltip("This state manager initially have two state 'On ground' and 'Flying'. You must not change states.")]
    [SerializeField] GroundSensor groundSensor;

    private void Start()
    {
        this.ConnectState(0)
                     .To(1, () => { return !groundSensor.IsOnGround; })

            .ConnectState(1)
                     .To(0, () => { return groundSensor.IsOnGround; });

        EndDefineAll();
        Activate(0);
    }

    [ContextMenu("Construct States")]
    void ConstructStates()
    {
        activated = false;
        this.name = "Behaviour States";
        states = new List<State>();
        states.Add(new State(0, "On ground"));
        states.Add(new State(1, "Flying"));
    }
    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            this.Execute();
        }
    }
}
