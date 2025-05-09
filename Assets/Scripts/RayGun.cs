using UnityEngine;
using UnityEngine.InputSystem;

// Input Action Event system triggers this method, that calls Shoot()
// PlayerInput -> InputActions -> Invoke Unity events
// RayGun.ShootTrigger is assigned to action(RightTrigger->ShootTrigger)
// (no need for code-based binding)
public class RayGun : MonoBehaviour
{
    public LineRenderer laserPrefab;
    public GameObject rayImpactPrefab;
    public Transform shootingPoint;
    public float maxDistance = 5f;
    public float laserDuration = 0.2f;
    public AudioSource shootAudio;
    public AudioClip shootClip;
    public LayerMask hitMask;

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
            // if ray hits gameobj with IDamageable, call TakeDamage()
            IDamageable damageable = hitInfo.transform.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log("RayGun: Hit a damageable object: " + hitInfo.transform.name);
                damageable.TakeDamage(10f, hitInfo.point, hitInfo.normal, gameObject);
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

        if (laserPrefab)
        {
            LineRenderer lr = Instantiate(laserPrefab);
            lr.positionCount = 2;
            lr.SetPosition(0, shootingPoint.position);
            lr.SetPosition(1, targetPoint);
            Destroy(lr.gameObject, laserDuration);
        }

    }
}