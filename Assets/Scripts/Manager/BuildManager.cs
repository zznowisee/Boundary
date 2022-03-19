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

    void InitSquare(Vector2Int lowLeftUnit, Vector2Int size)
    {
        Unit unit = MapManager.Instance[lowLeftUnit];
        Square square = Instantiate(pfSquare);
        square.Setup(size, unit);
    }

    void InitPlayer(Vector2Int index)
    {
        Player player = Instantiate(pfPlayer);
        player.Setup(MapManager.Instance[index]);
    }

    private void Start()
    {
        InitSquare(Vector2Int.one, new Vector2Int(2, 2));
        //InitSquare(Vector2Int.one, new Vector2Int(3, 2));
        InitPlayer(new Vector2Int(3, 2));
    }
}
