using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameplaySettings gameplaySettings;

    public event Action<int, WaveSettings> OnWaveStart;
    public event Action<int> OnWaveEnd;
    public List<UnityEvent> waveEvents;

    private int currentWave = -1;
    private bool dragonSpawnedThisWave = false;
    private const float DRAGON_SPAWN_PERCENT = 0.75f;

    public int CurrentWave => currentWave;
    public bool DragonSpawned => dragonSpawnedThisWave;

    public WaveSettings GetCurrentWaveSettings() => gameplaySettings.GetWaveSettings(currentWave);

    public void StartFirstWave()
    {
        currentWave = 0;
        dragonSpawnedThisWave = false;
        waveEvents[0]?.Invoke(); // sound effect etc
        Debug.Log($"Wave {currentWave} started (from StartFirstWave)");
        OnWaveStart?.Invoke(currentWave, gameplaySettings.GetWaveSettings(currentWave));
    }

    public void AdvanceWave()
    {
        OnWaveEnd?.Invoke(currentWave);
        currentWave++;
        waveEvents[currentWave]?.Invoke();
        dragonSpawnedThisWave = false;
        Debug.Log($"Wave {currentWave} started (from AdvanceWave)");
        OnWaveStart?.Invoke(currentWave, gameplaySettings.GetWaveSettings(currentWave));
    }

    public float GetTimeToStartWave(int waveIndex)
    {
        float totalTime = 0f;
        for (int i = 0; i < waveIndex; i++)
        {
            totalTime += gameplaySettings.GetWaveSettings(i).waveDuration;
        }

        return totalTime;
    }

    public bool ShouldSpawnDragon(float gameTimer)
    {
        if (!gameplaySettings.GetWaveSettings(currentWave).spawnDragon || dragonSpawnedThisWave)
        {
            return false;
        }

        float timeIntoThisWave = gameTimer - GetTimeToStartWave(currentWave);
        float waveDuration = gameplaySettings.GetWaveSettings(currentWave).waveDuration;
        return timeIntoThisWave >= waveDuration * DRAGON_SPAWN_PERCENT;
    }

    public void MarkDragonSpawned()
    {
        dragonSpawnedThisWave = true;
    }
}