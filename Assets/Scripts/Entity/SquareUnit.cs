using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareUnit : MonoBehaviour
{
    [HideInInspector] public Square square;
    [HideInInspector] public SquareUnitType squareUnitType;
    [HideInInspector] public BoundaryDirection boundaryDirection;
    private Vector2Int index;
    List<SquareBoundary> squareBoundaries;
    public void Setup(Square square_, Vector2Int index_)
    {
        square = square_;
        index = index_;
        square.OnMove += (Direction dir) => index += dir.GetValue();
        squareUnitType = SquareUnitType.INNER;
    }

    public void AddBoundary(SquareBoundary sb)
    {
        squareUnitType = SquareUnitType.BOUNDARY;
        boundaryDirection = boundaryDirection.EvaluateDirection(sb.direction);
        if(squareBoundaries == null)
        {
            squareBoundaries = new List<SquareBoundary>();
        }

        squareBoundaries.Add(sb);
    }

    public bool CanMove(Direction direction)
    {
        Unit unit = MapManager.Instance.GetUnit(direction.GetValue() + index);
        if (unit)
        {
            if (!unit.IsEmpty())
            {
                Box box = unit.currentEntity as Box;
                if (box.CanMove(direction))
                {
                    box.moveState = MoveState.CAN;
                }
            }
        }
        return true;
    }
}