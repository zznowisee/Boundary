using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUnit : MonoBehaviour
{
    [HideInInspector] public Vector2Int unitIndex;
    public Entity currentEntity;
    public bool Selected
    {
        get { return selected; }
    }

    private SpriteRenderer spriteRenderer;
    private bool selected;
    private Color defaultCol;

    public void Setup(Vector2Int index_, Color defaultCol_)
    {
        unitIndex = index_;
        defaultCol = defaultCol_;
        gameObject.name = $"{unitIndex.x}-{unitIndex.y}";
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultCol;
    }

    public bool IsEmpty() => currentEntity == null;

    public void Select(Color squareCol)
    {
        selected = true;
        spriteRenderer.color = squareCol;
    }

    public void CancelSelect()
    {
        selected = false;
        spriteRenderer.color = defaultCol;
    }
}
