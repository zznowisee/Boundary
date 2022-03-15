using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int index;
    public Entity currentEntity;
    public void Setup(Vector2Int index_)
    {
        this.index = index_;
    }
    public bool IsEmpty() => currentEntity == null;
}
