using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    [SerializeField] private int maxWaves = 3;
    [SerializeField] private float basePlayerHealth = 100f;
    [SerializeField] private WaveSettings[] waveSettings;

    public int MaxWaves => maxWaves;
    public float BasePlayerHealth => basePlayerHealth;

    public WaveSettings GetWaveSettings(int waveIndex)
    {
        // 0-wave indexing 
        // if not valid index, return default settings
        if (waveIndex < 0 || waveIndex >= maxWaves)
        {
            return GetDefaultWaveSettings();
        }

        if (waveIndex < waveSettings.Length)
        {
            return waveSettings[waveIndex];
        }

        // if settings are not defined, use defaults
        return GetDefaultWaveSettings();
    }

    private WaveSettings GetDefaultWaveSettings()
    {
        if (waveSettings != null && waveSettings.Length > 0)
        {
            return waveSettings[0];
        }

        return new WaveSettings()
        {
            ghostMaxVelocity = 2f,
            ghostMaxAcceleration = 6f,
            ghostChaseDistance = 7f,
            ghostAttackDamage = 1f,
            ghostAttackCooldown = 2,
            ghostHealth = 10f,
            spawnDelay = 2f,
            ghostCount = 20,
            volumeSpawn = new Vector3(4, 4, 4),
            spawnDragon = false,
            waveDuration = 60f,
            scoreValue = 10,
            initialSpawnDelay = 0f
        };
    }
}

// Class to define wave-specific settings (Flyweight pattern)
[Serializable]
public class WaveSettings
{
    public float ghostMaxVelocity;
    public float ghostMaxAcceleration;
    public float ghostChaseDistance;
    public float ghostAttackDamage;
    public float ghostAttackCooldown;
    public float ghostHealth;
    public float spawnDelay;
    public int ghostCount;
    public Vector3 volumeSpawn;
    public bool spawnDragon;
    public float waveDuration;
    public int scoreValue;
    public float initialSpawnDelay;
}