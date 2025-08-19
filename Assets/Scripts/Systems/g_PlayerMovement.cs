using UnityEngine;

public class g_PlayerMovement : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    float yVelocity;
    float pitch;
    Vector2 moveInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(0f, mouseX, 0f);
        pitch = Mathf.Clamp(pitch - mouseY, -maxPitch, maxPitch);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (controller.isGrounded && yVelocity < 0f) yVelocity = -2f;
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void FixedUpdate()
    {
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        controller.Move(move * speed * Time.fixedDeltaTime);

        yVelocity += gravity * Time.fixedDeltaTime;
        controller.Move(Vector3.up * yVelocity * Time.fixedDeltaTime);
    }
}
