using UnityEngine;
using UnityEngine.InputSystem;

// Input Action Event system triggers this method, that calls Shoot()
// PlayerInput -> InputActions -> Invoke Unity events
// RayGun.ShootTrigger is assigned to action(RightTrigger->ShootTrigger)
// (no need for code-based binding)
public class RayGun : MonoBehaviour
{
    [Header("Laser & Impact")] [SerializeField]
    private LineRenderer laserPrefab;

    [SerializeField] private GameObject rayImpactPrefab;
    [SerializeField] private float laserDuration = 0.2f;

    [Header("Shooting")] [SerializeField] private Transform shootingPoint;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float damage = 10f; // damage per shot
    [SerializeField] private LayerMask hitMask;

    [Header("Audio")] [SerializeField] private AudioSource shootAudio;
    [SerializeField] private AudioClip shootClip;

    public void ShootTrigger(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // play shooting sound
        if (shootAudio && shootClip) shootAudio.PlayOneShot(shootClip);

        // cast ray forward from gun
        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, hitMask);

        // if hit something, laser ends at the hit point, or goes max distance forward
        Vector3 targetPoint = hit ? hitInfo.point : shootingPoint.position + shootingPoint.forward * maxDistance;

        // If hit smth, react
        if (hit)
        {
            HandleHit(hitInfo);
        }

        if (laserPrefab)
        {
            DrawLaser(shootingPoint.position, targetPoint);
        }
    }

    private void HandleHit(RaycastHit hitInfo)
    {
        // if ray hits gameobj with IDamageable, call TakeDamage()
        IDamageable damageable = hitInfo.transform.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("RayGun: Hit a damageable object: " + hitInfo.transform.name);
            damageable.TakeDamage(damage, hitInfo.point, hitInfo.normal, gameObject);
        }
        // hits not damageable(walls)
        else if (rayImpactPrefab)
        {
            Debug.Log("RayGun: Hit a non-damageable object: " + hitInfo.transform.name);
            Quaternion rotation = Quaternion.LookRotation(-hitInfo.normal);
            GameObject impact = Instantiate(rayImpactPrefab, hitInfo.point, rotation);
            Destroy(impact, 1f);
        }
    }

    private void DrawLaser(Vector3 start, Vector3 end)
    {   
        LineRenderer lr = Instantiate(laserPrefab);
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        Destroy(lr.gameObject, laserDuration);
    }
}