using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BoundaryInfo
{
    public Vector2Int minIndex;
    public Vector2Int maxIndex;
    public Direction direction;
    public BoundaryType boundaryType;
}

public class SquareBoundary : MonoBehaviour, ISelectable
{
    private Square square;
    private SquareUnit squareUnit;
    private SpriteRenderer spriteRenderer;
    private Color squareCol;
    [SerializeField] private Material dottedMat;
    [SerializeField] private Material solidMat;

    [HideInInspector] public Vector2Int pointsIndex { get { return boundaryInfo.minIndex + boundaryInfo.maxIndex; } }
    public BoundaryInfo boundaryInfo;

    public Square Square
    {
        get { return square; }
    }

    public bool selected { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Square square_, Direction direction_, Color squareCol_, SquareUnit squareUnit_, Vector2Int minIndex_, Vector2Int maxIndex_)
    {
        square = square_;
        boundaryInfo.direction = direction_;
        squareCol = squareCol_;
        spriteRenderer.color = squareCol_;
        squareUnit = squareUnit_;
        boundaryInfo.boundaryType = BoundaryType.SOLID;
        SetBoundaryVisual();
        boundaryInfo = new BoundaryInfo() { boundaryType = BoundaryType.SOLID, direction = direction_, maxIndex = maxIndex_, minIndex = minIndex_ };

        square.OnMoveStart += (Direction dir) => Teleport(dir.GetValue());
    }
    
    public void Teleport(Vector2Int movement)
    {
        boundaryInfo.minIndex += movement;
        boundaryInfo.maxIndex += movement;
    }

    private void SetBoundaryVisual()
    {
        switch (boundaryInfo.boundaryType)
        {
            case BoundaryType.DOTTED:
                spriteRenderer.material = dottedMat;
                break;
            case BoundaryType.SOLID:
                spriteRenderer.material = solidMat;
                break;
        }
    }

    public void SetBoundaryType(BoundaryType boundaryType_)
    {
        boundaryInfo.boundaryType = boundaryType_;
        SetBoundaryVisual();
    }


    public bool CanMove(Direction direction_, List<Entity> canMoveEntityList)
    {
        //如果是虚线 不需要判定 直接return true
        if (boundaryInfo.boundaryType == BoundaryType.DOTTED)
            return true;

        Vector2Int checkIndex = squareUnit.unitIndex;
        if (boundaryInfo.direction == direction_)
            checkIndex += direction_.GetValue();

        MapUnit checkUnit = MapManager.Instance[checkIndex];
        //如果已经到达地图边界， 是否会存在从0进入 会从 x最大的点 出来的情况
        if (checkUnit == null)
        {
            return false;
        }
        else
        {
            if (!checkUnit.IsEmpty())
            {
                if(checkUnit.currentEntity is Player)
                {
                    return true;
                }

                if(checkUnit.currentEntity is Box)
                {
                    Box box = checkUnit.currentEntity as Box;
                    if(box.CanMove(direction_, canMoveEntityList, square))
                    {
                        canMoveEntityList.Add(box);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public Point2Index AdjacentUnitIndex()
    {
        Point2Index points = new Point2Index();
        points.p0 = boundaryInfo.minIndex;
        switch (boundaryInfo.direction)
        {
            case Direction.DOWN: points.p1 = boundaryInfo.minIndex + Vector2Int.down; break;
            case Direction.UP: points.p1 = boundaryInfo.minIndex + Vector2Int.down; break;
            case Direction.LEFT: points.p1 = boundaryInfo.minIndex + Vector2Int.left; break;
            case Direction.RIGHT: points.p1 = boundaryInfo.minIndex + Vector2Int.left; break;
        }
        return points;
    }

    public void LeftClick()
    {
        print($"Click on { square.name }");
        square.Select();
    }

    public void RightClick()
    {
        square.Select();
    }

    public void LeftRelease()
    {
        print("Left release");
        square.CancelSelect();
    }

    public void Dragging()
    {
        MapUnit newUnit = InputHelper.GetMapUnitUnderMouse();
        if (newUnit == null)
            return;

        if (newUnit.unitIndex == squareUnit.unitIndex)
            return;

        square.Teleport(newUnit.unitIndex - squareUnit.unitIndex);
    }

    public void Highlighting()
    {
        spriteRenderer.color = Color.white;
    }

    public void Normal()
    {
        spriteRenderer.color = squareCol;
    }

    public void RightRelease()
    {
        Destroy(square.gameObject);
    }
}

public struct Point2Index
{
    public Vector2Int p0;
    public Vector2Int p1;
}
