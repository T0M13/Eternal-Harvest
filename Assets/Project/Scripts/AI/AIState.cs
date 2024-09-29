public abstract class AIState
{
    public virtual void EnterState(AIAgent agent) { }
    public virtual void UpdateState(AIAgent agent) { }
    public virtual void ExitState(AIAgent agent) { }
    public abstract AIStateType GetStateType();
    public virtual void DrawGizmos(AIAgent agent) { }
}
