using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IodoShiba.Bound2DUtility;

public class BossTitanAI : AI
{
    public enum ActionModes
    {
        Normal,
        Jump,
        Tuckle,
    }

    [SerializeField] float makingDecisionCycle;
    [SerializeField] float centerOfFieldX;
    [SerializeField] float distanceOfJumpUpBorderFromCenter;
    [SerializeField] float passPlatformtime1;
    [SerializeField] float passPlatformtime2;
    [SerializeField] AreaObjectDetector playerDetector;
    [SerializeField] List<BoxCollider2D> areaColliders;

    Vector2Int position;
    ActionModes actionMode;

    float t;
    int moveDirection = 0;
    bool doJump = false;
    bool doPassPlatform;
    int jumpUpRowsCount;
    bool isActing = false;

    public Vector2Int Position { get => position; }
    public ActionModes ActionMode { get => actionMode; }
    public int MoveDirection { get => moveDirection; }
    public bool DoJump { get => doJump; }
    public bool DoPassPlatform { get => doPassPlatform; }
    public int JumpUpRowsCount { get => jumpUpRowsCount; }

    int PositionVectorToAreaIndex(in Vector2Int vector2Int) => vector2Int.x + vector2Int.y * 2;
    bool IsPlayerAndIInSameColumn(int areaIndexPlayerIsIn) => areaIndexPlayerIsIn % 2 == position.x;

    float GetJumpUpBorder(int direction) => centerOfFieldX + direction * distanceOfJumpUpBorderFromCenter;

    Vector2Int WhereAmI()
    {
        int i = 0;
        for(; i < areaColliders.Count; ++i)
        {
            
            Debug.Log(areaColliders[i].bounds);
            if (areaColliders[i].bounds.Contains2D(transform.position))
            {
                break;
            }
            
        }
        Debug.Log($"Final i:{i}");
        return new Vector2Int(i % 2, i / 2);
    }

    private void OnEnable()
    {
        t = 0;
        isActing = false;
        position = WhereAmI();
    }

    public override void Decide()
    {
        Debug.Log(t);
        if (t < makingDecisionCycle || isActing)
        {
            actionMode = ActionModes.Normal;

            if (!isActing)
            {
                t += Time.deltaTime;
            } 
            return;
        }

        isActing = true;

        position = WhereAmI();
        int areaIndexPlayerIsIn = playerDetector.GetDetectingIndex();

        Debug.Log($"Player is in Area {areaIndexPlayerIsIn}");
        Debug.Log($"Found Boss pos : {position}");

        jumpUpRowsCount = areaIndexPlayerIsIn / 2 - position.y;
        moveDirection = areaIndexPlayerIsIn % 2 - position.x;

        if (areaIndexPlayerIsIn != PositionVectorToAreaIndex(position) && IsPlayerAndIInSameColumn(areaIndexPlayerIsIn)) //プレイヤーとボスが異なるエリアかつ同列
        {
            actionMode = ActionModes.Jump;
            StartCoroutine(BranchVerticalMove());
        }
        else
        {
            actionMode = ActionModes.Tuckle;
            if (position.y == 0 && areaIndexPlayerIsIn / 2 == 0)
            {
                StartCoroutine(StateHorizontalMoveEver());
            }
            else
            {
                StartCoroutine(StateHorizontalMove());
            }
        }


        t = 0;
    }



    IEnumerator StateHorizontalMove()
    {
        while((transform.position.x - GetJumpUpBorder(-1 * moveDirection)) * moveDirection < 0)
        {
            yield return null;
        }

        yield return StartCoroutine(BranchVerticalMove());
    }

    IEnumerator BranchVerticalMove()
    {
        if(jumpUpRowsCount >= 0)
        {
            return StateJump();
        }
        else
        {
            return StatePassPlatform();
        }
    }

    IEnumerator StateJump()
    {
        doJump = true;
        yield return new WaitForSeconds(.7f);
        doJump = false;
        if (moveDirection != 0)
        {
            yield return StartCoroutine(StateHorizontalMoveEver());
        }
        else
        {
            isActing = false;
        }
    }

    IEnumerator StatePassPlatform()
    {
        doPassPlatform = true;
        yield return new WaitForSeconds(jumpUpRowsCount == -1 ? passPlatformtime1 : passPlatformtime2);
        doPassPlatform = false;
        if (moveDirection != 0)
        {
            yield return StartCoroutine(StateHorizontalMoveEver());
        }
        else
        {
            isActing = false;
        }
    }

    IEnumerator StateHorizontalMoveEver()
    {
        while (enabled)
        {
            yield return null;
        }
        isActing = false;
        moveDirection = 0;
    }


}
