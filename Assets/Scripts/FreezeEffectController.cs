using System.Collections.Generic;
using UnityEngine;

public class FreezeEffectController : MonoBehaviour
{
    public static FreezeEffectController Instance { get; private set; }

    [SerializeField] private Transform player;
    [SerializeField] private Renderer freezeQuadRenderer;
    [SerializeField] private float effectRadius = 7f;
    [SerializeField] private float noEffect = 14f;
    [SerializeField] private float fullEffect = 7f;

    [SerializeField] private float freezeBuildDuration = 1.5f; // seconds to reach full freeze
    [SerializeField] private float smoothingSpeed = 8f;
    [SerializeField] private float checkInterval = 0.1f; // Check for nearby ghosts every 0.1 seconds

    private Material freezeMaterial;
    private HashSet<GhostLogic> trackedGhosts = new HashSet<GhostLogic>(); // all active ghosts
    private HashSet<GhostLogic> nearbyGhosts = new HashSet<GhostLogic>(); // ghosts in the range

    private float currentSideValue;
    private float fadeOutTimer = 0f;
    private float freezeTimer = 0f;
    private float lastCheckTime = 0f;

    [SerializeField] private float fadeOutDuration = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (player == null)
        {
            player = Camera.main?.transform;
        }

        if (freezeQuadRenderer != null)
        {
            freezeMaterial = freezeQuadRenderer.material;
        }

        currentSideValue = noEffect;
        if (freezeMaterial != null)
        {
            freezeMaterial.SetFloat("_Sides", currentSideValue);
        }
    }

    private void Update()
    {
        if (freezeMaterial == null || player == null) return;
        if (Time.time - lastCheckTime > checkInterval)
        {
            UpdateNearbyGhosts();
            lastCheckTime = Time.time;
        }

        UpdateFreezeEffect();
    }


    private void UpdateNearbyGhosts()
    {
        trackedGhosts.RemoveWhere(g => g == null || !g.gameObject.activeInHierarchy || g.IsDead);
        nearbyGhosts.Clear();

        foreach (GhostLogic ghost in trackedGhosts)
        {
            float distance = Vector3.Distance(player.position, ghost.transform.position);
            if (distance <= effectRadius)
            {
                nearbyGhosts.Add(ghost);
            }
        }
    }

    private void UpdateFreezeEffect()
    {
        bool shouldFreeze = nearbyGhosts.Count > 0;
        float targetValue;

        if (shouldFreeze)
        {
            fadeOutTimer = 0f;
            freezeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(freezeTimer / freezeBuildDuration);
            targetValue = Mathf.Lerp(noEffect, fullEffect, t);
        }
        else
        {
            fadeOutTimer += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(fadeOutTimer / fadeOutDuration);
            freezeTimer = Mathf.Lerp(freezeTimer, 0f, fadeProgress);
            float remainingFreeze = Mathf.Clamp01(freezeTimer / freezeBuildDuration);
            targetValue = Mathf.Lerp(noEffect, fullEffect, remainingFreeze);
        }

        currentSideValue = Mathf.Lerp(currentSideValue, targetValue, Time.deltaTime * smoothingSpeed);
        freezeMaterial.SetFloat("_Sides", currentSideValue);
    }


    public void RegisterGhost(GhostLogic ghost)
    {
        if (ghost != null)
        {
            trackedGhosts.Add(ghost);
        }
    }

    public void UnregisterGhost(GhostLogic ghost)
    {
        if (ghost != null)
        {
            trackedGhosts.Remove(ghost);
            nearbyGhosts.Remove(ghost);
        }
    }
}