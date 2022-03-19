using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lattice<T>
{
    public T this[Vector2Int index] => GetValue(index.x, index.y);
    public T this[int x, int y] => GetValue(x, y);

    private T[,] array;
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;

    public Lattice(int width, int height, float cellSize, Vector3 originPosition, Func<Lattice<T>, int, int, T> Create)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        array = new T[width, height];
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                array[x, y] = Create(this, x, y);
            }
        }
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public Vector3 GetCenterPosition(int x, int y)
    {
        return GetWorldPosition(x, y) + Vector3.one * cellSize / 2f;
    }

    private T GetValue(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return array[x, y];
        }
        else
        {
            return default(T);
        }
    }
}
