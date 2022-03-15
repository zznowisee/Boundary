using UnityEngine;
public enum BoundaryDirection
{
    HORIZONTAL,
    VERTICAL,
    BOTH
}

public enum BoundaryType
{
    SOLID,
    DOTTED
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum MoveState
{
    NULLCHECK = 0,
    CAN,
    CANNOT
}

public static class DirectionExtension
{
    public static Vector2Int GetValue(this Direction direction)
    {
        switch (direction)
        {
            case Direction.DOWN: return Vector2Int.down;
            case Direction.UP: return Vector2Int.up;
            case Direction.LEFT: return Vector2Int.left;
            case Direction.RIGHT: return Vector2Int.right;
        }
        return Vector2Int.zero;
    }

    public static BoundaryDirection EvaluateDirection(this Direction dir)
    {
        BoundaryDirection bd = dir == Direction.DOWN || dir == Direction.UP ? BoundaryDirection.VERTICAL : BoundaryDirection.HORIZONTAL;
        return bd;
    }

    public static Direction Opposite (this Direction direction)
    {
        switch (direction)
        {
            case Direction.DOWN: return Direction.UP;
            case Direction.UP: return Direction.DOWN;
            case Direction.LEFT: return Direction.RIGHT;
            case Direction.RIGHT: return Direction.LEFT;
        }
        return direction;
    }
}

public static class BoundaryDirectionExtension
{
    public static BoundaryDirection EvaluateDirection(this BoundaryDirection bd, Direction dir)
    {
        if (bd == BoundaryDirection.BOTH)
            return bd;

        if (bd == BoundaryDirection.HORIZONTAL)
        {
            if(dir == Direction.DOWN || dir == Direction.UP)
            {
                return BoundaryDirection.BOTH;
            }
            return bd;
        }
        else
        {
            if (dir == Direction.LEFT || dir == Direction.RIGHT)
            {
                return BoundaryDirection.BOTH;
            }
            return bd;
        }
    }
}

public enum SquareUnitType
{
    INNER,
    BOUNDARY
}


