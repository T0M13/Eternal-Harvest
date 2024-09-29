using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    public float ChanceToChange;

    public WalkerObject(Vector2 position, Vector2 direction, float chanceToChange)
    {
        Position = position;
        Direction = direction;
        ChanceToChange = chanceToChange;
    }
}
