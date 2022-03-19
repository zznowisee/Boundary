using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int index;
    [HideInInspector] public RectPoint rectPoint;

    public void Setup(Vector2Int index_, Point p00, Point p10, Point p01, Point p11)
    {
        this.index = index_;
        rectPoint = new RectPoint(p00, p10, p01, p11);
        gameObject.name = $"{index.x}-{index.y}";
    }
    public bool IsEmpty() => InputHelper.IsUnitEmpty(transform.position);
}
