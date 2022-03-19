using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBoundary : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material dottedMat;
    [SerializeField] private Material solidMat;
    [HideInInspector] public Square square;
    public Direction direction;
    [HideInInspector] public BoundaryType boundaryType;
    [HideInInspector] public Vector2Int pointsIndex;
    public Vector2Int squareUnitIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Square square_, Direction direction_, Color color_, Vector2Int squareUnitIndex_, Vector2Int pointsIndex_)
    {
        square = square_;
        direction = direction_;
        spriteRenderer.color = color_;
        boundaryType = BoundaryType.SOLID;
        SetBoundaryVisual();
        squareUnitIndex = squareUnitIndex_;
        pointsIndex = pointsIndex_;
        square.OnMove += (Direction dir) => squareUnitIndex += dir.GetValue();
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

    private void UpdateBoundaryType()
    {

        SetBoundaryVisual();
    }

    public bool CanMove(Direction direction_)
    {
        //如果是虚线 不需要判定 直接return true
        if (boundaryType == BoundaryType.DOTTED)
            return true;
        Vector2Int checkIndex = squareUnitIndex;
        print("Boundary Direction: " + direction);
        print("Move Direction: " + direction_);
        print("before: " + checkIndex);
        if (direction == Direction.LEFT && direction_ == Direction.LEFT) checkIndex += Vector2Int.left;
        else if (direction == Direction.RIGHT && direction_ == Direction.RIGHT) checkIndex += Vector2Int.right;
        else if (direction == Direction.DOWN && direction_ == Direction.DOWN) checkIndex += Vector2Int.down;
        else if (direction == Direction.UP && direction_ == Direction.UP) checkIndex += Vector2Int.up;
        print("after:" + checkIndex);
        Unit checkUnit = MapManager.Instance[checkIndex];
        //如果已经到达地图边界， 是否会存在从0进入 会从 x最大的点 出来的情况
        if (checkUnit == null)
        {
            //print("Check Unit Is Null");
            return false;
        }
        else
        {
            if (!checkUnit.IsEmpty())
            {
                /*                Box box = checkUnit.currentEntity as Box;
                                return box.CanMove(direction);*/
                print(squareUnitIndex + "checking ," + checkUnit.index  +" :Check Unit Is Not Empty");
                return false;
            }
            return true;
        }
    }
}
