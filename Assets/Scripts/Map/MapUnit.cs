using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUnit : MonoBehaviour
{
    [HideInInspector] public Vector2Int unitIndex;
    public Entity currentEntity;

    private SpriteRenderer spriteRenderer;
    private Color defaultCol;
    private bool active;
    private bool selected;

    public bool Selected
    {
        get { return selected; }
    }

    public bool Active
    {
        get { return active; }
    }

    public void Setup(Vector2Int index_, Color defaultCol_)
    {
        active = true;
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

    public void SetActive()
    {
        active = !active;
        CancelSelect();
        spriteRenderer.enabled = active;
    }
}
