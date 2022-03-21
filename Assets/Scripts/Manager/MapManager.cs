using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public Lattice<Point> map;
    public Unit[,] units;

    public Unit this[Vector2Int index] => GetUnit(index.x, index.y);
    public Unit this[int x, int y] => GetUnit(x, y);
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

        Vector2 origin = new Vector2((mapSize.x - 1) * cellSize, (mapSize.y - 1)* cellSize) / -2f;
        print(origin);
        map = new Lattice<Point>(mapSize.x, mapSize.y, cellSize, origin, (Lattice<Point> g, int x, int y) => new Point(x, y, g));
        units = new Unit[mapSize.x - 1, mapSize.y - 1];

        for (int y = 0; y < map.GetWidth() - 1; y++)
        {
            for (int x = 0; x < map.GetHeight() - 1; x++)
            {
                Unit unit = Instantiate(pfUnit, map.GetCenterPosition(x, y), Quaternion.identity, holder);
                unit.transform.localScale = Vector3.one * (1 - slotOutlinePercent) * cellSize;
                unit.Setup(new Vector2Int(x, y));
                units[x, y] = unit;
                unit.transform.Find("index").GetComponent<TextMeshPro>().text = $"{x}.{y}";
            }
        }
    }

    private Unit GetUnit(int x, int y)
    {
        if(x >= 0 && x < units.GetLength(0) && y >= 0 && y < units.GetLength(1))
        {
            return units[x, y];
        }
        return null;
    }
}

public class Point
{
    public Lattice<Point> map;
    public Vector2Int index;
    public Vector3 position;

    public Point(int x, int y, Lattice<Point> map)
    {
        this.index = new Vector2Int(x, y);
        this.map = map;
        position = map.GetWorldPosition(x, y);
    }
}

public class Line
{
    public Vector2Int sIndex;
    public Vector2Int lIndex;
}