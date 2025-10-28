using UnityEngine;
using UnityEngine.InputSystem;

public class IsoCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerControls controls;

    private readonly Vector2 isoRight = new(1f, -0.5f);  
    private readonly Vector2 isoUp = new(1f, 0.5f);      

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
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
        Vector2 isoDirection = (moveInput.x * isoRight + moveInput.y * isoUp);

        if (isoDirection.sqrMagnitude > 1f)
            isoDirection.Normalize();

        rb.MovePosition(rb.position + isoDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
