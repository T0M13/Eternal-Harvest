using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AIStateMachine stateMachine;
    [SerializeField] private Animator aiAnimator;
    [SerializeField] private BoxCollider2D aiCollider;

    [Header("State Configurations")]
    [SerializeField] private List<AIStateConfig> stateConfigs;
    [SerializeField][ShowOnly] private AIStateType currentStateType;

    [Header("Movement")]
    public Vector2 movementVector;
    private float moveSpeed;

    public AIStateMachine StateMachine { get => stateMachine; set => stateMachine = value; }
    public Animator AiAnimator { get => aiAnimator; set => aiAnimator = value; }
    public BoxCollider2D AiCollider { get => aiCollider; set => aiCollider = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

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

    private void OnDrawGizmosSelected()
    {
        if (stateMachine != null)
        {
            stateMachine.DrawGizmos(this);
        }
    }
}
