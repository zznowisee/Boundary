using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputHelper
{
    static Camera mainCam;
    public static Camera MainCam
    {
        get
        {
            if(mainCam == null)
            {
                mainCam = Camera.main;
            }
            return mainCam;
        }
    }

    public static Vector3 MouseWorldPosition => mainCam.ScreenToWorldPoint(Input.mousePosition);
    public static bool IsMouseOverUIObject => EventSystem.current.IsPointerOverGameObject();
    public static Square[] GetSquaresInWorldPosition(Vector3 position)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.forward, float.MaxValue);
        List<Square> squares = new List<Square>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                SquareUnit su = hits[i].collider.gameObject.GetComponent<SquareUnit>();
                if (su)
                {
                    squares.Add(su.square);
                }
            }
        }

        return squares.ToArray();
    }

    public static Vector2Int GetBoundaryIndex(Vector2Int playerIndex, Direction moveDir)
    {
        switch (moveDir)
        {
            case Direction.DOWN: return playerIndex + playerIndex + Vector2Int.right;
            case Direction.UP: return playerIndex + Vector2Int.up + playerIndex + Vector2Int.one;
            case Direction.RIGHT: return playerIndex + Vector2Int.right + playerIndex + Vector2Int.one;
            case Direction.LEFT: return playerIndex + playerIndex + Vector2Int.up;
        }
        return Vector2Int.zero;
    }

    public static Vector2Int GetBoundaryCheckIndex(Direction direction, Vector2Int current)
    {
        Vector2Int checkIndex = Vector2Int.zero;
        switch (direction)
        {
            case Direction.DOWN: checkIndex = current * 2 + Vector2Int.right; break;
            case Direction.UP: checkIndex = current * 2 + Vector2Int.up + Vector2Int.one; break;
            case Direction.LEFT: checkIndex = current * 2 + Vector2Int.up; break;
            case Direction.RIGHT: checkIndex = current * 2 + Vector2Int.right + Vector2Int.one; break;
        }
        return checkIndex;
    }

    public static bool IsUnitEmpty(Vector3 position)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                Box box = hits[i].collider.GetComponent<Box>();
                if (box)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
