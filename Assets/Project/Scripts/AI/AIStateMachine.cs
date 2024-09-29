using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.VersionControl.Asset;

public class AIStateMachine
{
    private AIState currentState;
    private Dictionary<AIStateType, AIState> stateDictionary;  

    public AIStateMachine(Dictionary<AIStateType, AIState> states)
    {
        stateDictionary = states;
    }

    public void Initialize(AIStateType startingState, AIAgent agent)
    {
        currentState = stateDictionary[startingState];
        currentState.EnterState(agent);
    }

    public void ChangeState(AIStateType newState, AIAgent agent)
    {
        if (stateDictionary.ContainsKey(newState))
        {
            currentState.ExitState(agent);
            currentState = stateDictionary[newState];
            currentState.EnterState(agent);
        }
    }

    public void Update(AIAgent agent)
    {
        currentState.UpdateState(agent);
    }

    public AIStateType GetCurrentStateType()
    {
        return currentState.GetStateType();
    }

    public bool HasState(AIStateType stateType)
    {
        return stateDictionary.ContainsKey(stateType);
    }

    public void DrawGizmos(AIAgent agent)
    {
        currentState.DrawGizmos(agent);
    }

}
