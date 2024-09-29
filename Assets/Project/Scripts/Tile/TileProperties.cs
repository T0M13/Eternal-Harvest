using System;

[Flags]
public enum TileProperties
{
    None = 0,           // No special properties
    Walkable = 1 << 0,   // 0001 = 1: Can walk on this tile
    Buildable = 1 << 1,  // 0010 = 2: Can build on this tile
    Water = 1 << 2,      // 0100 = 4: Is a water tile
    Blocked = 1 << 3,      

}
