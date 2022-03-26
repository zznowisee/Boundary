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

    public static Vector3 MouseWorldPosition => MainCam.ScreenToWorldPoint(Input.mousePosition);
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

    public static MapUnit GetMapUnitUnderMouse()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPosition, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                MapUnit unit = hits[i].collider.GetComponent<MapUnit>();
                if (unit)
                {
                    return unit;
                }
            }
        }
        return null;
    }

    public static Square GetSquareUnderWorldPosition()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPosition, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                SquareBoundary sb = hits[i].collider.GetComponent<SquareBoundary>();
                if (sb)
                {
                    return sb.Square;
                }
            }
        }
        return null;
    }

    public static bool IsTheseKeysDown(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsTheseKeysHeld(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                return true;
            }
        }
        return false;
    }
}
