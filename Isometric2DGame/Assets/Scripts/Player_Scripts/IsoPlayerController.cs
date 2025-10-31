using UnityEngine;
using UnityEngine.InputSystem;

public class IsoPlayerController : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script requires Rigidbody2D,PlayerAnimationController and HealthSystem components on the same GameObject.";

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public float attackRange = 1.2f;
    [Range(5f, 180f)] public float attackAngle = 180f; 
    public float attackCooldown = 0.6f;
    public int attackDamage = 25;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerControls controls;
    private PlayerAnimationController anim;
    private HealthSystem health;
    private float nextAttackTime;
    private bool isDead;

    private readonly Vector2 isoRight = new(1f, -0.5f);
    private readonly Vector2 isoUp = new(1f, 0.5f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        anim = GetComponent<PlayerAnimationController>();
        health = GetComponent<HealthSystem>();
    }

    void OnEnable()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Attack.performed += OnAttack;
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Attack.performed -= OnAttack;
        controls.Player.Disable();
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        if (isDead) return;
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnAttack(InputAction.CallbackContext ctx)
    {
        if (isDead || Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        anim.TriggerAttack();

        Vector2 attackDir = anim.GetLastMoveDir();
        if (attackDir == Vector2.zero) attackDir = Vector2.up;

        Vector2 isoAttackDir = (attackDir.x * isoRight + attackDir.y * isoUp).normalized;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (var collider in hitColliders)
        {
            if (collider.isTrigger)
                continue;

            HealthSystem enemyHealth = collider.GetComponent<HealthSystem>();
            if (enemyHealth == null)
                continue;

            Vector2 toEnemy = (Vector2)collider.transform.position - (Vector2)transform.position;
            float distance = toEnemy.magnitude;
            if (distance > attackRange)
                continue;

            float angle = Vector2.Angle(isoAttackDir, toEnemy);
            if (angle <= attackAngle / 2f)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }



    void FixedUpdate()
    {
        if (health != null && health.IsDead)
        {
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 isoDirection = (moveInput.x * isoRight + moveInput.y * isoUp);
        if (isoDirection.sqrMagnitude > 1f)
            isoDirection.Normalize();

        rb.MovePosition(rb.position + isoDirection * moveSpeed * Time.fixedDeltaTime);
        anim.UpdateAnimation(moveInput);
    }

    void OnDrawGizmosSelected()
    {
        if (anim == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector2 dir = anim.GetLastMoveDir();
        if (dir == Vector2.zero) dir = Vector2.up;

        Vector2 isoDir = (dir.x * isoRight + dir.y * isoUp).normalized;

        float halfAngle = attackAngle / 2f * Mathf.Deg2Rad;

        Vector2 left = new(
            isoDir.x * Mathf.Cos(-halfAngle) - isoDir.y * Mathf.Sin(-halfAngle),
            isoDir.x * Mathf.Sin(-halfAngle) + isoDir.y * Mathf.Cos(-halfAngle)
        );
        Vector2 right = new(
            isoDir.x * Mathf.Cos(halfAngle) - isoDir.y * Mathf.Sin(halfAngle),
            isoDir.x * Mathf.Sin(halfAngle) + isoDir.y * Mathf.Cos(halfAngle)
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + isoDir * attackRange);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + left * attackRange);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + right * attackRange);
    }
}
