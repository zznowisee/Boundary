using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Entity, IMoveChecker
{
    public MoveState moveState { get; set; }

    private void OnEnable() => SquareManager.Instance.boxes.Add(this);
    private void OnDisable() => SquareManager.Instance.boxes.Remove(this);

    public void AddToSquare()
    {
        Square[] squares = InputHelper.GetSquaresInWorldPosition(transform.position);
        for (int i = 0; i < squares.Length; i++)
            squares[i].AddNewBox(this);
    }

    public bool CanMove(Direction direction)
    {
        moveState = MoveState.CAN;
        return true;
    }
}
