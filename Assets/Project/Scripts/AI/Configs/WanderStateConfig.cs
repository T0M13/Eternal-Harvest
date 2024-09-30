using UnityEngine;

[CreateAssetMenu(menuName = "AI/WanderStateConfig", fileName = "_Wander_State_Config")]
public class WanderStateConfig : AIStateConfig
{
    public string walkStateString = "Walk";
    public float walkDuration = 2f;
    public float wanderRadius = 5f;
    public LayerMask walkableLayerMask;
    public Color gizmoColor = Color.green;
    public int maxWanderAttempts = 10;

    public string walkRight = "WalkRight";
    public string walkLeft = "WalkLeft";
    public string walkUp = "WalkUp";
    public string walkDown = "WalkDown";

    public Vector3Int[] directions = new Vector3Int[]
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
