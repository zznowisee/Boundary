using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareUnit : MonoBehaviour
{
    [HideInInspector] public Square square;
    [HideInInspector] public SquareUnitType squareUnitType;
    [HideInInspector] public Vector2Int unitIndex;

    public void Setup(Square square_, Vector2Int index_)
    {
        square = square_;
        unitIndex = index_;
        square.OnMove += (Direction dir) => unitIndex += dir.GetValue();
        squareUnitType = SquareUnitType.INNER;
    }
}