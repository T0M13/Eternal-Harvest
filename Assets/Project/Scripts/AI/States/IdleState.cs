using UnityEngine;

public class IdleState : AIState
{
    private string idleStateString;

    public override void EnterState(AIAgent agent)
    {
        if (agent.AiAnimator != null && !string.IsNullOrEmpty(idleStateString))
        {
            agent.AiAnimator.Play(idleStateString);  
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

    public void SetIdleConfig(string idleStateString)
    {
        this.idleStateString = idleStateString;
    }
}
