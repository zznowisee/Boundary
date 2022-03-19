using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    public static BoundaryManager Instance { get; private set; }
    [HideInInspector] public List<IMoveChecker> moveCheckers;
    [HideInInspector] public List<Square> squares;

    private void Awake()
    {
        Instance = this;
        moveCheckers = new List<IMoveChecker>();
    }

    public List<SquareBoundary> GetSolidBoundaries(Vector2Int pointsIndex)
    {
        List<SquareBoundary> boundaries = new List<SquareBoundary>();
        for (int i = 0; i < squares.Count; i++)
        {
            for (int j = 0; j < squares[i].squareBoundaries.Count; j++)
            {
                if (squares[i].squareBoundaries[j].pointsIndex == pointsIndex)
                {
                    if (squares[i].squareBoundaries[j].boundaryType == BoundaryType.SOLID)
                    {
                        boundaries.Add(squares[i].squareBoundaries[j]);
                    }
                }
            }
        }
        return boundaries;
    }

    public void PlayerFinishMove()
    {
        for (int i = 0; i < squares.Count; i++)
        {
            squares[i].moveState = MoveState.NONECHECK;
        }
    }
}
