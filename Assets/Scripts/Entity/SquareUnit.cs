using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareUnit : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public Square square;
    public Vector2Int unitIndex;
    [HideInInspector] public SquareUnitType squareUnitType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Square square_, Vector2Int index_, Color squareUnitCol_)
    {
        square = square_;
        unitIndex = index_;
        square.OnMoveStart += (Direction dir) => unitIndex += dir.GetValue();
        squareUnitCol_.a = .1f;
        spriteRenderer.color = squareUnitCol_;
    }

    public bool FindBox()
    {
        MapUnit unit = MapManager.Instance[unitIndex];
        if(unit.currentEntity != null)
        {
            return unit.currentEntity as Box != null;
        }
        return false;
    }
}