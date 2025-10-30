using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script requires Rigidbody2D, EnemyAnimationController, HealthSystem and NavMeshAgent components on the same GameObject.";

    [Header("AI Settings")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public int attackDamage = 5;
    public float idleWaitTime = 2f;
    public float patrolRadius = 4f;
    public float detectionRange = 6f;

    private NavMeshAgent agent;
    private Transform player;
    private EnemyAnimationController anim;
    private HealthSystem health;
    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private float idleTimer;
    private float nextAttackTime;
    private bool playerInRange;
    private State state;

    private float stuckTimer = 0f;
    private Vector3 lastPosition;
    public float stuckThreshold = 0.05f;  
    public float stuckTimeLimit = 2f;     

    private enum State { Idle, Patrol, Chase, Attack }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<HealthSystem>();
        anim = GetComponent<EnemyAnimationController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spawnPosition = transform.position;
        PickPatrolPoint();
        state = State.Idle;

        lastPosition = transform.position;
    }

    void Update()
    {
        if (health == null || health.IsDead)
        {
            agent.ResetPath();
            return;
        }

        float distanceToPlayer = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (state)
        {
            case State.Idle:
                Idle();
                if (playerInRange) ChangeState(State.Chase);
                break;

            case State.Patrol:
                Patrol();
                if (playerInRange) ChangeState(State.Chase);
                break;

            case State.Chase:
                Chase();
                if (!playerInRange)
                    ChangeState(State.Patrol);
                else if (distanceToPlayer < attackRange)
                    ChangeState(State.Attack);
                break;

            case State.Attack:
                Attack();
                if (!playerInRange || distanceToPlayer > attackRange + 0.5f)
                    ChangeState(State.Chase);
                break;
        }

        anim.UpdateAnimation(agent.velocity.normalized);
    }

    void Idle()
    {
        agent.ResetPath();
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleWaitTime)
        {
            idleTimer = 0f;
            PickPatrolPoint();
            ChangeState(State.Patrol);
        }
    }

    void Patrol()
    {
        if (!agent.hasPath)
            agent.SetDestination(patrolTarget);

        if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckTimeLimit)
            {
                PickPatrolPoint();
                agent.SetDestination(patrolTarget);
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;

        if (Vector2.Distance(transform.position, patrolTarget) < 0.3f)
            ChangeState(State.Idle);
    }

    void Chase()
    {
        if (player == null) return;
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        agent.ResetPath();
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        anim.TriggerAttack();

        if (player && Vector2.Distance(transform.position, player.position) <= attackRange + 0.3f)
            player.GetComponent<HealthSystem>()?.TakeDamage(attackDamage);
    }

    void PickPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = spawnPosition + randomOffset;
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                return;
            }
        }

        patrolTarget = spawnPosition;
    }

    void ChangeState(State newState) => state = newState;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPosition, patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(patrolTarget, 0.1f);
    }
}
