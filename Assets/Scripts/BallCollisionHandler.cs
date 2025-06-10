using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    [SerializeField] private string landableSurface = "Horizontal";
    [SerializeField] private float minTreshold = 0.23f;

    private HuntingBallSpawner spawner;
    private Rigidbody rb;
    private bool firstCollisionDetection = false;

    public void Initialize(HuntingBallSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.isKinematic || !firstCollisionDetection) return;

        if (rb.linearVelocity.magnitude < minTreshold)
        {
            rb.isKinematic = true;
            spawner.OnBallLanded(transform.position);
            firstCollisionDetection = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb.isKinematic || !collision.collider.CompareTag(landableSurface)) return;
        firstCollisionDetection = true;
    }
}