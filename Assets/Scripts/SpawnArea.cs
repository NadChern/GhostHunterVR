using System.Collections;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public GameObject ghost;
    public Vector3 volumeSpawn; 
    public float delaySpawn = 1f;
    public int counter = 10;
    
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
        Quaternion spawnRot = Quaternion.LookRotation(Vector3.forward); ; // TO FIX!
        Instantiate(ghost, spawnPos, spawnRot);
    }
}
