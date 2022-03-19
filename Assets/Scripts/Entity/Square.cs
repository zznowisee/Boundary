using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Square : Entity
{
    private Unit lowerLeftUnit;
    private Vector2Int squareSize;
    private Transform squareUnitsHolder;
    private Transform squareBoundariesHolder;
    [SerializeField] private Color color;
    [SerializeField] private SquareBoundary pfSquareBoundary;
    [SerializeField] private SquareUnit pfSquareUnit;
    [HideInInspector] public List<SquareBoundary> squareBoundaries;

    public event Action<Direction> OnMove;

    private void Awake()
    {
        squareUnitsHolder = transform.Find("squareUnitsHolder");
        squareBoundariesHolder = transform.Find("squareBoundariesHolder");
        squareBoundaries = new List<SquareBoundary>();
    }

    private void OnEnable() => BoundaryManager.Instance.squares.Add(this);
    private void OnDisable() => BoundaryManager.Instance.squares.Remove(this);

    public void Setup(Vector2Int squareSize_, Unit lowerLeftUnit_)
    {
        squareSize = squareSize_;
        lowerLeftUnit = lowerLeftUnit_;
        transform.position = lowerLeftUnit.transform.position;
        List<Unit> checkedUnits = new List<Unit>();
        CalculateBoundary(lowerLeftUnit, GetUnitsList(), checkedUnits);
    }

    private List<Unit> GetUnitsList()
    {
        List<Unit> units = new List<Unit>();
        Vector2Int maxIndex = lowerLeftUnit.index + squareSize - Vector2Int.one;
        for (int y = lowerLeftUnit.index.y; y <= maxIndex.y; y++)
        {
            for (int x = lowerLeftUnit.index.x; x <= maxIndex.x; x++)
            {
                Unit unit = MapManager.Instance[x, y];
                if (unit) units.Add(unit);
            }
        }
        return units;
    }

    private void CheckPoint(Unit baseUnit, Unit checkUnit, List<Unit> squareUnits, List<Unit> checkedUnits, Direction direction, SquareUnit su)
    {
        Lattice<Point> map = MapManager.Instance.map;

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

            SpawnBoundary(p0, p1, su.unitIndex, direction);
        }
        else
        {
            if (!checkedUnits.Contains(checkUnit))
            {
                CalculateBoundary(checkUnit, squareUnits, checkedUnits);
            }
        }
    }

    private void CalculateBoundary(Unit current, List<Unit> units, List<Unit> checkedUnits)
    {
        SquareUnit su = Instantiate(pfSquareUnit, current.transform.position, Quaternion.identity, squareUnitsHolder);
        su.Setup(this, current.index);
        checkedUnits.Add(current);

        Unit up = MapManager.Instance[current.index + Vector2Int.up];
        Unit down = MapManager.Instance[current.index + Vector2Int.down];
        Unit left = MapManager.Instance[current.index + Vector2Int.left];
        Unit right = MapManager.Instance[current.index + Vector2Int.right];

        CheckPoint(current, up, units, checkedUnits, Direction.UP, su);
        CheckPoint(current, down, units, checkedUnits, Direction.DOWN, su);
        CheckPoint(current, left, units, checkedUnits, Direction.LEFT, su);
        CheckPoint(current, right, units, checkedUnits, Direction.RIGHT, su);
    }

    private void SpawnBoundary(Point p0, Point p1, Vector2Int squareUnitIndex, Direction direction)
    {
        SquareBoundary squareBoundary = Instantiate(pfSquareBoundary, (p0.position + p1.position) / 2f,
                                                           p0.index.y != p1.index.y ? Quaternion.Euler(new Vector3(0, 0, 90f)) : Quaternion.identity,
                                                           squareBoundariesHolder);
        Vector2Int pointsIndex = p0.index + p1.index;

        squareBoundary.Setup(this, direction, color, squareUnitIndex, pointsIndex);
        squareBoundaries.Add(squareBoundary);
    }

    public override bool CanMove(Direction direction)
    {
        foreach(var boundary in squareBoundaries)
        {
            if (boundary.direction.GetDirectionType() != direction.GetDirectionType())
                continue;

            if (!boundary.CanMove(direction))
            {
                moveState = MoveState.CANNOT;
                return false;
            }
        }

        moveState = MoveState.CAN;
        return true;
    }

    public override void MoveTo(Direction direction)
    {
        StartCoroutine(MoveToTarget(direction));
    }

    public override IEnumerator MoveToTarget(Direction direction)
    {
        state = State.Moving;
        Unit target = MapManager.Instance[lowerLeftUnit.index + direction.GetValue()];
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;

        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / moveTime;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        state = State.Idle;
        lowerLeftUnit = target;
        OnMove?.Invoke(direction);
    }
}
