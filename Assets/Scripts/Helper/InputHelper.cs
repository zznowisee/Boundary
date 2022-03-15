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
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.up, float.MaxValue);
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

    public static Unit GetUnitInWorldPosition(Vector3 position)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(position, Vector3.up, float.MaxValue);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider)
            {
                Unit unit = hit[i].collider.GetComponent<Unit>();
                if (unit)
                {
                    return unit;
                }
            }
        }
        return null;
    }


    public static SquareUnit[] GetSquareUnitsInWorldPosition(Vector3 position)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.up, float.MaxValue);
        List<SquareUnit> squares = new List<SquareUnit>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                SquareUnit su = hits[i].collider.gameObject.GetComponent<SquareUnit>();
                if (su)
                {
                    squares.Add(su);
                }
            }
        }

        return squares.ToArray();
    }


}
