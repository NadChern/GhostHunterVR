using System;
using UnityEngine;

// Ghost Physics, Damage, Attack Player, Animation
// Implements IDamageable, handles hovering, flying physics, death logic

[RequireComponent(typeof(Rigidbody))]
public class GhostLogic : MonoBehaviour, IDamageable
{
    // Static event: help to subscribe once,
    // be notified any time any ghost dies (vs ref to every ghost)
    public static event Action<GhostLogic> OnGhostDeath;

    // Wave settings
    private float maxVelocity;
    private float maxAcceleration;
    private float baseHealth;
    private float chaseDistance;
    private float attackCooldown;
    private float attackDamage;

    [SerializeField] private float hoverRadius = 0.5f;
    [SerializeField] private Vector3 hoverNoiseFrequency = new Vector3(1f, 1f, 1f);

    private float currentHealth;

    public float AttackCooldown => attackCooldown;
    public bool IsDead => currentHealth <= 0;

    public Rigidbody rb { get; private set; }
    public Transform player { get; private set; }
    public Animator animator { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        player = Camera.main?.transform;
        Debug.Log(player);
    }

    public void ApplySettings(WaveSettings waveSettings)
    {
        maxVelocity = waveSettings.ghostMaxVelocity;
        maxAcceleration = waveSettings.ghostMaxAcceleration;
        baseHealth = waveSettings.ghostHealth;
        attackDamage = waveSettings.ghostAttackDamage;
        attackCooldown = waveSettings.ghostAttackCooldown;
        chaseDistance = waveSettings.ghostChaseDistance;
        currentHealth = baseHealth;
    }

    public void HoverAround(Vector3 target)
    {
        Vector3 noise = new Vector3(
            Mathf.PerlinNoise(Time.time * hoverNoiseFrequency.x, 0) - 0.5f,
            Mathf.PerlinNoise(Time.time * hoverNoiseFrequency.y, 1) - 0.5f,
            Mathf.PerlinNoise(Time.time * hoverNoiseFrequency.z, 2) - 0.5f
        ) * hoverRadius;

        FlyTo(target + noise);
    }

    public void FlyTo(Vector3 target)
    {
        Vector3 targetVel = target - rb.position;
        targetVel = Vector3.ClampMagnitude(targetVel, maxVelocity);
        Vector3 accel = (targetVel - rb.linearVelocity) / Time.fixedDeltaTime;

        if (accel.magnitude > maxAcceleration)
            accel = accel.normalized * maxAcceleration;

        rb.AddForce(accel, ForceMode.Acceleration);
    }

    // Attack player 
    public void AttackPlayer()
    {
        if (player == null) return;

        IDamageable playerHealth = player.GetComponent<IDamageable>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage, player.position, Vector3.zero);
        }
    }

    public void PlayAnimation(string triggerName)
    {
        if (animator != null)
            animator.SetTrigger(triggerName);
    }


    // Checks if player is within chase distance 
    public bool IsPlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= chaseDistance;
    }

    public void TakeDamage(float damage, Vector3 position, Vector3 normal, GameObject damageSource = null,
        IDamageable.DamageCallback callback = null)
    {
        currentHealth -= damage;
        Debug.Log($"{name} took {damage} damage. Remaining HP: {currentHealth}");
        callback?.Invoke(this, currentHealth, IsDead);

        if (IsDead)
        {
            GetComponent<Collider>().enabled = false;
            OnGhostDeath?.Invoke(this); // notify GameManager
            Debug.Log($"{name} is dead. Switching to DissolveState.");
            GetComponent<GhostBehaviour>().SwitchState(new DissolveState());
        }
    }

    public void Heal(float healing, IDamageable.DamageCallback callback = null)
    {
        currentHealth += healing;
        callback?.Invoke(this, currentHealth, false);
    }

    public void ResetGhost()
    {
        // reset health
        currentHealth = baseHealth;

        // reset rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // re-enable collider
        Collider ghostCollider = GetComponent<Collider>();
        if (ghostCollider != null)
        {
            ghostCollider.enabled = true;
        }
    }
}