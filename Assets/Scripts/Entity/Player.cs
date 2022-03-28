using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Entity, ISelectable
{
    private Color defaultCol;
    private SpriteRenderer spriteRenderer;
    public override event Action<Direction> OnMoveStart;
    public override event Action OnMoveEnd;
    public event Action OnCheckEnd;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultCol = spriteRenderer.color;
    }

    private void Update()
    {
        if (ProcessManager.Instance.GameState == GameState.EDITING)
            return;

        HandleInput();
    }

    public override void Setup(MapUnit initUnit_)
    {
        base.Setup(initUnit_);
        OnMoveEnd += SquareManager.Instance.PlayerFinishMove;
        OnCheckEnd += SquareManager.Instance.PlayerFinishCheck;
    }

    public override IEnumerator MoveToTarget(Direction direction)
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
        OnMoveEnd?.Invoke();

        EnterNewUnit(target);
    }

    private void HandleInput()
    {
        if (state == State.Moving)
            return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) MoveTo(Direction.UP);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) MoveTo(Direction.DOWN);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveTo(Direction.LEFT);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveTo(Direction.RIGHT);
    }

    public override void MoveTo(Direction direction)
    {
        List<Entity> canMoveEntityList = new List<Entity>();
        if (CanMove(direction, canMoveEntityList))
        {
            List<Recorder> members = new List<Recorder>() { recorder };
            canMoveEntityList.ForEach(entity =>
            {
                entity.MoveTo(direction);
                members.Add(entity.Recorder);
            });
            ProcessManager.Instance.UndoHelper.Record(members);
            StartCoroutine(MoveToTarget(direction));
        }
        OnCheckEnd?.Invoke();
    }

    public override bool CanMove(Direction direction, List<Entity> canMoveEntityList)
    {
        print("Running player - CanMove");
        MapUnit target = MapManager.Instance[direction.GetValue() + anchorUnit.unitIndex];
        if (target == null) return false;
        if (!target.Active) return false;
        Vector2Int boundaryCheckIndex = InputHelper.GetBoundaryCheckIndex(direction, anchorUnit.unitIndex);
        List<Square> squares = SquareManager.Instance.GetSolidBoundarySquares(boundaryCheckIndex);
        if (squares.Count == 0)
        {
            return target.IsEmpty();
        }
        else
        {
            foreach(var square in squares)
            {
                if(square.moveState == MoveState.CANNOT)
                {
                    return false;
                }
                else
                {
                    if (!square.CanMove(direction, canMoveEntityList))
                    {
                        print("can not move");
                        square.moveState = MoveState.CANNOT;
                        return false;
                    }
                    else
                    {
                        square.moveState = MoveState.CAN;
                        canMoveEntityList.Add(square);
                    }
                }
            }
        }
        return true;
    }

    public void LeftClick()
    {
        spriteRenderer.color = Color.white;
    }

    public void RightClick()
    {
        spriteRenderer.color = Color.white;
    }

    public void RightRelease()
    {
        Destroy(gameObject);
    }

    public void LeftRelease()
    {
        spriteRenderer.color = defaultCol;
    }

    public void Dragging()
    {
        MapUnit newUnit = InputHelper.GetMapUnitUnderMousePosition();
        if (newUnit == null)
            return;
        if (!newUnit.IsEmpty())
            return;
        if (newUnit.unitIndex == anchorUnit.unitIndex)
            return;

        Teleport(newUnit.unitIndex - anchorUnit.unitIndex);
    }
}
