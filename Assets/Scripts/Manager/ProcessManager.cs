using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProcessManager : Singleton<ProcessManager>
{

    public enum OperateType
    {
        IDLE = 0,
        CREATING,
        DRAGGING,
        SELECTING
    }

    public GameState GameState
    {
        get { return gameState; }
    }

    public UndoHelper UndoHelper
    {
        get { return undoHelper; }
    }

    private GameState gameState;
    private OperateType operateType;
    private KeyCode? currentCreateKeyCode;
    private ISelectable currentSelecting;
    private List<MapUnit> selectingUnitList;
    private List<MapUnit> unactiveUnitList;
    private List<Recorder> recorders;
    private UndoHelper undoHelper;
    private bool[] squareColorCheckArray;

    [Header("引用")]
    [SerializeField] private PaletteSO palette;
    [SerializeField] private GameObject createGhost;
    [Header("预制体")]
    [SerializeField] private Square pfSquare;
    [SerializeField] private Player pfPlayer;
    [SerializeField] private Box pfBox;
    [Header("按钮")]
    [SerializeField] private Button generateBtn;
    [SerializeField] private Button clearBtn;
    [SerializeField] private Button playBtn;
    [Header("按键")]
    [SerializeField] private KeyCode boxCreateKey;
    [SerializeField] private KeyCode playerCreateKey;

    public override void Awake()
    {
        base.Awake();
        recorders = new List<Recorder>();
        selectingUnitList = new List<MapUnit>();
        undoHelper = new UndoHelper();
        unactiveUnitList = new List<MapUnit>();
        squareColorCheckArray = new bool[palette.colorList.Count];
    }

    private void Start()
    {
        gameState = GameState.EDITING;

        generateBtn.onClick.AddListener(Generate);
        clearBtn.onClick.AddListener(Clear);
        playBtn.onClick.AddListener(Test);

        createGhost.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.EDITING:
                HandleInput();
                break;
            case GameState.TESTING:
                HandleUndo();
                break;
        }
    }

    private void HandleInput()
    {
        if (InputHelper.IsMouseOverUIObject)
            return;

        switch (operateType)
        {
            case OperateType.IDLE: IdleState(); break;
            case OperateType.CREATING: CreatingState(); break;
            case OperateType.DRAGGING: DraggingState(); break;
            case OperateType.SELECTING: SelectingState(); break;
        }
    }

    private void IdleState()
    {
        HandleCreateSetup();
        HandleEntitySelect();
    }

    private void HandleUndo()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
            return;
        }
    }

    private void HandleCreateSetup()
    {
        if (Input.GetKeyDown(boxCreateKey))
        {
            currentCreateKeyCode = boxCreateKey;
            operateType = OperateType.CREATING;
            createGhost.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(playerCreateKey))
        {
            currentCreateKeyCode = playerCreateKey;
            operateType = OperateType.CREATING;
            createGhost.gameObject.SetActive(true);
        }
    }

    private void HandleEntitySelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentSelecting = GetSelectableUnderMousePosition();
            if (currentSelecting != null)
            {
                operateType = OperateType.DRAGGING;
                currentSelecting.LeftClick();
                return;
            }

            MapUnit unit = InputHelper.GetMapUnitUnderMousePosition();
            if (unit != null)
            {
                if (!unit.Active)
                    return;

                if (unit.Selected)
                {
                    unit.CancelSelect();
                    selectingUnitList.Remove(unit);
                }
                else
                {
                    if (GetSquareIndex() == -1)
                        return;

                    unit.Select(palette.colorList[GetSquareIndex()]);
                    selectingUnitList.Add(unit);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            currentSelecting = GetSelectableUnderMousePosition();
            if (currentSelecting != null)
            {
                operateType = OperateType.SELECTING;
                currentSelecting.RightClick();
                return;
            }

            MapUnit unit = InputHelper.GetMapUnitUnderMousePosition();
            if(unit != null)
            {
                if (unit.IsEmpty())
                {
                    unit.SetActive();
                    if (!unit.Active)
                        unactiveUnitList.Add(unit);
                    else
                        unactiveUnitList.Remove(unit);

                    if (selectingUnitList.Contains(unit))
                        selectingUnitList.Remove(unit);
                }
            }
        }
    }

    private void CreatingState()
    {
        MapUnit mapUnit = InputHelper.GetMapUnitUnderMousePosition();
        if (mapUnit != null)
        {
            createGhost.gameObject.transform.position = mapUnit.transform.position;
            if (Input.GetMouseButtonDown(0))
            {
                if (mapUnit.IsEmpty())
                {
                    if(currentCreateKeyCode == playerCreateKey)
                    {
                        Player player = Instantiate(pfPlayer);
                        player.Setup(mapUnit);
                    }
                    else if(currentCreateKeyCode == boxCreateKey)
                    {
                        Box box = Instantiate(pfBox);
                        box.Setup(mapUnit);
                    }
                }
            }
        }

        if (Input.GetKeyUp((KeyCode)currentCreateKeyCode))
        {
            currentCreateKeyCode = null;
            createGhost.gameObject.SetActive(false);
            operateType = OperateType.IDLE;
        }
    }

    private void DraggingState()
    {
        currentSelecting.Dragging();

        if (Input.GetMouseButtonUp(0))
        {
            currentSelecting.LeftRelease();
            currentSelecting = null;
            operateType = OperateType.IDLE;
        }
    }

    private void SelectingState()
    {
        if (Input.GetMouseButtonUp(1))
        {
            operateType = OperateType.IDLE;
            ISelectable selectable = GetSelectableUnderMousePosition();
            if(selectable == null)
            {
                currentSelecting.LeftRelease();
            }
            else
            {
                if(selectable.transform == currentSelecting.transform)
                {
                    currentSelecting.RightRelease();
                }
            }
        }
    }

    public void AddRecorder(Recorder recorder) => recorders.Add(recorder);
    public void RemoveRecorder(Recorder recorder) => recorders.Remove(recorder);

    private void Clear()
    {
        for (int i = recorders.Count - 1; i >= 0; i--)
            Destroy(recorders[i].Entity.gameObject);
        for (int i = 0; i < squareColorCheckArray.Length; i++)
            squareColorCheckArray[i] = false;
        for (int i = 0; i < unactiveUnitList.Count; i++)
            unactiveUnitList[i].SetActive();
        unactiveUnitList.Clear();
        ClearSelecting();
    }

    private void Test()
    {
        switch (gameState)
        {
            case GameState.TESTING: Stop(); break;
            case GameState.EDITING: Play(); break;
        }
    }

    private void Stop()
    {
        SquareManager.Instance.ResetAll();
        gameState = GameState.EDITING;
        playBtn.GetComponentInChildren<TextMeshProUGUI>().text = "PLAY";

        clearBtn.interactable = true;
        generateBtn.interactable = true;
        recorders.ForEach(recorder => recorder.Resetup());
    }

    private void Play()
    {
        SquareManager.Instance.DetectCoincidenceBoundary();

        gameState = GameState.TESTING;
        playBtn.GetComponentInChildren<TextMeshProUGUI>().text = "PAUSE";

        ClearSelecting();

        clearBtn.interactable = false;
        generateBtn.interactable = false;
    }

    private void Generate()
    {
        if (selectingUnitList.Count == 0)
            return;

        Square square = Instantiate(pfSquare);
        int squareIndex = GetSquareIndex();
        square.Setup(squareIndex, selectingUnitList, palette.colorList[squareIndex]);

        ClearSelecting();

        squareColorCheckArray[squareIndex] = true;
    }

    private void ClearSelecting()
    {
        selectingUnitList.ForEach(mapUnit => mapUnit.CancelSelect());
        selectingUnitList.Clear();
    }

    private int GetSquareIndex()
    {
        for (int i = 0; i < squareColorCheckArray.Length; i++)
        {
            if (!squareColorCheckArray[i])
            {
                return i;
            }
        }
        print("Run out of color");
        return -1;
    }

    private ISelectable GetSelectableUnderMousePosition()
    {
        if (InputHelper.IsMouseOverUIObject)
            return null;

        RaycastHit2D[] hits = Physics2D.RaycastAll(InputHelper.MouseWorldPosition, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                ISelectable selectable = hits[i].collider.GetComponent<ISelectable>();
                if(selectable != null)
                {
                    return selectable;
                }
            }
        }
        return null;
    }

    private void Undo()
    {
        undoHelper.Undo();
        SquareManager.Instance.DetectCoincidenceBoundary();
    }

    public void ResetColorCheckArray(int resetIndex)
    {
        squareColorCheckArray[resetIndex] = false;
    }
}

public class UndoHelper
{
    public List<List<Recorder>> recordersList;
    public int? pointer;
    public UndoHelper()
    {
        recordersList = new List<List<Recorder>>();
        pointer = null;
    }

    public void Record(List<Recorder> recorder)
    {
        recordersList.Add(recorder);
        pointer = recordersList.Count - 1;
    }

    public void Undo()
    {
        if (pointer == null)
            return;

        for (int i = 0; i < recordersList[(int)pointer].Count; i++)
        {
            recordersList[(int)pointer][i].Undo();
        }
        recordersList.RemoveAt(recordersList.Count - 1);

        if (--pointer < 0)
        {
            pointer = null;
        }
    }
}

