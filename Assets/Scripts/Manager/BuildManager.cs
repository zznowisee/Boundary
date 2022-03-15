using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private ColorSO squareColors;
    [Header("Ô¤ÖÆÌå")]
    [SerializeField] private Square pfSquare;
    [SerializeField] private Player pfPlayer;
    [SerializeField] private Box pfBox;

    private void Start()
    {
        InitPlayer(new Vector2Int(5, 5));
        InitSquare(new Vector2Int(1, 1), new Vector2Int(2, 3), squareColors.yellow);
        //InitSquare(new Vector2Int(3, 3), new Vector2Int(4, 3), squareColors.blue);
        //InitBox(new Vector2Int(2, 6));
        //InitBox(new Vector2Int(3, 4));
    }

    void InitPlayer(Vector2Int initIndex)
    {
        Unit playerUnit = MapManager.Instance.map.GetValue(initIndex.x, initIndex.y).unit;
        Player player = Instantiate(pfPlayer, playerUnit.transform.position, Quaternion.identity);
        player.Setup(playerUnit);
    }

    void InitBox(Vector2Int initIndex)
    {
        Unit unit = MapManager.Instance.map.GetValue(initIndex.x, initIndex.y).unit;
        Box box = Instantiate(pfBox, unit.transform.position, Quaternion.identity);
        box.Setup(unit);
    }

    void InitSquare(Vector2Int initIndex, Vector2Int size, Color color_)
    {
        Point slot = MapManager.Instance.map.GetValue(initIndex.x, initIndex.y);
        Square square = Instantiate(pfSquare, slot.unitPosition, Quaternion.identity);
        square.Setup(size, slot);
    }
}
