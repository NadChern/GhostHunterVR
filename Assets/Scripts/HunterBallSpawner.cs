using System.Collections;
using UnityEngine;

public class HunterBallSpawner : MonoBehaviour
{
    [Header("Hunter Ball Settings")] [SerializeField]
    private GameObject hunterBallPrefab;

    [SerializeField] private Transform leftController;
    [SerializeField] private Transform playerCamera;

    [Header("Animation Settings")] [SerializeField]
    private float spawnDistance = 3f; // distance in front of player

    [SerializeField] private float spawnHeight = 1f; // height offset
    [SerializeField] private float flyDuration = 4f; // time to fly to hand
    [SerializeField] private AnimationCurve flyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private GameObject spawnedBall;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main?.transform;
        }
    }

    // Method for UnityEvents 
    public void SpawnHunterBall()
    {
        if (hunterBallPrefab == null || playerCamera == null)
        {
            Debug.LogError("[HunterBallSpawner] Missing prefab or player camera!");
            return;
        }

        StartCoroutine(SpawnAndFlyToHand());
    }

    private IEnumerator SpawnAndFlyToHand()
    {
        Vector3 spawnPosition = playerCamera.position +
                                playerCamera.forward * spawnDistance +
                                Vector3.up * spawnHeight;

        spawnedBall = Instantiate(hunterBallPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("[HunterBallSpawner] Hunter ball spawned, flying to controller...");

        // Fly to players controller
        yield return StartCoroutine(FlyToController(spawnedBall, spawnPosition));

        // Attach to controller, enable interaction
        AttachToController(spawnedBall);
    }

    private IEnumerator FlyToController(GameObject ball, Vector3 startPos)
    {
        float elapsedTime = 0f;
        Vector3 targetPos = leftController != null ? leftController.position : playerCamera.position;

        while (elapsedTime < flyDuration)
        {
            float t = elapsedTime / flyDuration;
            float curveValue = flyCurve.Evaluate(t);

            // Update target position in case controller moved
            if (leftController != null)
                targetPos = leftController.position;

            // Smooth movement with curve
            ball.transform.position = Vector3.Lerp(startPos, targetPos, curveValue);

            // Add rotation during flight
            ball.transform.Rotate(0, 360f * Time.deltaTime, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position
        ball.transform.position = targetPos;
    }

    private void AttachToController(GameObject ball)
    {
        if (leftController != null)
        {
            // Parent to controller for movement tracking
            ball.transform.SetParent(leftController);
            ball.transform.localPosition = Vector3.zero;
        }

        Debug.Log("[HunterBallSpawner] Hunter ball attached to controller!");
    }
}