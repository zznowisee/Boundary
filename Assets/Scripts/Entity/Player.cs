using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Entity
{

    public new event Action<Direction> OnMoveStart;
    public new event Action OnMoveEnd;

    private void Update()
    {
        HandleInput();
    }

    public override void Setup(Unit initUnit_)
    {
        base.Setup(initUnit_);
        OnMoveEnd += SquareManager.Instance.PlayerFinishMove;
    }

    public override IEnumerator MoveToTarget(Direction direction)
    {
        state = State.Moving;
        Unit target = MapManager.Instance[currentUnit.index + direction.GetValue()];
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;
        LeaveCurrentUnit();
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        state = State.Idle;
        OnMoveEnd?.Invoke();

        EnterNewUnit(target);
    }

    private void HandleInput()
    {
        if (state == State.Moving)
            return;

        if (Input.GetKeyDown(KeyCode.W)) MoveTo(Direction.UP);
        else if (Input.GetKeyDown(KeyCode.S)) MoveTo(Direction.DOWN);
        else if (Input.GetKeyDown(KeyCode.A)) MoveTo(Direction.LEFT);
        else if (Input.GetKeyDown(KeyCode.D)) MoveTo(Direction.RIGHT);
    }

    public override void MoveTo(Direction direction)
    {
        if (CanMove(direction))
        {
            StartCoroutine(MoveToTarget(direction));
        }
    }

    public override bool CanMove(Direction direction)
    {
        Unit target = MapManager.Instance[direction.GetValue() + currentUnit.index];
        if (target == null) return false;

        Vector2Int boundaryCheckIndex = InputHelper.GetBoundaryCheckIndex(direction, currentUnit.index);
        List<Square> squares = SquareManager.Instance.GetSolidBoundarySquares(boundaryCheckIndex);
        List<Square> canMoveSquareList = new List<Square>();
        //û��square
        if (squares.Count == 0)
        {
            //print("Null squares, No need to check boundary");
            if (target.IsEmpty())
            {
                //print(target.index + " :target is empty");
                return true;
            }
            else
            {
                //print(target.index + ": target is not empty");
                return false;
            }
        }
        else
        {
            foreach(var square in squares)
            {
                if(square.moveState == MoveState.CAN)
                {
                    continue;
                }
                else if(square.moveState == MoveState.CANNOT)
                {
                    return false;
                }
                else
                {
                    if (!square.CanMove(direction))
                    {
                        square.moveState = MoveState.CANNOT;
                        return false;
                    }
                    else
                    {
                        square.moveState = MoveState.CAN;
                        canMoveSquareList.Add(square);
                    }
                }
            }
        }
        canMoveSquareList.ForEach(square => square.MoveTo(direction));
        return true;
    }
}
