using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainingCountOperator : MonoBehaviour
{
    public void ResetRemainingCount() {SotS.ReviveController.Reset();}
    public void AddRemainingCount(int amount){SotS.ReviveController.AddRemainingCount(amount);}
}
