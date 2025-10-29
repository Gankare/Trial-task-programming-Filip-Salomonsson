using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script requires an Rigidbody2D component on the same GameObject.";

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float detectionRange = 6f;
    public float attackRange = 1.2f;
    public float idleWaitTime = 2f;
    public float patrolRadius = 4f;

    private Rigidbody2D rb;
    private EnemyAnimationController anim;
    private Transform player;
    private Vector2 spawnPosition;
    private Vector2 targetPoint;
    private float idleTimer;
    private State currentState;

    private readonly Vector2 isoRight = new(1f, -0.5f);
    private readonly Vector2 isoUp = new(1f, 0.5f);

    private enum State { Idle, Patrol, Chase, Attack }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimationController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPosition = transform.position;
        PickNewPatrolPoint();
        currentState = State.Idle;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                Idle();
                if (distanceToPlayer < detectionRange)
                    ChangeState(State.Chase);
                break;

            case State.Patrol:
                Patrol();
                if (distanceToPlayer < detectionRange)
                    ChangeState(State.Chase);
                break;

            case State.Chase:
                Chase();
                if (distanceToPlayer < attackRange)
                    ChangeState(State.Attack);
                else if (distanceToPlayer > detectionRange * 1.5f)
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
        {
            ChangeState(State.Idle);
        }
    }

    void Chase()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;
        anim.TriggerAttack();
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
}
