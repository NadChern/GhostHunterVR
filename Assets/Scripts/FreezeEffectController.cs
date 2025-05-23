using UnityEngine;

public class FreezeEffectController : MonoBehaviour
{
    public static FreezeEffectController Instance { get; private set; }

    [SerializeField] private Transform player;
    [SerializeField] private Renderer freezeQuadRenderer;
    [SerializeField] private float effectRadius = 5f;
    [SerializeField] private float startEffect = 50f;
    [SerializeField] private float fullEffect = 12f;
    [SerializeField] private float noEffect = 120f;
    private Material freezeMaterial;
    private GhostLogic targetGhost;
    private float currentSideValue;

    [SerializeField] private float freezeBuildDuration = 3f; // seconds to reach full freeze
    [SerializeField] private float smoothingSpeed = 5f;

    private bool isFreezing = false;
    private float freezeTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (player == null)
            player = transform; // auto-assign if on main camera

        if (freezeQuadRenderer != null)
            freezeMaterial = freezeQuadRenderer.material;
    }

    public void SetTargetGhost(GhostLogic ghost)
    {
        targetGhost = ghost;
    }

    void Update()
    {
        if (freezeMaterial == null || player == null)
            return;

        // If ghost is dead or unassigned, fade out effect
        if (targetGhost == null || targetGhost.IsDead)
        {
            isFreezing = false;
            freezeTimer = 0f;
            currentSideValue = Mathf.Lerp(currentSideValue, noEffect, Time.deltaTime * smoothingSpeed);
            freezeMaterial.SetFloat("_Sides", currentSideValue);
            return;
        }

        float distance = Vector3.Distance(player.position, targetGhost.transform.position);

        // If within range, start freezing
        if (distance <= effectRadius)
        {
            isFreezing = true;
        }
        else
        {
            isFreezing = false;
        }

        if (isFreezing)
        {
            // Build up the freeze over time
            freezeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(freezeTimer / freezeBuildDuration);
            float targetValue = Mathf.Lerp(startEffect, fullEffect, t);
            currentSideValue = Mathf.Lerp(currentSideValue, targetValue, Time.deltaTime * smoothingSpeed);
        }
        else
        {
            // Fade out if ghost is far
            freezeTimer = 0f;
            currentSideValue = Mathf.Lerp(currentSideValue, noEffect, Time.deltaTime * smoothingSpeed);
        }

        freezeMaterial.SetFloat("_Sides", currentSideValue);
    }
}