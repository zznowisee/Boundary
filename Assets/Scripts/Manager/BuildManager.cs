using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private PaletteSO squareColors;
    [Header("Ô¤ÖÆÌå")]
    [SerializeField] private Square pfSquare;
    [SerializeField] private Player pfPlayer;
    [SerializeField] private Box pfBox;

    void InitSquare(Vector2Int lowLeftUnit, Vector2Int size)
    {
        MapUnit unit = MapManager.Instance[lowLeftUnit];
        Square square = Instantiate(pfSquare);
        //square.Setup(size, unit);
    }

    void InitPlayer(Vector2Int index)
    {
        Player player = Instantiate(pfPlayer);
        player.Setup(MapManager.Instance[index]);
    }

    void InitBox(Vector2Int index)
    {
        Box box = Instantiate(pfBox);
        box.Setup(MapManager.Instance[index]);
    }

    private void Start()
    {
        Vector2Int offset = new Vector2Int(3, 3);
        InitSquare(offset + new Vector2Int(1, 0), new Vector2Int(3, 3));
        InitSquare(offset + new Vector2Int(0, 2), new Vector2Int(3, 2));
        InitPlayer(offset + new Vector2Int(0, 4));

        InitBox(offset + new Vector2Int(0, 3));
        InitBox(offset + new Vector2Int(3, 0));
    }
}
