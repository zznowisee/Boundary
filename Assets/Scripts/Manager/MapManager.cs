using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    [Header("Map参数")]
    [SerializeField] Vector2Int mapSize;
    [SerializeField] private float cellSize;
    [Range(0, 1f)]
    [SerializeField] private float slotOutlinePercent = .05f;
    [Header("预制体")]
    [SerializeField] private Unit pfUnit;
    public Grid<Point> map;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    public void Init()
    {
        string holderName = "SlotHolder";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform holder = new GameObject(holderName).transform;
        holder.transform.parent = transform;

        Vector2 origin = new Vector2(mapSize.x * cellSize, mapSize.y * cellSize) / -2f;
        map = new Grid<Point>(mapSize.x, mapSize.y, cellSize, origin, (Grid<Point> g, int x, int y) => new Point(x, y, g));

        for (int y = 0; y < map.GetHeight(); y++)
        {
            for (int x = 0; x < map.GetWidth(); x++)
            {
                Unit unit = Instantiate(pfUnit, map.GetCenterPosition(x, y), Quaternion.identity, holder);
                unit.transform.localScale = Vector3.one * (1 - slotOutlinePercent) * cellSize;
                unit.gameObject.name = $"{x}-{y}";
                unit.Setup(new Vector2Int(x, y));
                map.GetValue(x, y).SetUnit(unit);
                map.GetValue(x, y).position = map.GetWorldPosition(x, y);
            }
        }
    }

    public Unit GetUnit(int x, int y)
    {
        if(map[x, y] != null)
        {
            return map[x, y].unit;
        }

        return null;
    }

    public Unit GetUnit(Vector2Int index)
    {
        if(map[index.x, index.y] != null)
        {
            return map[index.x, index.y].unit;
        }

        return null;
    }
}

public class Point
{
    public Grid<Point> map;
    public Unit unit;
    public Vector2Int index;
    public Vector3 unitPosition;
    public Vector3 position;

    public Point(int x, int y, Grid<Point> map)
    {
        this.index = new Vector2Int(x, y);
        this.map = map;
        unitPosition = map.GetCenterPosition(x, y);
    }

    public void SetUnit(Unit unit) => this.unit = unit;
}
