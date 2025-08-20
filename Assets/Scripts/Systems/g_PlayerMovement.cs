using UnityEngine;

public class g_PlayerMovement : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchMoveMultiplier = 0.5f;
    [SerializeField] private float crouchSpeed = 5f;

    float yVelocity;
    Vector2 moveInput;
    float originalHeight;
    float targetHeight;
    bool isCrouching;
    public bool isMoving { get; private set; }
    public bool isSprinting { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalHeight = controller.height;
        targetHeight = originalHeight;
    }

    void Update()
    {
        HandleMovementInput();
        HandleJumping();
        HandleCrouching();
    }

    void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        isMoving = moveInput.magnitude > 0.1f && controller.isGrounded;
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching && isMoving;
    }

    void HandleJumping()
    {
        if (controller.isGrounded && yVelocity < 0f) yVelocity = -2f;
        if (!isCrouching && Input.GetButtonDown("Jump") && controller.isGrounded)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void HandleCrouching()
    {
        isCrouching = Input.GetKey(KeyCode.C);
        targetHeight = isCrouching ? crouchHeight : originalHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSpeed);
    }

    void FixedUpdate()
    {
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        float speed = moveSpeed;

        if (isCrouching) speed *= crouchMoveMultiplier;
        else if (isSprinting) speed *= sprintMultiplier;

        controller.Move(move * speed * Time.fixedDeltaTime);

        yVelocity += gravity * Time.fixedDeltaTime;
        controller.Move(Vector3.up * yVelocity * Time.fixedDeltaTime);
    }
}