using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Entity
{
    private void Update()
    {
        if (state == State.Moving)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveTo(Direction.UP);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveTo(Direction.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveTo(Direction.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveTo(Direction.RIGHT);
        }
    }

    public override bool CanMove(Direction direction)
    {
        Unit target = MapManager.Instance.GetUnit(currentUnit.index + direction.GetValue());
        bool canMove = true;
        if (target)
        {
            //inner
            SquareUnit[] baseUnits = InputHelper.GetSquareUnitsInWorldPosition(transform.position);
            //outter
            SquareUnit[] targetUnits = InputHelper.GetSquareUnitsInWorldPosition(target.transform.position);

            //baseUnit point to direction boundary
            for (int i = 0; i < baseUnits.Length; i++)
            {
                var su = baseUnits[i];
                if (su.CanMove(direction))
                {

                }
            }
            //targetUnit point to opposite direction boundary
        }
        return true;
    }
}
