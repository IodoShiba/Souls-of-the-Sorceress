using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputName
{
    public static class Axis
    {
        public const string horizontal = "Horizontal";
        public const string vertical = "Vertical";
    }
    public static class Button
    {
        public const string attack = "Attack";
        public const string magicalAttack = "Magical Attack";
        public const string openUmbrella = "Open Umbrella";
        public const string jump = "Jump";
        public const string awake = "Awake";
        public const string submit = "Submit";
        public const string cancel = "Cancel";
    }
}

public static class SceneName
{
    public const string title = "Title";
    public const string modeSelect = "";
    public const string talking = "talking_scene";

    public const string stage0 = "stage0";
    public const string stage1Tutorial = "stage1_tutorial";
    public const string stage1 = "stage1";
    public const string boss1 = "1-boss";
}

public static class TagName
{
    public const string player = "Player";
    public const string attack = "Attack";
    public const string ground = "Ground";
}

public static class LayerName
{
    public const string ground = "Ground";
    public const string invincibleActor = "Invincible Actor";
    public const string uncrossActor = "Uncross Actor";
}