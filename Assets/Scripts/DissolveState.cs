using UnityEngine;

public class DissolveState : IGhostState
{
    private float timer;

    public void EnterState(GhostBehaviour ghost)
    {
        timer = 2f;
        ghost.ghost.PlayAnimation("Dissolve");
    }

    public void UpdateState(GhostBehaviour ghost)
    {
        timer -= Time.fixedDeltaTime;
        if (timer <= 0)
        {
            GameObject.Destroy(ghost.gameObject);
        }
    }

    public void ExitState(GhostBehaviour ghost)
    {
    }
}