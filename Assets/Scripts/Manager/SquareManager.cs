using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour
{
    public static SquareManager Instance { get; private set; }
    [HideInInspector] public List<Square> squaresList;

    private void Awake()
    {
        Instance = this;
    }

    public List<Square> GetSolidBoundarySquares(Vector2Int pointsIndex)
    {
        List<Square> squares = new List<Square>();
        for (int i = 0; i < squaresList.Count; i++)
        {
            for (int j = 0; j < squaresList[i].squareBoundaries.Count; j++)
            {
                if (squaresList[i].squareBoundaries[j].pointsIndex == pointsIndex)
                {
                    if (squaresList[i].squareBoundaries[j].boundaryType == BoundaryType.SOLID)
                    {
                        squares.Add(squaresList[i]);
                    }
                }
            }
        }
        return squares;
    }

    public void PlayerFinishMove()
    {
        for (int i = 0; i < squaresList.Count; i++)
        {
            squaresList[i].moveState = MoveState.NONECHECK;
        }
    }

    public void UpdateSquareBoundaries(SquareBoundary checkSquareBoundary)
    {
        
    }
}
