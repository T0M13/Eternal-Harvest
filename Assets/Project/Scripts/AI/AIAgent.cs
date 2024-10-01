using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AIStateMachine stateMachine;
    [SerializeField] private Animator aiAnimator;
    [SerializeField] private BoxCollider2D aiCollider;
    [SerializeField] private MapGenerator mapGenerator;

    [Header("State Configurations")]
    [SerializeField] private List<AIStateConfig> stateConfigs;
    [SerializeField][ShowOnly] private AIStateType currentStateType;
    [SerializeField][ShowOnly] private bool statePaused = false;

    public AIStateMachine StateMachine { get => stateMachine; set => stateMachine = value; }
    public Animator AiAnimator { get => aiAnimator; set => aiAnimator = value; }
    public BoxCollider2D AiCollider { get => aiCollider; set => aiCollider = value; }
    public MapGenerator MapGenerator { get => mapGenerator; set => mapGenerator = value; }

    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
        InitializeStates();
    }

    private void Start()
    {
        StartState();
    }

    private void Update()
    {
        if (statePaused) return;

        stateMachine.Update(this);
    }

    private void GetReferences()
    {
        if (aiAnimator == null)
        {
            try { aiAnimator = GetComponentInChildren<Animator>(); }
            catch { Debug.Log("AIAnimator Missing from AIAgent"); }
        }

        if (aiCollider == null)
        {
            try { aiCollider = GetComponent<BoxCollider2D>(); }
            catch { Debug.Log("BoxCollider2D Missing from AIAgent"); }
        }
        if (mapGenerator == null)
        {
            try { mapGenerator = FindObjectOfType<MapGenerator>(); }
            catch { Debug.Log("MapGenerator Missing from AIAgent"); }
        }

    }

    private void InitializeStates()
    {
        var states = new Dictionary<AIStateType, AIState>();

        foreach (var config in stateConfigs)
        {
            AIState state = config.InitializeState(this);
            states.Add(config.GetStateType(), state);
        }

        stateMachine = new AIStateMachine(states);
    }

    private void StartState()
    {
        currentStateType = stateConfigs[0].GetStateType();
        stateMachine.Initialize(currentStateType, this);
    }

    public void TransitionToState(AIStateType newState)
    {
        stateMachine.ChangeState(newState, this);
        currentStateType = newState;
    }

    public void StateStop()
    {
        statePaused = true;
    }

    public void StateStart()
    {
        statePaused = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (stateMachine != null)
        {
            stateMachine.DrawGizmos(this);
        }
    }
}
