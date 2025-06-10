using System.Collections;
using UnityEngine;

// Dragon spawner that waits for the initial delay before spawning a single dragon
// Implements ISpawnable interface like other spawners
public class DragonSpawner : MonoBehaviour, ISpawnable
{
    [SerializeField] private GameObject dragonPrefab;
    [SerializeField] private Transform spawnPoint;

    private WaveSettings currentWaveSettings;
    private Coroutine spawnCoroutine;
    private GameObject currentDragon; // track the spawned dragon

    [SerializeField] private WaveManager waveManager;

    private void Start()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart += HandleWaveStart;
        }
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart -= HandleWaveStart;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private void HandleWaveStart(int waveIndex, WaveSettings settings)
    {
        // Only spawn dragon if this wave is configured to spawn dragons
        if (settings.spawnDragon)
        {
            InitializeFromWave(settings);
            Spawn();
        }
    }

    public void InitializeFromWave(WaveSettings settings)
    {
        currentWaveSettings = settings;
    }

    public void Spawn()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(ActivateWithDelay());
    }

    private IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(currentWaveSettings.initialSpawnDelay);

        // Spawn the dragon
        GameObject dragon = Instantiate(dragonPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnCoroutine = null;
    }
}