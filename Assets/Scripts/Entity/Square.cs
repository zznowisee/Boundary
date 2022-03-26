using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Square : Entity
{

    public override event Action<Direction> OnMoveStart;
    public override event Action OnMoveEnd;

    public int boxNumber;

    private int squareIndex;
    private Transform squareUnitsHolder;
    private Transform squareBoundariesHolder;
    private List<Vector2Int> pointsList;

    private Color squareCol;
    [SerializeField] private SquareBoundary pfSquareBoundary;
    [SerializeField] private SquareUnit pfSquareUnit;

    [HideInInspector] public List<SquareBoundary> squareBoundaries;
    [HideInInspector] public List<SquareUnit> squareUnits;
    public SquareUnit this[Vector2Int index] => GetSquareUnit(index); 

    private void Awake()
    {
        squareUnitsHolder = transform.Find("squareUnitsHolder");
        squareBoundariesHolder = transform.Find("squareBoundariesHolder");
        squareBoundaries = new List<SquareBoundary>();
        squareUnits = new List<SquareUnit>();
        pointsList = new List<Vector2Int>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SquareManager.Instance.squaresList.Add(this);
    }
    protected override void OnDisable()
    {
        ProcessManager.Instance.RemoveRecorder(recorder);
        ProcessManager.Instance.ResetColorCheckArray(squareIndex);
        SquareManager.Instance.squaresList.Remove(this);
    }

    private SquareUnit GetSquareUnit(Vector2Int index)
    {
        for (int i = 0; i < squareUnits.Count; i++)
        {
            if(squareUnits[i].unitIndex == index)
            {
                return squareUnits[i];
            }
        }
        return null;
    }

    public void Setup(int squareIndex_, List<MapUnit> allUnits, Color boundaryCol_)
    {
        squareIndex = squareIndex_;
        anchorUnit = allUnits[0];
        squareCol = boundaryCol_;
        transform.position = anchorUnit.transform.position;
        CalculateBoundary(anchorUnit, allUnits, new List<MapUnit>());
        OnMoveStart += UpdatePointsUnitsList;
    }

    public override void Teleport(Vector2Int movement)
    {
        //update indices
        for (int i = 0; i < pointsList.Count; i++)
            pointsList[i] += movement;
        for (int i = 0; i < squareUnits.Count; i++)
            squareUnits[i].unitIndex += movement;
        for (int i = 0; i < squareBoundaries.Count; i++)
            squareBoundaries[i].Teleport(movement);

        anchorUnit = MapManager.Instance[movement + anchorUnit.unitIndex];
        transform.position = anchorUnit.transform.position;
    }

    void UpdatePointsUnitsList(Direction direction)
    {
        for (int i = 0; i < pointsList.Count; i++)
            pointsList[i] += direction.GetValue();
    }

    private void CheckPoint(MapUnit baseUnit, MapUnit checkUnit, List<MapUnit> squareUnits, List<MapUnit> checkedUnits, Direction direction, SquareUnit su)
    {
        Lattice<Point> map = MapManager.Instance.map;

        if (!squareUnits.Contains(checkUnit))
        {
            Point p0 = null, p1 = null;
            switch (direction)
            {
                case Direction.UP:
                    p0 = map[baseUnit.unitIndex + Vector2Int.up];
                    p1 = map[baseUnit.unitIndex + Vector2Int.one];
                    break;
                case Direction.DOWN:
                    p0 = map[baseUnit.unitIndex];
                    p1 = map[baseUnit.unitIndex + Vector2Int.right];
                    break;
                case Direction.LEFT:
                    p0 = map[baseUnit.unitIndex];
                    p1 = map[baseUnit.unitIndex + Vector2Int.up];
                    break;
                case Direction.RIGHT:
                    p0 = map[baseUnit.unitIndex + Vector2Int.right];
                    p1 = map[baseUnit.unitIndex + Vector2Int.one];
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

    private SquareUnit AddNewSquareUnit(MapUnit unit)
    {
        SquareUnit su = Instantiate(pfSquareUnit, unit.transform.position, Quaternion.identity, squareUnitsHolder);
        su.Setup(this, unit.unitIndex, squareCol);
        Vector2Int[] indices = new Vector2Int[] 
        { 
            unit.unitIndex, 
            unit.unitIndex + Vector2Int.right, 
            unit.unitIndex + Vector2Int.up, 
            unit.unitIndex + Vector2Int.one 
        };


        for (int i = 0; i < indices.Length; i++)
            if (!pointsList.Contains(indices[i]))
                pointsList.Add(indices[i]);
        su.squareUnitType = SquareUnitType.INNER;
        return su;
    }

    private void CalculateBoundary(MapUnit current, List<MapUnit> units, List<MapUnit> checkedUnits)
    {
        checkedUnits.Add(current);

        SquareUnit su = AddNewSquareUnit(current);
        squareUnits.Add(su);

        MapUnit up = MapManager.Instance[current.unitIndex + Vector2Int.up];
        MapUnit down = MapManager.Instance[current.unitIndex + Vector2Int.down];
        MapUnit left = MapManager.Instance[current.unitIndex + Vector2Int.left];
        MapUnit right = MapManager.Instance[current.unitIndex + Vector2Int.right];

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
        squareBoundary.Setup(this, direction, squareCol, su, p0.index, p1.index);
        squareBoundaries.Add(squareBoundary);
    }

    public override bool CanMove(Direction direction, List<Entity> canMoveEntityList)
    {
        foreach(var boundary in squareBoundaries)
        {
            if (boundary.boundaryInfo.direction.GetDirectionType() != direction.GetDirectionType())
                continue;

            if (!boundary.CanMove(direction, canMoveEntityList))
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
        OnMoveStart?.Invoke(direction);
        state = State.Moving;
        MapUnit target = MapManager.Instance[anchorUnit.unitIndex + direction.GetValue()];
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
        anchorUnit = target;
    }

    private int GetBoxNumber()
    {
        int number = 0;
        for (int i = 0; i < squareUnits.Count; i++)
        {
            MapUnit unit = MapManager.Instance[squareUnits[i].unitIndex];
            Box box = unit.currentEntity as Box;
            if (box)
            {
                number++;
            }
        }
        return number;
    }

    public bool ContainsBoundary(SquareBoundary sb)
    {
        return pointsList.Contains(sb.boundaryInfo.minIndex) && pointsList.Contains(sb.boundaryInfo.maxIndex) && (UnitsIndicesContains(sb.AdjacentUnitIndex().p0) || UnitsIndicesContains(sb.AdjacentUnitIndex().p1));
    }

    private bool UnitsIndicesContains(Vector2Int index)
    {
        for (int i = 0; i < squareUnits.Count; i++)
            if (squareUnits[i].unitIndex == index)
                return true;

        return false;
    }

    public void FinishOnceMovement()
    {
        moveState = MoveState.NONECHECK;
        boxNumber = GetBoxNumber();
    }

    public void Select()
    {
        squareBoundaries.ForEach(squareBoundary => squareBoundary.Highlighting());
    }

    public void CancelSelect()
    {
        squareBoundaries.ForEach(squareBoundary => squareBoundary.Normal());
    }

    public void ResetSquareBoundaries()
    {
        squareBoundaries.ForEach(squareBoundary => squareBoundary.SetBoundaryType(BoundaryType.SOLID));
    }
}
