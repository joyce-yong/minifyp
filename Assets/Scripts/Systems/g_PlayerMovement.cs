using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class g_PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.5f;
    [SerializeField] float crouchMultiplier = 0.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.2f;

    [Header("Crouch")]
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float heightSmooth = 8f;

    CharacterController controller;
    Vector2 input;
    Vector3 velocity;
    float originalHeight;
    bool isCrouching;
    public bool isMoving { get; private set; }
    public bool isSprinting { get; private set; }

    bool isSafe = false;
    public bool IsSafe => isSafe;
    private bool isDead = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleGravity();
        HandleJump();
        HandleCrouch();
    }

    void HandleInput()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isMoving = input.sqrMagnitude > 0.1f;
        isSprinting = Input.GetKey(KeyCode.LeftShift) && isMoving && !isCrouching;
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move.Normalize();

        float speed = moveSpeed;
        if (isCrouching) speed *= crouchMultiplier;
        else if (isSprinting) speed *= sprintMultiplier;

        if (input.sqrMagnitude < 0.01f) move = Vector3.zero;

        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && controller.isGrounded && !isCrouching)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void HandleCrouch()
    {
        isCrouching = Input.GetKey(KeyCode.C);
        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * heightSmooth);
    }

    // Red Light Green Light RedLine Point
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "RedLine")
        {
            if (!isSafe)
            {
                isSafe = true;
                Debug.Log("Player passed!");

                if (FindAnyObjectByType<g_Girl>() is g_Girl gameManager)
                {
                    gameManager.PlayerWon();
                }
            }
        }
    }

    // Player die
    public void KillPlayer()
    {
        if (isDead || isSafe) return;
        SetDeadState(true);
        if (g_ScreenFader.Instance != null)
        {
            StartCoroutine(g_ScreenFader.Instance.FadeOutIn(RespawnPlayer));
        }
        else
        {
            RespawnPlayer();
        }
    }
    public bool PlayerIsDead()
    {
        return isDead;
    }
    public void SetDeadState(bool state)
    {
        isDead = state;
    }

    public void RespawnPlayer()
    {
        isDead = true;
        Debug.Log("Player Killed. Respawning...");

        controller.enabled = false;
        velocity = Vector3.zero;

        Vector3 spawnPoint = g_RespawnCheckpoint.GetRespawnPoint();
        transform.position = spawnPoint + Vector3.up * 0.5f;

        velocity = Vector3.zero;

        controller.enabled = true;
        isDead = false;

        if (FindAnyObjectByType<g_Girl>() is g_Girl gameManager)
        {
            gameManager.StopGame();
        }
    }

}
