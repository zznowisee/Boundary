using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoundaryInfo
{
    public Direction direction;
    public Vector2Int pointsIndex;
    public BoundaryType boundaryType;
}

public class SquareBoundary : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material dottedMat;
    [SerializeField] private Material solidMat;
    [HideInInspector] public Square square;

    [HideInInspector] public SquareUnit squareUnit;
    [HideInInspector] public Direction direction;

    [HideInInspector] public BoundaryType boundaryType;
    [HideInInspector] public Vector2Int pointsIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Square square_, Direction direction_, Color color_, SquareUnit squareUnit_, Vector2Int pointsIndex_)
    {
        square = square_;
        direction = direction_;
        spriteRenderer.color = color_;
        squareUnit = squareUnit_;
        boundaryType = BoundaryType.SOLID;
        SetBoundaryVisual(); 
        pointsIndex = pointsIndex_;
        square.OnMove += (Direction dir) => pointsIndex += dir.GetValue() * 2;
    }
    
    private void SetBoundaryVisual()
    {
        switch (boundaryType)
        {
            case BoundaryType.DOTTED:
                spriteRenderer.material = dottedMat;
                break;
            case BoundaryType.SOLID:
                spriteRenderer.material = solidMat;
                break;
        }
    }

    public void UpdateBoundaryType()
    {
        Unit current = MapManager.Instance[squareUnit.mapUnitIndex];
        Unit another = MapManager.Instance[squareUnit.mapUnitIndex + direction.GetValue()];



        //p0 p1上是否有别的边界
        //p0 p1是否在别的边界内
        SetBoundaryVisual();
    }

    public bool CanMove(Direction direction_)
    {
        //如果是虚线 不需要判定 直接return true
        if (boundaryType == BoundaryType.DOTTED)
            return true;
        Vector2Int checkIndex = squareUnit.mapUnitIndex;

        if (direction == Direction.LEFT && direction_ == Direction.LEFT) checkIndex += Vector2Int.left;
        else if (direction == Direction.RIGHT && direction_ == Direction.RIGHT) checkIndex += Vector2Int.right;
        else if (direction == Direction.DOWN && direction_ == Direction.DOWN) checkIndex += Vector2Int.down;
        else if (direction == Direction.UP && direction_ == Direction.UP) checkIndex += Vector2Int.up;

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
}
