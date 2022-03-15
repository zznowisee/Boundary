using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveChecker
{
    Transform transform { get; }
    MoveState moveState { get; set; }
}
