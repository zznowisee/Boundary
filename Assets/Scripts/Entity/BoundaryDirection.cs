using UnityEngine;

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
public enum SquareUnitType
{
    INNER,
    OUTTER
}

public enum MoveState
{
    NONECHECK = 0,
    CAN,
    CANNOT
}

public enum DirectionType
{
    HORIZONTAL,
    VERTICAL
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


