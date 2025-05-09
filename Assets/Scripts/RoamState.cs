using UnityEngine;

public class RoamState : IGhostState
    {
        private Vector3 roamTarget;
        private float timer;
        private float roamRadius = 3f;
        private float roamDurationMin = 2f;
        private float roamDurationMax = 4f;
        private float chaseDistance = 7f;

        public void EnterState(GhostBehaviour ghost)
        {
            roamTarget = ghost.transform.position + Random.insideUnitSphere * roamRadius;
            roamTarget.y = ghost.transform.position.y; // maintain altitude
            timer = Random.Range(roamDurationMin, roamDurationMax);

            ghost.ghost.PlayAnimation("Idle");
        }

        public void UpdateState(GhostBehaviour ghost)
        {
            ghost.ghost.HoverAround(roamTarget);
            timer -= Time.fixedDeltaTime;

            // Reset roaming target
            if (timer <= 0)
            {
                EnterState(ghost); // Pick a new random point
            }

            // Check distance to player
            if (ghost.ghost.player != null &&
                Vector3.Distance(ghost.transform.position, ghost.ghost.player.position) < chaseDistance)
            {
                ghost.SwitchState(ghost.ChaseState);
            }
        }

        public void ExitState(GhostBehaviour ghost) {}
     
    }


