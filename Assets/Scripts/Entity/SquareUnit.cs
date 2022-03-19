using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareUnit : MonoBehaviour
{
    [HideInInspector] public Square square;
    [HideInInspector] public Vector2Int mapUnitIndex;
    [HideInInspector] public SquareUnitType squareUnitType;

    public void Setup(Square square_, Vector2Int index_)
    {
        square = square_;
        mapUnitIndex = index_;
        square.OnMove += (Direction dir) => mapUnitIndex += dir.GetValue();
    }

    public bool FindBox()
    {
        Unit unit = MapManager.Instance[mapUnitIndex];
        if(unit.currentEntity != null)
        {
            return unit.currentEntity as Box != null;
        }
        return false;
    }
}