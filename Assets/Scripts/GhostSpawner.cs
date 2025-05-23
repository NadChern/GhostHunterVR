using System.Collections;
using UnityEngine;

// Spawns ghosts with specified delay using coroutine, wave-based settings. 
// Implements ISpawnable interface
public class GhostSpawner : MonoBehaviour, ISpawnable
{
    // Wave settings
    private Vector3 volumeSpawn;
    private float delaySpawn;
    private int counter;
    private WaveSettings currentWaveSettings;
    private Coroutine spawnCoroutine; // to track active coroutine
    private float initSpawnDelay;
    
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

        // Prevent coroutine from running after object is gone
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private void HandleWaveStart(int waveIndex, WaveSettings settings)
    {
        InitializeFromWave(settings);
        Spawn();
    }

    public void InitializeFromWave(WaveSettings settings)
    {
        currentWaveSettings = settings;
        volumeSpawn = settings.volumeSpawn;
        delaySpawn = settings.spawnDelay;
        counter = settings.ghostCount;
        initSpawnDelay = settings.initialSpawnDelay;
    }


    public void Spawn()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        // initial spawn delay
        yield return new WaitForSeconds(initSpawnDelay);
        
        while (counter > 0)
        {
            SpawnGhost();
            counter--;
            yield return new WaitForSeconds(delaySpawn);
        }

        spawnCoroutine = null;
    }

    private void SpawnGhost()
    {
        float x = Random.Range(-volumeSpawn.x, volumeSpawn.x);
        float y = Random.Range(-volumeSpawn.y, volumeSpawn.y);
        float z = Random.Range(-volumeSpawn.z, volumeSpawn.z);
        Vector3 offset = new Vector3(x, y, z);

        // ghost face forward
        Vector3 spawnPos = transform.position + offset;
        Quaternion spawnRot = Quaternion.LookRotation(Vector3.forward);

        // use GhostPool
        GhostPool.Instance.GetGhost(spawnPos, spawnRot, currentWaveSettings);
    }
}