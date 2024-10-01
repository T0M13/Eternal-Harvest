using UnityEngine;

[CreateAssetMenu(menuName = "AI/WanderStateConfig", fileName = "_Wander_State_Config")]
public class WanderStateConfig : AIStateConfig
{
    public float minTileDistance = 3f; // Minimum distance a tile should be away from the agent
    public float maxTileDistance = 10f; // Maximum distance a tile should be away from the agent
    public float distanceBias = 2f;
    public float wanderSpeed = 5f;
    public float adaptiveDistanceReduction = 0.5f; // How much to reduce the minimum distance if no valid tiles are found
    public LayerMask walkableLayerMask; // LayerMask for walkable areas
    public Color gizmoColor = Color.green; // Color for Gizmos
    public int maxWanderAttempts = 10; // Maximum attempts to find a wanderable position

    public string walkRight = "WalkRight";
    public string walkLeft = "WalkLeft";
    public string walkUp = "WalkUp";
    public string walkDown = "WalkDown";

    public Vector3Int[] directions = new Vector3Int[]  // Movement directions
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0)
    };

    public override AIStateType GetStateType()
    {
        return AIStateType.Wander;
    }

    public override AIState InitializeState(AIAgent agent)
    {
        WanderState wanderState = new WanderState();
        wanderState.Config = this;
        return wanderState;
    }
}
