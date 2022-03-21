using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int index;
    [HideInInspector] public Entity currentEntity;
    [HideInInspector] public List<SquareUnit> squareUnits;

    public void Setup(Vector2Int index_)
    {
        this.index = index_;
        gameObject.name = $"{index.x}-{index.y}";
    }

    public bool IsEmpty() => currentEntity == null;
}
