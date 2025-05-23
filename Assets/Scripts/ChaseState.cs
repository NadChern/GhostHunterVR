using UnityEngine;

public class ChaseState : IGhostState
{
    private float stopChaseDistance = 2f;
    private float attackTimer;

    public void EnterState(GhostBehaviour ghost)
    {
        ghost.ghost.PlayAnimation("Move");
        attackTimer = 0f;

        FreezeEffectController freezeController = GameObject.FindFirstObjectByType<FreezeEffectController>();
        freezeController?.SetTargetGhost(ghost.ghost);
    }

    public void UpdateState(GhostBehaviour ghost)
    {
        if (ghost.ghost.player == null)
            return;

        Vector3 playerPos = ghost.ghost.player.position;
        Vector3 ghostPos = ghost.transform.position;
        float distance = Vector3.Distance(ghostPos, playerPos);

        // look direction vector
        Vector3 lookDirection = (playerPos - ghostPos);

        // look at the player, but stay upright
        if (lookDirection != Vector3.zero)
        {
            // keep ghost upright
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            // limit up/down tilt to preserve more of the upright appearance
            Vector3 eulerAngles = targetRotation.eulerAngles;

            // convert angles to -180 to 180 range for proper clamping
            if (eulerAngles.x > 180f) eulerAngles.x -= 360f;

            // clamp the x rotation (up/down tilt) 
            eulerAngles.x = Mathf.Clamp(eulerAngles.x, -30f, 30f);
            targetRotation = Quaternion.Euler(eulerAngles);

            ghost.transform.rotation = Quaternion.Slerp(ghost.transform.rotation, targetRotation, Time.deltaTime * 3f);
        }

        if (distance > stopChaseDistance)
        {
            ghost.ghost.FlyTo(playerPos);
            ghost.ghost.PlayAnimation("Move");
        }
        else
        {
            ghost.ghost.rb.linearVelocity = Vector3.zero; // hover in place
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                ghost.ghost.PlayAnimation("Attack");
                ghost.ghost.AttackPlayer();
                attackTimer = ghost.ghost.AttackCooldown;
            }
        }

        if (ghost.ghost.IsDead)
        {
            ghost.SwitchState(ghost.DissolveState);
        }
    }


    public void ExitState(GhostBehaviour ghost)
    {
    }
}