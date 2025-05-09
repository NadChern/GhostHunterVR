using UnityEngine;

// Damage system interface
// can apply damage and healing to any game object (ghosts, player)
public interface IDamageable
{
    public delegate void DamageCallback(IDamageable damagableAffected, float hpAffected, bool targetDied);

    void Heal(float healing, DamageCallback callback = null);

    void TakeDamage(
        float damage,
        Vector3 position,
        Vector3 normal,
        GameObject damageSource = null,
        DamageCallback callback = null);
}