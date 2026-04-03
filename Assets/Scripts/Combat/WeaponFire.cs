using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class WeaponFire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioClip fireClip;

    [Header("Fire Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 4f;
    [SerializeField] private float fireDistance = 40f;

    private PlayerInputHandler inputHandler;
    private float nextFireTime;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (!inputHandler.FireHeld)
            return;

        if (Time.time < nextFireTime)
            return;

        Fire();
    }

    private void Fire()
    {
        nextFireTime = Time.time + 1f / fireRate;

        if (firePoint == null || mainCamera == null)
            return;

        if (AudioManager.Instance != null && fireClip != null)
        {
            AudioManager.Instance.PlaySFX(fireClip, 1f);
        }

        Vector3 aimPoint = GetAimPointOnGroundPlane();
        Vector3 shotDirection = (aimPoint - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, shotDirection, out RaycastHit hit, fireDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Debug.DrawLine(firePoint.position, hit.point, Color.red, 0.15f);
        }
        else
        {
            Debug.DrawRay(firePoint.position, shotDirection * fireDistance, Color.white, 0.15f);
        }
    }

    private Vector3 GetAimPointOnGroundPlane()
    {
        Ray ray = mainCamera.ScreenPointToRay(inputHandler.LookScreenPosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return firePoint.position + transform.forward * fireDistance;
    }
}