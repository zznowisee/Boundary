using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct BoundaryInfo
{
    public Vector2Int minIndex;
    public Vector2Int maxIndex;
    public Direction direction;
    public BoundaryType boundaryType;
}

public class SquareBoundary : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material dottedMat;
    [SerializeField] private Material solidMat;
    [HideInInspector] public Square square;

    [HideInInspector] public SquareUnit squareUnit;
    [HideInInspector] public Vector2Int pointsIndex { get { return boundaryInfo.minIndex + boundaryInfo.maxIndex; } }
    public BoundaryInfo boundaryInfo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Square square_, Direction direction_, Color color_, SquareUnit squareUnit_, Vector2Int minIndex_, Vector2Int maxIndex_)
    {
        square = square_;
        boundaryInfo.direction = direction_;
        spriteRenderer.color = color_;
        squareUnit = squareUnit_;
        boundaryInfo.boundaryType = BoundaryType.SOLID;
        SetBoundaryVisual();
        square.OnMove += (Direction dir) =>
        {
            boundaryInfo.minIndex += dir.GetValue();
            boundaryInfo.maxIndex += dir.GetValue();
        };
        boundaryInfo = new BoundaryInfo() { boundaryType = BoundaryType.SOLID, direction = direction_, maxIndex = maxIndex_, minIndex = minIndex_ };
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

    public void UpdateBoundaryType()
    {
        Unit current = MapManager.Instance[squareUnit.mapUnitIndex];
        Unit another = MapManager.Instance[squareUnit.mapUnitIndex + boundaryInfo.direction.GetValue()];



        //p0 p1上是否有别的边界
        //p0 p1是否在别的边界内
        SetBoundaryVisual();
    }

    public bool CanMove(Direction direction_)
    {
        //如果是虚线 不需要判定 直接return true
        if (boundaryInfo.boundaryType == BoundaryType.DOTTED)
            return true;
        Vector2Int checkIndex = squareUnit.mapUnitIndex;

        if (boundaryInfo.direction == Direction.LEFT && direction_ == Direction.LEFT) checkIndex += Vector2Int.left;
        else if (boundaryInfo.direction == Direction.RIGHT && direction_ == Direction.RIGHT) checkIndex += Vector2Int.right;
        else if (boundaryInfo.direction == Direction.DOWN && direction_ == Direction.DOWN) checkIndex += Vector2Int.down;
        else if (boundaryInfo.direction == Direction.UP && direction_ == Direction.UP) checkIndex += Vector2Int.up;

        Unit checkUnit = MapManager.Instance[checkIndex];
        //如果已经到达地图边界， 是否会存在从0进入 会从 x最大的点 出来的情况
        if (checkUnit == null)
        {
            print("Check Unit Is Null");
            return false;
        }
        else
        {
            if (!checkUnit.IsEmpty())
            {
                return checkUnit.currentEntity.CanMove(direction_);
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
}

public struct Point2Index
{
    public Vector2Int p0;
    public Vector2Int p1;
}
