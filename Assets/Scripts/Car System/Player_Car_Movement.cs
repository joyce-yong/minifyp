using UnityEngine;
using DG.Tweening;

public class Player_Car_Movement : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxHorizontalLook = 45f;
    public float maxUpLook = 30f;
    public float maxDownLook = 0f;
    public float cameraShakeIntensity = 0.00005f;
    public float cameraShakeSpeed = 15f;

    [Header("Steering Settings")]
    public Transform steeringWheel;
    public float maxSteeringAngle = 35.571f;
    public float steeringSpeed = 2f;
    public float steeringReturnSpeed = 3f;

    [Header("Car Movement")]
    public Transform carBase;
    public float moveSpeed = 10f;
    public float reverseSpeed = 5f;
    public float turnSpeed = 50f;
    public float acceleration = 2f;
    public float deceleration = 3f;

    [Header("Headlight Settings")]
    public Light headlight;
    public float flickerDuration = 0.15f;

    [Header("Control Lock")]
    public bool startLocked = true;

    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    private float currentSteeringAngle = 0f;
    private float targetSteeringAngle = 0f;
    private float currentSpeed = 0f;
    private Vector3 originalCameraPos;
    private float shakeTimer = 0f;
    private bool isMoving = false;
    private bool isHeadlightOn = false;
    private float originalHeadlightIntensity = 0f;
    private bool isControlsLocked = false;

    void Start()
    {
        isControlsLocked = startLocked;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
        {
            originalCameraPos = playerCamera.localPosition;
        }

        if (headlight != null)
        {
            originalHeadlightIntensity = headlight.intensity;
            headlight.enabled = false;
            isHeadlightOn = false;
        }
    }

    public void UnlockControls()
    {
        isControlsLocked = false;
    }

    public void LockControls()
    {
        isControlsLocked = true;
    }

    void Update()
    {
        HandleCarMovement();
        HandleCameraLook();
        HandleSteering();
        HandleCameraShake();
        HandleHeadlightToggle();
    }

    void HandleCameraLook()
    {
        if (playerCamera == null || isControlsLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraRotationY += mouseX;
        cameraRotationX -= mouseY;

        cameraRotationY = Mathf.Clamp(cameraRotationY, -maxHorizontalLook, maxHorizontalLook);
        cameraRotationX = Mathf.Clamp(cameraRotationX, -maxDownLook, maxUpLook);

        playerCamera.localRotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0f);
    }

    void HandleSteering()
    {
        if (steeringWheel == null || isControlsLocked) return;

        if (Input.GetKey(KeyCode.A))
        {
            targetSteeringAngle = -maxSteeringAngle;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetSteeringAngle = maxSteeringAngle;
        }
        else
        {
            targetSteeringAngle = 0f;
        }

        float speed = (targetSteeringAngle == 0f) ? steeringReturnSpeed : steeringSpeed;
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, Time.deltaTime * speed);

        steeringWheel.localRotation = Quaternion.Euler(currentSteeringAngle, 0f, 0f);
    }

    void HandleCarMovement()
    {
        if (carBase == null || isControlsLocked) return;

        float verticalInput = 0f;
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = -1f;
        }

        float targetSpeed = 0f;
        if (verticalInput > 0)
        {
            targetSpeed = -moveSpeed;
        }
        else if (verticalInput < 0)
        {
            targetSpeed = reverseSpeed;
        }

        float accelRate = (targetSpeed != 0f) ? acceleration : deceleration;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelRate);

        isMoving = Mathf.Abs(currentSpeed) > 0.1f;

        carBase.Translate(new Vector3(currentSpeed * Time.deltaTime, 0f, 0f));

        if (isMoving && horizontalInput != 0f)
        {
            float turnAmount = horizontalInput * turnSpeed * Time.deltaTime * (currentSpeed / moveSpeed);
            carBase.Rotate(Vector3.up, turnAmount);
        }
    }

    void HandleCameraShake()
    {
        if (playerCamera == null) return;

        if (isMoving)
        {
            shakeTimer += Time.deltaTime * cameraShakeSpeed;
            float shakeX = Mathf.Sin(shakeTimer) * cameraShakeIntensity;
            float shakeY = Mathf.Cos(shakeTimer * 1.3f) * cameraShakeIntensity;

            playerCamera.localPosition = originalCameraPos + new Vector3(shakeX, shakeY, 0f);
        }
        else
        {
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, originalCameraPos, Time.deltaTime * 5f);
            shakeTimer = 0f;
        }
    }

    void HandleHeadlightToggle()
    {
        if (headlight == null) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            isHeadlightOn = !isHeadlightOn;

            if (isHeadlightOn)
            {
                FlickerHeadlightOn();
            }
            else
            {
                headlight.enabled = false;
            }
        }
    }

    void FlickerHeadlightOn()
    {
        if (headlight == null) return;

        Sequence flickerSequence = DOTween.Sequence();

        flickerSequence.Append(DOTween.To(() => headlight.intensity, x => headlight.intensity = x, originalHeadlightIntensity, flickerDuration)
            .OnStart(() => headlight.enabled = true));
        flickerSequence.Append(DOTween.To(() => headlight.intensity, x => headlight.intensity = x, 0f, flickerDuration * 0.5f));
        flickerSequence.Append(DOTween.To(() => headlight.intensity, x => headlight.intensity = x, originalHeadlightIntensity, flickerDuration * 0.5f));
        flickerSequence.Append(DOTween.To(() => headlight.intensity, x => headlight.intensity = x, 0f, flickerDuration * 0.3f));
        flickerSequence.Append(DOTween.To(() => headlight.intensity, x => headlight.intensity = x, originalHeadlightIntensity, flickerDuration * 0.3f));

        flickerSequence.Play();
    }
}
