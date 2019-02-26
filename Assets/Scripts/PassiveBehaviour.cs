using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveBehaviour : MonoBehaviour
{
    protected bool nowOnEffect = false;

    public bool NowOnEffect { get => nowOnEffect; }
}
