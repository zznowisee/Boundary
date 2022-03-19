using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveChecker
{
    GameObject gameObject { get; }
    MoveState moveState { get; set; }
    bool CanMove(Direction direction);
}
