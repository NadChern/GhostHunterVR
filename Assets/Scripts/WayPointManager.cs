using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance;

    [SerializeField] private Transform[] dragonWaypoints;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform[] GetDragonWaypoints()
    {
        return dragonWaypoints;
    }
}