using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Entity
{

    public override bool CanMove(Direction direction)
    {
        Unit target = MapManager.Instance[direction.GetValue() + currentUnit.index];
        if (target == null)
            return false;

        Vector2Int boundaryCheckIndex = InputHelper.GetBoundaryCheckIndex(direction, currentUnit.index);
        List<Square> squares = SquareManager.Instance.GetSolidBoundarySquares(boundaryCheckIndex);

        if (squares.Count == 0)
        {
            MoveTo(direction);
            return target.IsEmpty();
        }
        else
        {
            foreach(var square in squares)
            {
                if (square.moveState == MoveState.CANNOT || square.moveState == MoveState.NONECHECK)
                {
                    return false;
                }
            }
        }

        MoveTo(direction);
        return true;
    }
}
