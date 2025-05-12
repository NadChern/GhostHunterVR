using System.Collections;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private Vector3 volumeSpawn = new Vector3(4,4,4);
    [SerializeField] private float delaySpawn = 2.0f;
    [SerializeField] private int counter = 20;
   
    public void SpawnActivate()
    {
        StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        while(counter > 0)
        {
            Spawn();
            counter--;
            yield return new WaitForSeconds(delaySpawn);
        }
    }

    private void Spawn()
    {
        float x = Random.Range(-volumeSpawn.x, volumeSpawn.x);
        float y = Random.Range(-volumeSpawn.y, volumeSpawn.y);
        float z = Random.Range(-volumeSpawn.z, volumeSpawn.z);
        Vector3 offset = new Vector3(x,y,z);
        
        // ghost face forward
        Vector3 spawnPos = transform.position + offset; 
        Quaternion spawnRot = Quaternion.LookRotation(Vector3.forward); 
        
        // use of GhostPool
        GhostPool.Instance.GetGhost(spawnPos, spawnRot);
    }
}
