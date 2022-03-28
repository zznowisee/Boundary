using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    public enum State { Idle = 0, Moving }

    public Recorder Recorder
    {
        get { return recorder; }
    }

    protected State state;
    protected MapUnit anchorUnit;
    protected const float moveTime = .2f;
    protected Recorder recorder;
    public virtual event Action<Direction> OnMoveStart;
    public virtual event Action OnMoveEnd;

    public MoveState moveState;    //运动状态是否已经被检测过了

    protected virtual void OnEnable()
    {
        recorder = new Recorder(this);
        ProcessManager.Instance.AddRecorder(recorder);
    }
    protected virtual void OnDisable()
    {
        LeaveCurrentUnit();
        ProcessManager.Instance.RemoveRecorder(recorder);
    }

    protected void LeaveCurrentUnit()
    {
        if(anchorUnit.currentEntity == this)
            anchorUnit.currentEntity = null;
        anchorUnit = null;
    }

    protected void EnterNewUnit(MapUnit newUnit)
    {
        anchorUnit = newUnit;
        anchorUnit.currentEntity = this;
    }

    public virtual void Setup(MapUnit initUnit_)
    {
        EnterNewUnit(initUnit_);
        transform.position = anchorUnit.transform.position;
    }

    public virtual void MoveTo(Direction direction)
    {
        StartCoroutine(MoveToTarget(direction));
    }

    public virtual IEnumerator MoveToTarget(Direction direction)
    {
        OnMoveStart?.Invoke(direction);
        state = State.Moving;
        MapUnit target = MapManager.Instance[anchorUnit.unitIndex + direction.GetValue()];
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

        EnterNewUnit(target);
        OnMoveEnd?.Invoke();
    }

    public virtual bool CanMove(Direction direction, List<Entity> CanMoveEntities)
    {
        return true;
    }

    public virtual void Teleport(Vector2Int movement)
    {
        Vector2Int preAnchorUnitIndex = anchorUnit.unitIndex;
        LeaveCurrentUnit();
        anchorUnit = MapManager.Instance[movement + preAnchorUnitIndex];
        EnterNewUnit(anchorUnit);
        transform.position = anchorUnit.transform.position;
    }
}

public class Recorder
{
    public Entity Entity
    {
        get { return entity; }
    }

    private Entity entity;
    private List<Vector2Int> movements;

    public Recorder(Entity entity_)
    {
        entity = entity_;
        movements = new List<Vector2Int>();
        entity.OnMoveStart += (Direction direction) => movements.Add(direction.GetValue());
    }

    public void Undo()
    {
        if (movements.Count == 0)
            return;

        entity.Teleport(movements[movements.Count - 1] * -1);
        movements.RemoveAt(movements.Count - 1);
    }

    public void Resetup()
    {
        entity.Teleport(GetTotalMovement());
        movements.Clear();
    }

    private Vector2Int GetTotalMovement()
    {
        Vector2Int total = Vector2Int.zero;
        movements.ForEach(move => total += move);
        return total * -1;
    }
}