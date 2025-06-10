using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameplaySettings gameplaySettings;
    [SerializeField] private DragonSpawner dragonSpawner;
    public event Action<int, WaveSettings> OnWaveStart;
    public event Action<int> OnWaveEnd;
    public List<UnityEvent> waveEvents;

    private int currentWave = -1;

    public int CurrentWave => currentWave;

    public WaveSettings GetCurrentWaveSettings() => gameplaySettings.GetWaveSettings(currentWave);

    public void StartFirstWave()
    {
        currentWave = 0;
        waveEvents[0]?.Invoke(); // sound effect etc
        Debug.Log($"Wave {currentWave} started (from StartFirstWave)");
        OnWaveStart?.Invoke(currentWave, gameplaySettings.GetWaveSettings(currentWave));
    }

    public void AdvanceWave()
    {
        OnWaveEnd?.Invoke(currentWave);
        currentWave++;
        waveEvents[currentWave]?.Invoke();
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

    public void SetCurrentWave(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < gameplaySettings.MaxWaves)
        {
            currentWave = waveIndex;
            Debug.Log($"Wave restored to: {currentWave}");

            OnWaveStart?.Invoke(currentWave, gameplaySettings.GetWaveSettings(currentWave));
        }
    }
}