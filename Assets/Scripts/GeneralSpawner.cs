using UnityEngine;

// Used for simple one-shot game objects that don't need wave-specific behaviors.
// Implements ISpawnable interface.
public class GeneralSpawner : MonoBehaviour, ISpawnable
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform spawnPoint;

    private WaveSettings currentWaveSetting;

    public void InitializeFromWave(WaveSettings settings)
    {
        currentWaveSetting = settings;
    }

    public void Spawn()
    {
        if (prefab == null || spawnPoint == null)
        {
            return;
        }

        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"[GeneralSpawner] Spawned {prefab.name} at {spawnPoint.position}");
    }
}