using UnityEngine;

public class Tile
{
    public Vector2Int Position { get; private set; }
    public TileType TileType { get; set; }
    public GameObject ObjectOnTile { get; set; }

    public Tile(Vector2Int position, TileType type)
    {
        Position = position;
        TileType = type;
        ObjectOnTile = null;
    }
}