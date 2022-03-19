using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Entity
{



    public override bool CanMove(Direction direction)
    {
        return true;
    }
}
