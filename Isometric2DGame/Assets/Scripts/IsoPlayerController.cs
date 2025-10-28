using UnityEngine;
using UnityEngine.InputSystem;

public class IsoPlayerController : MonoBehaviour
{
    [Header("Script Info")]
    [TextArea]
    [Tooltip("This is just an informational note.")]
    public string info = "This script requires an Rigidbody2D component on the same GameObject.";

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerControls controls;
    private PlayerAnimationController anim;

    private readonly Vector2 isoRight = new(1f, -0.5f);
    private readonly Vector2 isoUp = new(1f, 0.5f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        anim = GetComponent<PlayerAnimationController>();
    }

    void OnEnable()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Disable();
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector2 direction = moveInput;
        Vector2 isoDirection = (moveInput.x * isoRight + moveInput.y * isoUp);
        if (isoDirection.sqrMagnitude > 1f)
            isoDirection.Normalize();

        rb.MovePosition(rb.position + isoDirection * moveSpeed * Time.fixedDeltaTime);
        //Note: Use "+ direction" instead of "+ isoDirection" for normal movement (Not isometric movement)

        anim.UpdateAnimation(moveInput);
    }
}
