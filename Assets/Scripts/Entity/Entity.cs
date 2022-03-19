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

    protected void LeaveCurrentUnit()
    {
        currentUnit.currentEntity = null;
        currentUnit = null;
    }

    protected void EnterNewUnit(Unit newUnit)
    {
        currentUnit = newUnit;
        currentUnit.currentEntity = this;
    }

    public virtual void Setup(Unit initUnit_)
    {
        EnterNewUnit(initUnit_);
        transform.position = currentUnit.transform.position;
    }

    public virtual void MoveTo(Direction direction)
    {
        StartCoroutine(MoveToTarget(direction));
    }

    public virtual IEnumerator MoveToTarget(Direction direction)
    {
        state = State.Moving;
        Unit target = MapManager.Instance[currentUnit.index + direction.GetValue()];
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;
        LeaveCurrentUnit();
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        state = State.Idle;
        OnMoveEnd?.Invoke();

        EnterNewUnit(target);
    }

    public virtual bool CanMove(Direction direction)
    {
        return true;
    }
}
