using System;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => (float)currentHealth / (float)maxHealth;

    private void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    public void TakeDamage(float damage, Vector3 position, Vector3 normal, GameObject damageSource = null,
        IDamageable.DamageCallback callback = null)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        bool isDead = currentHealth <= 0;
        callback?.Invoke(this, currentHealth, isDead);
        if (isDead)
        {
            Debug.Log("Player died!");
            OnPlayerDeath?.Invoke();
        }
    }

    public void Heal(float healing, IDamageable.DamageCallback callback = null)
    {
        currentHealth += healing;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        callback?.Invoke(this, currentHealth, false);
    }
    
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}