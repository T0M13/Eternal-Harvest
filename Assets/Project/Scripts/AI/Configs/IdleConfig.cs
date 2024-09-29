using UnityEngine;

[CreateAssetMenu(menuName = "AI/IdleStateConfig", fileName = "_Idle_State_Config")]

public class IdleStateConfig : AIStateConfig
{
    public string IdleStateString = "Idle";

    public override AIStateType GetStateType()
    {
        return AIStateType.Idle;
    }

    public override AIState InitializeState(AIAgent agent)
    {
        IdleState idleState = new IdleState();
        idleState.SetIdleConfig(IdleStateString);
        return idleState;
    }
}
