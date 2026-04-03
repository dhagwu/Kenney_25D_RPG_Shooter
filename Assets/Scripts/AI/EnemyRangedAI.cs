using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyRangedAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform firePoint;

    [Header("Ranges")]
    [SerializeField] private float detectRange = 18f;
    [SerializeField] private float preferredRange = 8f;
    [SerializeField] private float retreatRange = 4f;
    [SerializeField] private float fireDistance = 20f;

    [Header("Attack")]
    [SerializeField] private float damagePerShot = 8f;
    [SerializeField] private float fireInterval = 1.1f;

    [Header("Movement")]
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float rotateSpeed = 720f;

    [Header("Audio")]
    [SerializeField] private AudioClip fireClip;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private float nextFireTime;
    private float nextRepathTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        agent.updateRotation = false;
        agent.stoppingDistance = preferredRange;
    }

    private void Start()
    {
        TryFindTarget();
    }

    private void Update()
    {
        if (target == null)
        {
            TryFindTarget();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > detectRange)
        {
            agent.isStopped = true;
            RotateVisualToward(target.position - transform.position);
            return;
        }

        if (distanceToTarget > preferredRange)
        {
            agent.isStopped = false;
            agent.stoppingDistance = preferredRange;

            if (Time.time >= nextRepathTime)
            {
                agent.SetDestination(target.position);
                nextRepathTime = Time.time + repathInterval;
            }
        }
        else if (distanceToTarget < retreatRange)
        {
            agent.isStopped = false;
            agent.stoppingDistance = 0.1f;   // 关键：后退时不要继续用 8

            Vector3 retreatDirection = (transform.position - target.position).normalized;
            Vector3 retreatTarget = transform.position + retreatDirection * 4f;

            if (Time.time >= nextRepathTime)
            {
                agent.SetDestination(retreatTarget);
                nextRepathTime = Time.time + repathInterval;
            }
        }
        else
        {
            agent.isStopped = true;

            if (Time.time >= nextFireTime)
            {
                FireAtPlayer();
                nextFireTime = Time.time + fireInterval;
            }
        }

        RotateVisual();
    }

    private void FireAtPlayer()
    {
        if (firePoint == null || target == null)
            return;

        Vector3 targetPoint = target.position + Vector3.up * 1.0f;
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        if (AudioManager.Instance != null && fireClip != null)
        {
            AudioManager.Instance.PlaySFX(fireClip, 1f);
        }

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, fireDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            PlayerHealth hitPlayer = hit.collider.GetComponentInParent<PlayerHealth>();

            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(damagePerShot);
            }

            Debug.DrawLine(firePoint.position, hit.point, Color.yellow, 0.2f);
        }
        else
        {
            Debug.DrawRay(firePoint.position, direction * fireDistance, Color.cyan, 0.2f);
        }
    }

    private void TryFindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        target = player.transform;
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void RotateVisual()
    {
        if (visualRoot == null)
            return;

        Vector3 faceDirection = target != null
            ? target.position - transform.position
            : transform.forward;

        RotateVisualToward(faceDirection);
    }

    private void RotateVisualToward(Vector3 direction)
    {
        if (visualRoot == null)
            return;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        visualRoot.rotation = Quaternion.RotateTowards(
            visualRoot.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }
}