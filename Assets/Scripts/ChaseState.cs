using UnityEngine;
    public class ChaseState : IGhostState
    {
        private float stopChaseDistance = 2f; 

        public void EnterState(GhostBehaviour ghost)
        {
            ghost.ghost.PlayAnimation("Move");
        }

        public void UpdateState(GhostBehaviour ghost)
        {
            if (ghost.ghost.player == null)
                return;

            Vector3 playerPos = ghost.ghost.player.position;
            Vector3 ghostPos = ghost.transform.position;
            float distance = Vector3.Distance(ghostPos, playerPos);
            
            Vector3 lookDirection = (playerPos - ghostPos);
            lookDirection.y = 0f; // Keep upright
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                ghost.transform.rotation = Quaternion.Slerp(ghost.transform.rotation, targetRotation, Time.deltaTime * 3f);
            }

            if (distance > stopChaseDistance)
            {
                ghost.ghost.FlyTo(playerPos); 
                // move animation add
                
            }
            else
            {
                ghost.ghost.rb.velocity = Vector3.zero; // hover in place
                // attack animation trigger
            }

            if (ghost.ghost.health <= 0)
            {
                ghost.SwitchState(ghost.DissolveState);
            }
        }

        public void ExitState(GhostBehaviour ghost) {}
    }
