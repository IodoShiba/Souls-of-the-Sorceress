using UnityEngine;
using System.Collections;

namespace ActorSarah
{
    public class PlayerInputCommander : PlayerCommander
    {
        [SerializeField] float joyDirectionKeyRadiusThreshold;

        public override void DecideOverride()
        {
            Vector2 joyIn;
            // Directional.Evaluate(joyIn = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            Directional.Evaluate(joyIn = InputDaemon.GetVector2("Move"));
            //Debug.Log($"In:{joyIn}");
            //Debug.Log($"Evaluation Before Update:{Directional.Evaluation}");
            Attack.Evaluate(InputDaemon.IsPressed("Attack"));
            Jump.Evaluate(InputDaemon.IsPressed("Jump"));
            OpenUmbrella.Evaluate(InputDaemon.IsPressed("Open Umbrella"));
            AwakeButton.Evaluate(InputDaemon.IsPressed("Awake"));
            AnalogueUp.Evaluate(joyIn.y > joyDirectionKeyRadiusThreshold);
            AnalogueDown.Evaluate(joyIn.y < -joyDirectionKeyRadiusThreshold);

            //Debug.Log($"Evaluation After Update:{Directional.Evaluation}");
        }
    }
}