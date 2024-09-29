using UnityEngine;

public abstract class AIStateConfig : ScriptableObject
{
    public abstract AIStateType GetStateType();
    public abstract AIState InitializeState(AIAgent agent);
}
