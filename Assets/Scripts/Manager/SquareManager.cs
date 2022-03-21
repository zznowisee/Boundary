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

    public void PlayerFinishMove()
    {
        for (int i = 0; i < squaresList.Count; i++)
        {
            squaresList[i].moveState = MoveState.NONECHECK;
        }
        UpdateCoincidenceArea();
    }

    private void Update()
    {
       //UpdateCoincidenceArea();
    }

    public void UpdateCoincidenceArea()
    {
        for (int i = 0; i < dottedBoundariesList.Count; i++)
        {
            dottedBoundariesList[i].SetBoundaryType(BoundaryType.SOLID);
        }
        dottedBoundariesList.Clear();
        for (int i = 0; i < squaresList.Count; i++)
        {
            Square current = squaresList[i];
            for (int j = 0; j < squaresList[i].squareBoundaries.Count; j++)
            {
                SquareBoundary sb = current.squareBoundaries[j];
                for (int k = 0; k < squaresList.Count; k++)
                {
                    if (k != i)
                    {
                        if (squaresList[k].ContainsBoundary(sb) && current.boxNumber <= squaresList[k].boxNumber)
                        {
                            print(sb.square.gameObject.name);
                            sb.SetBoundaryType(BoundaryType.DOTTED);
                            dottedBoundariesList.Add(sb);
                        }
                    }
                }
            }
        }
    }
}
