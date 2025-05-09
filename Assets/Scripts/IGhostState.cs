// State Pattern interface
// Defines contract for ghost states like RoamState, ChaseState, DissolveState
// Each state class implements this interface to control ghost behavior dynamically at runtime.
public interface IGhostState
{
    void EnterState(GhostBehaviour ghost);
    void UpdateState(GhostBehaviour ghost);
    void ExitState(GhostBehaviour ghost); 
}