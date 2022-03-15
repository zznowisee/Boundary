using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareManager : MonoBehaviour
{
    public static SquareManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [HideInInspector] public List<Square> squaresList;
    [HideInInspector] public List<Box> boxes;

    private void Start()
    {
        squaresList = new List<Square>();
    }

    public void CheckSquareBox()
    {
        for (int i = 0; i < squaresList.Count; i++)
            squaresList[i].ClearBoxes();

        for (int i = 0; i < boxes.Count; i++)
            boxes[i].AddToSquare();
    }
}
