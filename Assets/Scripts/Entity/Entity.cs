using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    public enum State { Idle = 0, Moving }

    protected State state;
    public Unit currentUnit;
    protected const float moveTime = .2f;

    public virtual event Action OnReached;
    public virtual event Action<Unit, Direction> OnBeginMove;

    public void Setup(Unit initUnit_)
    {
        currentUnit = initUnit_;
        currentUnit.currentEntity = this;

        transform.position = currentUnit.transform.position;
    }

    public virtual void MoveTo(Direction direction)
    {
        if (!CanMove(direction))
            return;

        Unit target = MapManager.Instance.GetUnit(currentUnit.index + direction.GetValue());

        if(target != null)
        {
            StartCoroutine(MoveToTarget(target));
        }
    }

    public IEnumerator MoveToTarget(Unit unit_)
    {
        state = State.Moving;
        currentUnit.currentEntity = null;
        currentUnit = null; 

        Vector3 start = transform.position;
        Vector3 end = unit_.transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        currentUnit = unit_;
        state = State.Idle;
        currentUnit = unit_;
        currentUnit.currentEntity = this;
        OnReached?.Invoke();
    }

    public virtual bool CanMove(Direction direction)
    {
        return true;
    }
}
