using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script requires an Animator component on the same GameObject.";

    private Animator animator;
    private Vector2 lastMoveDir;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateAnimation(Vector2 moveInput)
    {
        float speed = moveInput.magnitude;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
            lastMoveDir = moveInput.normalized;

         animator.SetFloat("LastMoveX", lastMoveDir.x);
         animator.SetFloat("LastMoveY", lastMoveDir.y);
    }
    public Vector2 GetLastMoveDir()
    {
        return lastMoveDir;
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TriggerTakeDamage()
    {
        animator.SetTrigger("TakeDamage");
    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Dead");
    }
}
