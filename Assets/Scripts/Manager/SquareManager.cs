using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SquareManager : MonoBehaviour
{
    public static SquareManager Instance { get; private set; }
    [HideInInspector] public List<Square> squaresList;
    private List<SquareBoundary> dottedBoundariesList;
    private void Awake()
    {
        Instance = this;
        dottedBoundariesList = new List<SquareBoundary>();
    }

    private void Start()
    {
        PlayerFinishMove();
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
                    if (squaresList[i].squareBoundaries[j].boundaryInfo.boundaryType == BoundaryType.SOLID)
                    {
                        squares.Add(squaresList[i]);
                    }
                }
            }
        }
        return squares;
    }

    public void PlayerFinishCheck()
    {
        squaresList.ForEach(square => square.moveState = MoveState.NONECHECK);
    }

    public void PlayerFinishMove()
    {
        for (int i = 0; i < squaresList.Count; i++)
        {
            squaresList[i].FinishOnceMovement();
        }
        DetectCoincidenceBoundary();
    }

    public void DetectCoincidenceBoundary()
    {
        //reset all dotted boundary
        for (int i = 0; i < dottedBoundariesList.Count; i++)
        {
            dottedBoundariesList[i].SetBoundaryType(BoundaryType.SOLID);
        }
        dottedBoundariesList.Clear();

        //detect all dotted boundary
        for (int i = 0; i < squaresList.Count; i++)
        {
            Square current = squaresList[i];
            for (int j = 0; j < squaresList[i].squareBoundaries.Count; j++)
            {
                SquareBoundary sb = current.squareBoundaries[j];
                for (int k = 0; k < squaresList.Count; k++)
                {
                    if (k == i)
                        continue;

                    if (squaresList[k].ContainsBoundary(sb) && current.boxNumber <= squaresList[k].boxNumber)
                    {
                        sb.SetBoundaryType(BoundaryType.DOTTED);
                        dottedBoundariesList.Add(sb);
                    }
                }
            }
        }
    }

    public void ResetAll()
    {
        squaresList.ForEach(square => square.ResetSquareBoundaries());
        dottedBoundariesList.Clear();
    }
}
