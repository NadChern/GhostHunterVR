using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HuntingBallSpawner : MonoBehaviour
{
    [Header("Hunter Ball Settings")] [SerializeField]
    private GameObject hunterBallPrefab;

    [SerializeField] private Transform leftController;
    [SerializeField] private Transform player;

    [Header("Throw Settings")] [SerializeField]
    private float throwDistance = 6f;

    [Header("Recall Settings")] [SerializeField]
    private float returnTime = 1f;

    [SerializeField] private AnimationCurve recallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector3 recallControlOffset = new Vector3(0, 2, 0);

    [Header("Spawn Settings")] [SerializeField]
    private float spawnDistance = 4f;

    [SerializeField] private float spawnHeight = 1.36f;
    [SerializeField] private float flyDuration = 1f;
    [SerializeField] private AnimationCurve flyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private GameObject conusPrefab;
    private GameObject currentConusBall;

    // State & refs
    private GameObject heldBall;
    private Rigidbody ballRb;
    private bool isBallThrown = false;
    private bool readyToThrow = false;
    private bool isRecalling = false;
    private bool hasLanded = false;

    // NEW 
    [SerializeField] private float vacuumRadius = 3f;

    private bool isDragonBeingVacuumed = false;
    private ParticleSystem ballVFX;

    private void Start()
    {
        if (player == null)
        {
            player = Camera.main?.transform;
        }

        DragonLogic.OnDragonDeath += OnDragonDeath;
    }

    private void OnDestroy()
    {
        DragonLogic.OnDragonDeath -= OnDragonDeath;
    }

    public void SpawnHunterBall()
    {
        StartCoroutine(SpawnAndFlyToHand());
    }

    private IEnumerator SpawnAndFlyToHand()
    {
        Vector3 spawnPosition = player.position +
                                player.forward * spawnDistance +
                                Vector3.up * spawnHeight;

        heldBall = Instantiate(hunterBallPrefab, spawnPosition, Quaternion.identity);

        // get rigidbody from prefab
        ballRb = heldBall.GetComponent<Rigidbody>();

        // collision handler
        heldBall.GetComponent<BallCollisionHandler>().Initialize(this);

        yield return StartCoroutine(FlyToController(heldBall, spawnPosition));
    }

    private IEnumerator FlyToController(GameObject ball, Vector3 startPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime < flyDuration)
        {
            float t = elapsedTime / flyDuration;
            float curveValue = flyCurve.Evaluate(t);

            ball.transform.position = Vector3.Lerp(startPos, leftController.position, curveValue);
            ball.transform.Rotate(0, 360f * Time.deltaTime, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.position = leftController.position;
        ball.transform.rotation = leftController.rotation;
        AttachToController(ball);
    }

    private void AttachToController(GameObject ball)
    {
        ballVFX = ball.GetComponentInChildren<ParticleSystem>();

        if (ballVFX != null)
        {
            ballVFX.Stop();
        }

        ball.transform.SetParent(leftController);
        ball.transform.localPosition = Vector3.zero;

        // Reset all states
        isBallThrown = false;
        hasLanded = false;
        isRecalling = false;

        // Reset vacuum states
        isDragonBeingVacuumed = false;

        Debug.Log("[HunterBallSpawner] Ball attached to controller!");
    }

    public void SetReadyToThrow(bool state) => readyToThrow = state;

    public void ThrowHunterBall(InputAction.CallbackContext action)
    {
        if (!action.performed) return;

        // Throw the ball if it's in hand and ready
        if (heldBall != null && readyToThrow && !isBallThrown && !isRecalling)
        {
            LaunchBallPhysics();
            return;
        }

        // Recall the ball if it's thrown and landed
        if (isBallThrown && hasLanded && !isRecalling && !isDragonBeingVacuumed)
        {
            StartCoroutine(RecallRoutine());
        }
        else if (isDragonBeingVacuumed)
        {
            Debug.Log("[HunterBallSpawner] Cannot recall - dragon is being vacuumed!");
        }
    }

    public void LaunchBallPhysics()
    {
        heldBall.transform.SetParent(null);
        ballRb.isKinematic = false;
        isBallThrown = true;

        if (ballVFX != null)
        {
            ballVFX.Play();
        }

        ballRb.AddForce(leftController.forward * throwDistance, ForceMode.Impulse);
    }

    private IEnumerator RecallRoutine()
    {
        if (isRecalling) yield break;

        currentConusBall.GetComponent<Animator>().SetTrigger("scaleDown");
        Destroy(currentConusBall, 0.5f); // delete con from ball before flying to hand

        isRecalling = true;

        Vector3 startPos = heldBall.transform.position;
        Vector3 controlPos = startPos + recallControlOffset;

        float elapsed = 0f;

        Debug.Log("[HunterBallSpawner] Recalling ball...");

        while (elapsed < returnTime)
        {
            if (leftController == null) break;

            Vector3 endPos = leftController.position;

            float t = elapsed / returnTime;
            float curveT = recallCurve.Evaluate(t);

            // Quadratic Bézier curve for smooth recall
            Vector3 pos = Mathf.Pow(1 - curveT, 2) * startPos +
                          2 * (1 - curveT) * curveT * controlPos +
                          Mathf.Pow(curveT, 2) * endPos;

            heldBall.transform.position = pos;
            heldBall.transform.LookAt(endPos);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reattach to controller
        AttachToController(heldBall);
        Debug.Log("[HunterBallSpawner] Ball recalled successfully!");
    }

    // Called by BallCollisionHandler 
    public void OnBallLanded(Vector3 landingPoint)
    {
        if (!hasLanded)
        {
            hasLanded = true;

            Debug.Log("[HunterBallSpawner] Ball collision landing at: " + landingPoint);
            currentConusBall = Instantiate(conusPrefab, landingPoint, conusPrefab.transform.rotation);
            CheckForDragonInRange(landingPoint);
        }
    }

    private void CheckForDragonInRange(Vector3 ballPosition)
    {
        DragonLogic dragon = FindFirstObjectByType<DragonLogic>();

        if (dragon != null && dragon.IsInVacuumRange(ballPosition, vacuumRadius))
        {
            Debug.Log("[HuntingBallSpawner] Dragon found in range!");
            isDragonBeingVacuumed = true;
            dragon.GetVacuumedByBall(ballPosition);
        }
        else if (dragon != null)
        {
            Debug.Log("[HuntingBallSpawner] Dragon too far from ball for vacuum.");
        }
    }

    private void OnDragonDeath(DragonLogic dragon)
    {
        isDragonBeingVacuumed = false;

        if (isBallThrown && hasLanded && !isRecalling)
        {
            Debug.Log("[HunterBallSpawner] Auto-recalling ball after dragon death");
            StartCoroutine(RecallRoutine());
        }
    }
}