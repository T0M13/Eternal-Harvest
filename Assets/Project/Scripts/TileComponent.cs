using UnityEngine;

public class TileComponent : MonoBehaviour
{
    public Tile Tile { get; private set; }

    public void Init(Tile tile)
    {
        Tile = tile;
    }
}
