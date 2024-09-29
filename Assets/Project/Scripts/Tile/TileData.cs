using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Tiles/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public TileBase defaultTile; // Default middle tile
    public TileBase topLeftCornerTile;
    public TileBase topRightCornerTile;
    public TileBase bottomLeftCornerTile;
    public TileBase bottomRightCornerTile;
    public TileBase leftEdgeTile;
    public TileBase rightEdgeTile;
    public TileBase topEdgeTile;
    public TileBase bottomEdgeTile;

    // New Inner Corners and Inner Edges
    public TileBase innerTopLeftCornerTile;
    public TileBase innerTopRightCornerTile;
    public TileBase innerBottomLeftCornerTile;
    public TileBase innerBottomRightCornerTile;
    public TileBase innerLeftEdgeTile;
    public TileBase innerRightEdgeTile;
    public TileBase innerTopEdgeTile;
    public TileBase innerBottomEdgeTile;
}
