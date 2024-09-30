using UnityEngine;

public class IdleState : AIState
{
    public IdleStateConfig Config;

    public override void EnterState(AIAgent agent)
    {
        if (agent.AiAnimator != null && !string.IsNullOrEmpty(Config.IdleStateString))
        {
            agent.AiAnimator.Play(Config.IdleStateString);  
        }
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.Idle;
    }

    public override void DrawGizmos(AIAgent agent)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(agent.transform.position, 0.5f);
    }
}
