using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Square : Entity
{

    public int boxNumber;

    private Unit lowerLeftUnit;
    private Vector2Int squareSize;
    private Transform squareUnitsHolder;
    private Transform squareBoundariesHolder;

    private List<Vector2Int> pointsList;
    private List<Vector2Int> unitsList;

    [SerializeField] private Color color;
    [SerializeField] private SquareBoundary pfSquareBoundary;
    [SerializeField] private SquareUnit pfSquareUnit;

    [HideInInspector] public List<SquareBoundary> squareBoundaries;
    [HideInInspector] public List<SquareUnit> squareUnits;
    public SquareUnit this[Vector2Int index] => GetSquareUnit(index); 

    public event Action<Direction> OnMove;

    private void Awake()
    {
        squareUnitsHolder = transform.Find("squareUnitsHolder");
        squareBoundariesHolder = transform.Find("squareBoundariesHolder");
        squareBoundaries = new List<SquareBoundary>();
        squareUnits = new List<SquareUnit>();
        pointsList = new List<Vector2Int>();
        unitsList = new List<Vector2Int>();
    }

    private void OnEnable() => SquareManager.Instance.squaresList.Add(this);
    private void OnDisable() => SquareManager.Instance.squaresList.Remove(this);

    private SquareUnit GetSquareUnit(Vector2Int index)
    {
        for (int i = 0; i < squareUnits.Count; i++)
        {
            if(squareUnits[i].mapUnitIndex == index)
            {
                return squareUnits[i];
            }
        }
        return null;
    }

    public void Setup(Vector2Int squareSize_, Unit lowerLeftUnit_)
    {
        squareSize = squareSize_;
        lowerLeftUnit = lowerLeftUnit_;
        transform.position = lowerLeftUnit.transform.position;
        gameObject.name = squareSize_.ToString();
        List<Unit> checkedUnits = new List<Unit>();
        CalculateBoundary(lowerLeftUnit, GetUnitsList(), checkedUnits);

        OnMove += UpdatePointsUnitsList;
    }

    void UpdatePointsUnitsList(Direction dir)
    {
        for (int i = 0; i < unitsList.Count; i++)
            unitsList[i] += dir.GetValue();
        for (int i = 0; i < pointsList.Count; i++)
            pointsList[i] += dir.GetValue();
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
            //refac
            su.squareUnitType = SquareUnitType.OUTTER;
            SpawnBoundary(su, p0, p1, direction);
        }
        else
        {
            if (!checkedUnits.Contains(checkUnit))
            {
                CalculateBoundary(checkUnit, squareUnits, checkedUnits);
            }
        }
    }

    SquareUnit AddNewSquareUnit(Unit unit)
    {
        SquareUnit su = Instantiate(pfSquareUnit, unit.transform.position, Quaternion.identity, squareUnitsHolder);
        su.Setup(this, unit.index);
        Vector2Int[] indices = new Vector2Int[] 
        { 
            unit.index, 
            unit.index + Vector2Int.right, 
            unit.index + Vector2Int.up, 
            unit.index + Vector2Int.one 
        };

        unitsList.Add(unit.index);

        for (int i = 0; i < indices.Length; i++)
            if (!pointsList.Contains(indices[i]))
                pointsList.Add(indices[i]);
        su.squareUnitType = SquareUnitType.INNER;
        return su;
    }

    private void CalculateBoundary(Unit current, List<Unit> units, List<Unit> checkedUnits)
    {
        checkedUnits.Add(current);

        SquareUnit su = AddNewSquareUnit(current);
        squareUnits.Add(su);

        Unit up = MapManager.Instance[current.index + Vector2Int.up];
        Unit down = MapManager.Instance[current.index + Vector2Int.down];
        Unit left = MapManager.Instance[current.index + Vector2Int.left];
        Unit right = MapManager.Instance[current.index + Vector2Int.right];

        CheckPoint(current, up, units, checkedUnits, Direction.UP, su);
        CheckPoint(current, down, units, checkedUnits, Direction.DOWN, su);
        CheckPoint(current, left, units, checkedUnits, Direction.LEFT, su);
        CheckPoint(current, right, units, checkedUnits, Direction.RIGHT, su);
    }

    private void SpawnBoundary(SquareUnit su, Point p0, Point p1, Direction direction)
    {
        su.squareUnitType = SquareUnitType.OUTTER;

        SquareBoundary squareBoundary = Instantiate(pfSquareBoundary, (p0.position + p1.position) / 2f,
                                                           p0.index.y != p1.index.y ? Quaternion.Euler(new Vector3(0, 0, 90f)) : Quaternion.identity,
                                                           squareBoundariesHolder);
        squareBoundary.Setup(this, direction, color, su, p0.index, p1.index);
        squareBoundaries.Add(squareBoundary);
    }

    public override bool CanMove(Direction direction)
    {
        foreach(var boundary in squareBoundaries)
        {
            if (boundary.boundaryInfo.direction.GetDirectionType() != direction.GetDirectionType())
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

    public override IEnumerator MoveToTarget(Direction direction)
    {
        OnMove?.Invoke(direction);
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
    }

    public bool ContainsBoundary(SquareBoundary sb)
    {
        if (pointsList.Contains(sb.boundaryInfo.minIndex) && pointsList.Contains(sb.boundaryInfo.maxIndex) && (unitsList.Contains(sb.AdjacentUnitIndex().p0) || unitsList.Contains(sb.AdjacentUnitIndex().p1)))
        {
            print(gameObject.name + " Contains " + $"{sb.boundaryInfo.minIndex},{sb.boundaryInfo.maxIndex}");
            return true;
        }
        else
        {
            if(!pointsList.Contains(sb.boundaryInfo.minIndex) || !pointsList.Contains(sb.boundaryInfo.maxIndex))
            {
                print("Not contains points index");
            }
            if((!unitsList.Contains(sb.AdjacentUnitIndex().p0) && !unitsList.Contains(sb.AdjacentUnitIndex().p1)))
            {
                print("Not contains unit");
            }
            print(gameObject.name + " Not Contains " + $"{sb.boundaryInfo.minIndex},{sb.boundaryInfo.maxIndex}");
        }
        return false;
    }
}
