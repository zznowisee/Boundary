using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public T this[Vector2Int index] => GetValue(index.x, index.y);
    public T this[int x, int y] => GetValue(x,y);

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;

    private T[,] gridArray;
    private TextMesh[,] debugArray;
    public Grid(int _width , int _height , float _cellSize , Vector3 _originPosition , Func<Grid<T> , int ,int ,T> createGridObject)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;
        originPosition = _originPosition;

        gridArray = new T[width, height];
        debugArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        bool showDebug = false;
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    //debugArray[x, y] = Utils.CreateWorldText(null, gridArray[x, y]?.ToString(), GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs e) =>
            {
                 debugArray[e.x, e.y].text = gridArray[e.x, e.y]?.ToString();
            };
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition , out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y , T value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugArray[x, y].text = gridArray[x, y].ToString();
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetValue(Vector3 worldPosition , T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);

        SetValue(x, y, value);
    }

    public T GetValue(int x, int y)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public Vector3 GetCenterPosition(int x, int y)
    {
        return new Vector3(cellSize * (x + 0.5f - width * 0.5f), cellSize * (y + 0.5f - height * 0.5f));
    }

    public T GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void TriggerGridChanged(int x, int y)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }
}
