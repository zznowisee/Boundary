using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IMoveChecker
{
    public enum State { Idle = 0, Moving }

    protected State state;
    [HideInInspector] public Unit currentUnit;
    protected const float moveTime = .2f;

    public event Action<Direction> OnMoveStart;
    public event Action OnMoveEnd;

    public MoveState moveState { get; set; }
    private void OnEnable() => BoundaryManager.Instance.moveCheckers.Add(this);
    private void OnDisable() => BoundaryManager.Instance.moveCheckers.Remove(this);

    public virtual void Setup(Unit initUnit_)
    {
        currentUnit = initUnit_;
        transform.position = currentUnit.transform.position;
    }

    public virtual void MoveTo(Direction direction)
    {
        Unit target = MapManager.Instance[currentUnit.index + direction.GetValue()];

        if(target != null)
        {
            StartCoroutine(MoveToTarget(direction));
        }
    }

    public virtual IEnumerator MoveToTarget(Direction direction)
    {
        yield return null;
    }

    public virtual bool CanMove(Direction direction)
    {
        return true;
    }
}
