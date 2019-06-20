﻿using UnityEngine;
using System.Collections;

namespace ActorSarah
{
    public class PlayerInputCommander : PlayerCommander
    {
        [SerializeField] float joyDirectionKeyRadiusThreshold;

        public override void Decide()
        {
            Vector2 joyIn;
            Directional.Evaluate(joyIn = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            //Debug.Log($"In:{joyIn}");
            //Debug.Log($"Evaluation Before Update:{Directional.Evaluation}");
            Attack.Evaluate(Input.GetButton("Attack"));
            Jump.Evaluate(Input.GetButton("Jump"));
            OpenUmbrella.Evaluate(Input.GetButton("Open Umbrella"));
            AwakeButton.Evaluate(Input.GetButton("Awake"));
            AnalogueUp.Evaluate(joyIn.y > joyDirectionKeyRadiusThreshold);
            AnalogueDown.Evaluate(joyIn.y < -joyDirectionKeyRadiusThreshold);

            Directional.Update();
            Attack.Update();
            Jump.Update();
            OpenUmbrella.Update();
            AwakeButton.Update();
            AnalogueUp.Update();
            AnalogueDown.Update();
            DownAttackMultiPush.Update();
            //Debug.Log($"Evaluation After Update:{Directional.Evaluation}");
        }
    }
}