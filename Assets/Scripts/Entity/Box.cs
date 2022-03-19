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
        List<SquareBoundary> squareBoundaries = BoundaryManager.Instance.GetSolidBoundaries(boundaryCheckIndex);
        List<Square> squares = new List<Square>();
        squareBoundaries.ForEach(sb => squares.Add(sb.square));

        if(squares.Count == 0)
        {
            return target.IsEmpty();
        }
        else
        {
            foreach (var square in squares)
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
                    }
                }
            }
        }

        return true;
    }
}
