using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Square : MonoBehaviour
{
    private Point lowerLeftPoint;
    private Vector2Int squareSize;
    private List<SquareUnit> squareUnits;
    private List<Box> boxes;
    private Transform squareUnitsHolder;
    private Transform squareBoundariesHolder;
    [SerializeField] private Color color;
    [SerializeField] private SquareBoundary pfSquareBoundary;
    [SerializeField] private SquareUnit pfSquareUnit;

    public event Action<Direction> OnMove;

    private void Awake()
    {
        squareUnitsHolder = transform.Find("squareUnitsHolder");
        squareBoundariesHolder = transform.Find("squareBoundariesHolder");
        squareUnits = new List<SquareUnit>();
        boxes = new List<Box>();
    }

    private void OnEnable() => SquareManager.Instance.squaresList.Add(this);
    private void OnDisable() => SquareManager.Instance.squaresList.Remove(this);
    public void AddNewBox(Box box) => boxes.Add(box);
    public void ClearBoxes() => boxes.Clear();
    public void Setup(Vector2Int squareSize_, Point lowerLeftPoint_)
    {
        squareSize = squareSize_;
        lowerLeftPoint = lowerLeftPoint_;

        List<Unit> checkedUnits = new List<Unit>();
        CalculateBoundary(lowerLeftPoint.unit, GetUnitsList(), checkedUnits);
    }

    List<Unit> GetUnitsList()
    {
        List<Unit> units = new List<Unit>();
        Vector2Int maxIndex = lowerLeftPoint.index + squareSize - Vector2Int.one;
        for (int y = lowerLeftPoint.index.y; y <= maxIndex.y; y++)
        {
            for (int x = lowerLeftPoint.index.x; x <= maxIndex.x; x++)
            {
                Unit unit = MapManager.Instance.GetUnit(x, y);
                if (unit) units.Add(unit);
            }
        }
        return units;
    }

    void CheckPoint(Unit baseUnit, Unit checkUnit, List<Unit> squareUnits, List<Unit> checkedUnits, Direction direction, SquareUnit su)
    {
        Grid<Point> map = MapManager.Instance.map;

        if (!squareUnits.Contains(checkUnit))
        {
            Point p0 = null, p1 = null;
            switch (direction)
            {
                case Direction.UP:
                    p0 = map[baseUnit.index + Vector2Int.up];
                    p1 = map[baseUnit.index + Vector2Int.one];
                    break;
                case Direction.DOWN:
                    p0 = map[baseUnit.index];
                    p1 = map[baseUnit.index + Vector2Int.right];
                    break;
                case Direction.LEFT:
                    p0 = map[baseUnit.index];
                    p1 = map[baseUnit.index + Vector2Int.up];
                    break;
                case Direction.RIGHT:
                    p0 = map[baseUnit.index + Vector2Int.right];
                    p1 = map[baseUnit.index + Vector2Int.one];
                    break;
            }

            SpawnBoundary(p0, p1, su);
        }
        else
        {
            if (!checkedUnits.Contains(checkUnit))
            {
                CalculateBoundary(checkUnit, squareUnits, checkedUnits);
            }
        }
    }

    void CalculateBoundary(Unit current, List<Unit> units, List<Unit> checkedUnits)
    {
        SquareUnit su = Instantiate(pfSquareUnit, current.transform.position, Quaternion.identity, squareUnitsHolder);
        //su.Setup(this);
        squareUnits.Add(su);
        checkedUnits.Add(current);

        Unit up = MapManager.Instance.GetUnit(current.index + Vector2Int.up);
        Unit down = MapManager.Instance.GetUnit(current.index + Vector2Int.down);
        Unit left = MapManager.Instance.GetUnit(current.index + Vector2Int.left);
        Unit right = MapManager.Instance.GetUnit(current.index + Vector2Int.right);

        CheckPoint(current, up, units, checkedUnits, Direction.UP, su);
        CheckPoint(current, down, units, checkedUnits, Direction.DOWN, su);
        CheckPoint(current, left, units, checkedUnits, Direction.LEFT, su);
        CheckPoint(current, right, units, checkedUnits, Direction.RIGHT, su);
    }

    void SpawnBoundary(Point p0, Point p1, SquareUnit su)
    {
        Debug.DrawLine(p0.position, p1.position, Color.white, 100f);
        BoundaryDirection boundaryDirection = p0.index.y == p1.index.y ? BoundaryDirection.HORIZONTAL : BoundaryDirection.VERTICAL;
        SquareBoundary sb = Instantiate(pfSquareBoundary, (p0.position + p1.position) / 2f,
                                                           boundaryDirection == BoundaryDirection.VERTICAL ? Quaternion.Euler(new Vector3(0, 0, 90f)) : Quaternion.identity,
                                                           squareBoundariesHolder);
        Direction direction;
        if (boundaryDirection == BoundaryDirection.HORIZONTAL)
            direction = p0.index.x % 2 == 0 ? Direction.LEFT : Direction.RIGHT;
        else
            direction = p0.index.y % 2 == 0 ? Direction.DOWN : Direction.UP;
        su.AddBoundary(sb);
        sb.Setup(direction, color);
    }

    public IEnumerator MoveToTarget(Point target)
    {
        Vector3 start = transform.position;
        Vector3 end = target.unitPosition;
        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / .2f;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        lowerLeftPoint = target;
    }

    public bool CanMove(Direction direction)
    {
        for (int i = 0; i < squareUnits.Count; i++)
        {
            if(squareUnits[i].squareUnitType == SquareUnitType.BOUNDARY)
            {
                if(squareUnits[i].boundaryDirection == BoundaryDirection.BOTH ||
                   squareUnits[i].boundaryDirection == direction.EvaluateDirection())
                {
                    if (!squareUnits[i].CanMove(direction))
                    {
                        return false;
                    }
                }
            }
        }
        MoveTo(direction);
        return true;
    }

    private void MoveTo(Direction direction)
    {
        StartCoroutine(MoveToTarget(direction));
    }

    IEnumerator MoveToTarget(Direction dir)
    {
        Point target = MapManager.Instance.map[lowerLeftPoint.index + dir.GetValue()];

        Vector3 start = transform.position;
        Vector3 end = target.unitPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / .2f;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        lowerLeftPoint = target;
    }
}
