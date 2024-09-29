using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomTile", menuName = "Tiles/CustomTile")]
public class CustomTile : Tile
{
    [Header("Tile Properties")]
    public TileProperties tileProperties = TileProperties.None;
    public bool isOccupied = false;

    [Header("Associated Tilemap")]
    public ITilemap tilemap;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        this.tilemap = tilemap;

        return base.StartUp(position, tilemap, go);
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }

    public bool IsBuildable()
    {
        return (tileProperties & TileProperties.Buildable) != 0 && !isOccupied;
    }

    public bool IsWalkable()
    {
        return (tileProperties & TileProperties.Walkable) != 0;
    }

    public bool IsWater()
    {
        return (tileProperties & TileProperties.Water) != 0;
    }

    public bool IsBlocked()
    {
        return (tileProperties & TileProperties.Blocked) != 0;
    }

    public bool CanDrop()
    {
        return !IsBlocked();
    }
}
