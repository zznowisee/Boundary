using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBoundary : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material dottedMat;
    [SerializeField] private Material solidMat;
    [HideInInspector] public Square square;
    [HideInInspector] public Direction direction;
    [HideInInspector] public BoundaryType boundaryType;
    [HideInInspector] public Vector2Int pointsIndex;
    [HideInInspector] public Vector2Int squareUnitIndex;

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
        //��������� ����Ҫ�ж� ֱ��return true
        if (boundaryType == BoundaryType.DOTTED)
            return true;
        Vector2Int checkIndex = squareUnitIndex;
        print(checkIndex);
        if (direction == Direction.LEFT && direction_ == Direction.LEFT) checkIndex += Vector2Int.left;
        else if (direction == Direction.RIGHT && direction_ == Direction.RIGHT) checkIndex += Vector2Int.right;
        else if (direction == Direction.DOWN && direction_ == Direction.DOWN) checkIndex += Vector2Int.down;
        else if (direction == Direction.UP && direction_ == Direction.UP) checkIndex += Vector2Int.up;
        print(checkIndex);
        Unit checkUnit = MapManager.Instance[checkIndex];
        //����Ѿ������ͼ�߽磬 �Ƿ����ڴ�0���� ��� x���ĵ� ���������
        if (checkUnit == null)
        {
            print("Check Unit Is Null");
            return false;
        }
        else
        {
            if (!checkUnit.IsEmpty())
            {
                /*                Box box = checkUnit.currentEntity as Box;
                                return box.CanMove(direction);*/
                print("Check Unit Is Not Empty");
                return false;
            }
            return true;
        }
    }
}
