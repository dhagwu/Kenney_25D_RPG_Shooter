using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyChaser : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform visualRoot;

    [Header("Chase Settings")]
    [SerializeField] private float detectRange = 12f;
    [SerializeField] private float attackRange = 1.35f;
    [SerializeField] private float repathInterval = 0.15f;

    [Header("Attack Settings")]
    [SerializeField] private float damagePerHit = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 720f;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private float nextAttackTime;
    private float nextRepathTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        agent.updateRotation = false;
        agent.stoppingDistance = attackRange;
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
            agent.ResetPath();
            RotateVisualToward(target.position - transform.position);
            return;
        }

        if (distanceToTarget > attackRange)
        {
            if (Time.time >= nextRepathTime)
            {
                agent.SetDestination(target.position);
                nextRepathTime = Time.time + repathInterval;
            }
        }
        else
        {
            agent.ResetPath();

            if (Time.time >= nextAttackTime && playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerHit);
                nextAttackTime = Time.time + attackCooldown;
            }
        }

        RotateVisual();
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

        Vector3 faceDirection = agent.velocity.sqrMagnitude > 0.05f
            ? agent.velocity
            : (target != null ? target.position - transform.position : transform.forward);

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