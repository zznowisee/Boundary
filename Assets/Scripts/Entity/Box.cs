using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Entity, ISelectable
{
    private Color defaultCol;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultCol = spriteRenderer.color;
    }

    public bool CanMove(Direction direction, List<Entity> canMoveEntityList, Square square)
    {
        print("Running Box - CanMove");
        MapUnit target = MapManager.Instance[direction.GetValue() + anchorUnit.unitIndex];
        if (target == null) return false;
        if (!target.Active) return false;
        Vector2Int boundaryCheckIndex = InputHelper.GetBoundaryCheckIndex(direction, anchorUnit.unitIndex);
        List<Square> squares = SquareManager.Instance.GetSolidBoundarySquares(boundaryCheckIndex);

        if (squares.Count == 0)
        {
            if (target.IsEmpty())
            {
                return true;
            }
            else
            {
                if (!target.Active)
                    return false;

                Box box = target.currentEntity as Box;
                if (box.CanMove(direction, canMoveEntityList, square))
                {
                    canMoveEntityList.Add(target.currentEntity);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            foreach (var s in squares)
            {
                if (s != square)
                    return false;
            }
        }
        return true;
    }

    public void Dragging()
    {
        MapUnit mu = InputHelper.GetMapUnitUnderMousePosition();
        if (mu == null)
            return;
        if (!mu.IsEmpty())
            return;
        if (mu.unitIndex == anchorUnit.unitIndex)
            return;

        Teleport(mu.unitIndex - anchorUnit.unitIndex);
    }

    public void LeftClick()
    {
        spriteRenderer.color = Color.white;
    }

    public void LeftRelease()
    {
        spriteRenderer.color = defaultCol;
    }

    public void RightClick()
    {
        spriteRenderer.color = Color.white;
    }
    public void RightRelease()
    {
        Destroy(gameObject);
    }
}
