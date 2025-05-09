using UnityEngine;

// State Machine for Ghost
// (manages current state and transitions between them)

[RequireComponent(typeof(GhostLogic))]
public class GhostBehaviour : MonoBehaviour
{
    private IGhostState _currentState;
    public GhostLogic ghost;

    public RoamState RoamState = new RoamState();
    public ChaseState ChaseState = new ChaseState();
    public DissolveState DissolveState = new DissolveState();

    private void Awake()
    {
        ghost = GetComponent<GhostLogic>();
    }

    private void Start()
    {
        SwitchState(RoamState);
    }

    private void FixedUpdate()
    {
        _currentState?.UpdateState(this);
    }

    public void SwitchState(IGhostState newState)
    {
        _currentState?.ExitState(this); // exit old state
        _currentState = newState; // set new state
        _currentState?.EnterState(this); // start new state behavior
    }
}