using System.Collections.Generic;
using UnityEngine;

// Object pool pattern helps by reusing ghosts instead of destroying/
// instantiating them repeatedly (increase performance)
// Singleton pattern ensures only one GhostPool exists in the scene
public class GhostPool : MonoBehaviour
{
       public static GhostPool Instance { get; private set; }
       [SerializeField] private GameObject ghostPrefab;
       [SerializeField] private int initialPoolSize = 20;
       private Queue<GameObject> ghostPool = new Queue<GameObject>();

       private void Awake()
       {
              // Singleton pattern (ensures one GhostPool in the scene)
              // if another instance already exists, destroy duplicate
              if (Instance != null && Instance != this)
              {
                     Destroy(gameObject);
                     return;
              }

              Instance = this;
              for (int i = 0; i < initialPoolSize; i++)
              {
                     CreateGhost();
              }
       }

       private GameObject CreateGhost()
       {
              // create ghost from prefab
              GameObject ghost = Instantiate(ghostPrefab);

              // keep ghost hidden (inactive)
              ghost.SetActive(false);

              // add ghost to pool (to reuse it later)
              ghostPool.Enqueue(ghost);
              return ghost;
       }

       public GameObject GetGhost(Vector3 position, Quaternion rotation, WaveSettings waveSettings)
       {
              GameObject ghost = ghostPool.Count > 0 ? ghostPool.Dequeue() : CreateGhost();

              // setup ghosts for use
              ghost.transform.position = position;
              ghost.transform.rotation = rotation;
              ghost.SetActive(true);

              GhostLogic ghostLogic = ghost.GetComponent<GhostLogic>();
              if (ghostLogic != null)
              {
                     ghostLogic.ResetGhost(); // reset rigidbody, health etc
                     ghostLogic.ApplySettings(waveSettings); // apply wave-specific speed, health etc
              }

              return ghost;
       }

       public void ReturnGhost(GameObject ghost)
       {
              if (ghost == null) return;
              ghost.SetActive(false);
              ghostPool.Enqueue(ghost);
       }
}