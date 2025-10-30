using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script requires Rigidbody2D, EnemyAnimationController, and HealthSystem components on the same GameObject.";

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public int attackDamage = 5;

    [Header("Patrol Settings")]
    public float idleWaitTime = 2f;
    public float patrolRadius = 4f;

    [Header("Detection Settings")]
    public float detectionRange = 6f;
    public bool useTriggerDetection = true;

    private HealthSystem health;
    private Rigidbody2D rb;
    private EnemyAnimationController anim;
    private Transform player;
    private Vector2 spawnPosition;
    private Vector2 targetPoint;
    private float idleTimer;
    private float nextAttackTime;
    private bool playerInRange;
    private State currentState;

    private readonly Vector2 isoRight = new(1f, -0.5f);
    private readonly Vector2 isoUp = new(1f, 0.5f);

    private enum State { Idle, Patrol, Chase, Attack }

    void Awake()
    {
        health = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimationController>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("No Player found");

        spawnPosition = transform.position;
        PickNewPatrolPoint();
        currentState = State.Idle;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (useTriggerDetection && col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = detectionRange;
        }
    }

    void Update()
    {
        if (health == null || rb == null) return;
        if (health.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = player != null
            ? Vector2.Distance(transform.position, player.position)
            : Mathf.Infinity;

        bool detected = useTriggerDetection ? playerInRange : distanceToPlayer < detectionRange;

        switch (currentState)
        {
            case State.Idle:
                Idle();
                if (detected) ChangeState(State.Chase);
                break;

            case State.Patrol:
                Patrol();
                if (detected) ChangeState(State.Chase);
                break;

            case State.Chase:
                Chase();
                if (distanceToPlayer <= attackRange)
                    ChangeState(State.Attack);
                else if (!detected)
                    ChangeState(State.Patrol);
                break;

            case State.Attack:
                Attack();
                if (distanceToPlayer > attackRange + 0.5f)
                    ChangeState(State.Chase);
                break;
        }
    }
    void Idle()
    {
        anim.UpdateAnimation(Vector2.zero);
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleWaitTime)
        {
            idleTimer = 0;
            PickNewPatrolPoint();
            ChangeState(State.Patrol);
        }
    }

    void Patrol()
    {
        MoveTowards(targetPoint, moveSpeed);
        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
            ChangeState(State.Idle);
    }

    void Chase()
    {
        if (player == null) return;
        MoveTowards(player.position, chaseSpeed);
    }

    void Attack()
    {
        if (player == null) return;

        rb.linearVelocity = Vector2.zero;
        anim.UpdateAnimation(Vector2.zero);

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        anim.TriggerAttack();

        Vector2 toPlayer = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange + 0.3f)
            player.GetComponent<HealthSystem>()?.TakeDamage(attackDamage);
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        Vector2 isoDirection = (direction.x * isoRight + direction.y * isoUp);

        if (isoDirection.sqrMagnitude > 1f)
            isoDirection.Normalize();

        rb.MovePosition(rb.position + isoDirection * speed * Time.deltaTime);
        anim.UpdateAnimation(direction);
    }

    void PickNewPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        targetPoint = spawnPosition + randomOffset;
    }

    void ChangeState(State newState)
    {
        currentState = newState;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useTriggerDetection && other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (useTriggerDetection && other.CompareTag("Player"))
            playerInRange = false;
    }
}
