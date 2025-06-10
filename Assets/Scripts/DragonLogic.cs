using System;
using System.Collections;
using UnityEngine;

public class DragonLogic : MonoBehaviour
{
    // Static event: help to subscribe once, be notified any time any dragon dies
    public static event Action<DragonLogic> OnDragonDeath;

    [Header("Flight Settings")] [SerializeField]
    private float flightSpeed = 5f;

    [SerializeField] private float rotationSpeed = 2f;

    [Header("Waypoint Flight Settings")] private Transform[] waypoints; // 4 transforms in inspector
    [SerializeField] private float waypointReachDistance = 1f; // how close to get to waypoint
    [SerializeField] private float flightHeight = 3f; // fixed height for flying
    [SerializeField] private float movementSmoothness = 5f; // how smooth the movement is

    [Header("Vacuum Settings")] [SerializeField]
    private float pullTowardsBallDuration = 1f; // pull horiz toward ball

    [SerializeField] private float vacuumDownDuration = 1.5f; // vacuum down into ball
    [SerializeField] private AnimationCurve shrinkCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private Vector3 target;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 originalScale;
    private bool isDead = false;
    private bool isBeingVacuumed = false;
    private bool isFlying = false;
    public bool IsDead => isDead;
    private int currentWaypointIndex = 0;
    private Coroutine currentVacuumCoroutine = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        isFlying = false;

        if (WaypointManager.Instance != null)
        {
            waypoints = WaypointManager.Instance.GetDragonWaypoints();
            if (waypoints != null && waypoints.Length > 0)
            {
                currentWaypointIndex = 0;
            }
        }
    }

    // Called by Animation Event when sit animation completes
    public void OnSitAnimationComplete()
    {
        isFlying = true;
        if (!isDead && !isBeingVacuumed && isFlying)
        {
            FlyToWaypoints();
        }
    }

    private void Update()
    {
        if (!isDead && !isBeingVacuumed && isFlying)
        {
            FlyToWaypoints();
        }
    }


    private void FlyToWaypoints()
    {
        Debug.Log("Flying to waypoints");

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.Log("No waypoints assigned to dragon!");
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        Vector3 targetPosition = new Vector3(
            waypoints[currentWaypointIndex].position.x,
            flightHeight,
            waypoints[currentWaypointIndex].position.z
        );

        // Calculate horizontal distance (ignore height difference)
        Vector2 dragonPosXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 waypointPosXZ = new Vector2(targetPosition.x, targetPosition.z);
        float horizontalDistance = Vector2.Distance(dragonPosXZ, waypointPosXZ);

        // Movement - fixed approach similar to GhostLogic
        if (horizontalDistance > waypointReachDistance)
        {
            // Calculate desired velocity towards target
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 targetVelocity = direction * flightSpeed;

            // Check if dragon is moving in the right direction
            float dotProduct = Vector3.Dot(transform.forward, direction);

            Debug.Log($"Dot product (forward vs direction): {dotProduct}");
            if (dotProduct < 0)
            {
                Debug.LogWarning($"Dragon is facing AWAY from target! Dot product: {dotProduct}");
            }

            rb.linearVelocity =
                Vector3.Lerp(rb.linearVelocity, targetVelocity, movementSmoothness * Time.fixedDeltaTime);

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                Debug.Log($"Applied rotation: {transform.rotation.eulerAngles}");
            }
        }
        else
        {
            // Slow down when reaching waypoint
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, movementSmoothness * Time.fixedDeltaTime);

            // Check if actually close enough and velocity is low
            if (rb.linearVelocity.magnitude < 1f)
            {
                Debug.Log($"Reached waypoint {currentWaypointIndex}, selecting next waypoint");
                SelectNextWaypoint();
            }
        }
    }


    private void SelectNextWaypoint()
    {
        // Sequential waypoints: 0 → 1 → 2 → 3 → 0 → 1... (infinite loop)
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }


    public bool IsInVacuumRange(Vector3 ballPosition, float vacuumRadius)
    {
        if (isDead || isBeingVacuumed) return false;
        return Vector3.Distance(transform.position, ballPosition) <= vacuumRadius;
    }

    public void GetVacuumedByBall(Vector3 ballPosition)
    {
        if (isDead || isBeingVacuumed || currentVacuumCoroutine != null) return;
        isFlying = false;
        currentVacuumCoroutine = StartCoroutine(VacuumSequence(ballPosition));
    }

    private IEnumerator VacuumSequence(Vector3 ballPosition)
    {
        isBeingVacuumed = true;
        animator.SetBool("IsVaccumed", true);

        // Pull toward ball (at current height)
        yield return StartCoroutine(PullTowardsBallPhase(ballPosition));

        // Vacuum down into ball with shrinking effect
        yield return StartCoroutine(VacuumDownPhase(ballPosition));

        Die();
    }

    private IEnumerator PullTowardsBallPhase(Vector3 ballPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition =
            new Vector3(ballPosition.x, startPosition.y,
                ballPosition.z); // target is ball position but at dragon's current height

        float elapsedTime = 0f;
        while (elapsedTime < pullTowardsBallDuration)
        {
            float t = elapsedTime / pullTowardsBallDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t); // move horiz toward ball
            transform.localScale = originalScale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator VacuumDownPhase(Vector3 ballPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < vacuumDownDuration)
        {
            float t = elapsedTime / vacuumDownDuration;
            transform.position = Vector3.Lerp(startPosition, ballPosition, t);

            // Shrink 
            float scaleMultiplier = shrinkCurve.Evaluate(t);
            Vector3 currentScale = originalScale * scaleMultiplier;
            transform.localScale = currentScale;

            // Spin
            transform.Rotate(0, 360f * Time.deltaTime * 3, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        transform.position = ballPosition;
    }

    private void Die()
    {
        isDead = true;
        isBeingVacuumed = false;
        currentVacuumCoroutine = null;

        // Stop all movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Disable collider
        Collider dragonCollider = GetComponent<Collider>();
        if (dragonCollider != null)
        {
            dragonCollider.enabled = false;
        }

        // Notify GameManager
        OnDragonDeath?.Invoke(this);

        // Destroy after short delay
        Destroy(gameObject, 1f);
    }

    public void ResetScale()
    {
        transform.localScale = originalScale;
    }
}