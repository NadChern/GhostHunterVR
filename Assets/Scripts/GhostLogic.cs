using UnityEngine;

// Ghost Physics, Damage and Animation
// implements IDamageable, hovering and flying physics using Rigidbody 
// control of animation via triggers

[RequireComponent(typeof(Rigidbody))]
public class GhostLogic : MonoBehaviour, IDamageable
{
    public float maxVelocity = 2f;
    public float maxAcceleration = 6f;
    public float hoverRadius = 0.5f;
    public Vector3 hoverNoiseFrequency = new Vector3(1f, 1f, 1f);
    public float health = 10f;

    public Rigidbody rb;
    public Transform player;
    public Animator animator;

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

    public void PlayAnimation(string triggerName)
    {
        if (animator != null)
            animator.SetTrigger(triggerName);
    }

    public void TakeDamage(float damage, Vector3 position, Vector3 normal, GameObject damageSource = null,
        IDamageable.DamageCallback callback = null)
    {
        health -= damage;
        Debug.Log($"{name} took {damage} damage. Remaining HP: {health}");
        bool isDead = health <= 0;

        callback?.Invoke(this, health, isDead);
        GetComponent<Collider>().enabled = false;

        if (isDead)
        {   Debug.Log($"{name} is dead. Switching to DissolveState.");
            GetComponent<GhostBehaviour>().SwitchState(new DissolveState());
        }
    }

    public void Heal(float healing, IDamageable.DamageCallback callback = null)
    {
        health += healing;
        callback?.Invoke(this, health, false);
    }
}

