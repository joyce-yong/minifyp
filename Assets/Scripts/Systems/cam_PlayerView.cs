using UnityEngine;

public class cam_PlayerView : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] Transform playerBody;
    [SerializeField] float sensitivity = 100f;
    [SerializeField] float smoothing = 0.05f;
    [SerializeField] float maxPitch = 85f;

    [Header("Head Bob")]
    [SerializeField] float walkBobSpeed = 14f;
    [SerializeField] float walkBobAmount = 0.05f;
    [SerializeField] float idleBobSpeed = 2f;
    [SerializeField] float idleBobAmount = 0.01f;

    [Header("Zoom Settings")]
    [SerializeField] Camera playerCamera;
    [SerializeField] float defaultFOV = 60f;
    [SerializeField] float minZoomFOV = 15f;
    [SerializeField] float maxZoomFOV = 60f;
    [SerializeField] float zoomSpeed = 25f;
    [SerializeField] float zoomSmoothness = 8f;

    [Header("Camera Shake")]
    [SerializeField] float shakeIntensity = 0.15f;
    [SerializeField] float shakeDuration = 0.3f;
    [SerializeField] AnimationCurve shakeDecay = AnimationCurve.EaseInOut(0, 1, 1, 0);

    g_PlayerMovement movement;
    CharacterController controller;

    float xRot;
    float smoothX;
    float smoothY;
    float smoothVelX;
    float smoothVelY;

    float defaultYPos;
    float timer;

    float targetFOV;
    float currentFOV;

    bool isShaking;
    float shakeTimer;
    float shakeDurationTotal;
    Vector3 shakeOffset;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        movement = GetComponentInParent<g_PlayerMovement>();
        controller = GetComponentInParent<CharacterController>();
        defaultYPos = transform.localPosition.y;

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            defaultFOV = playerCamera.fieldOfView;
            currentFOV = targetFOV = defaultFOV;
        }
    }

    void LateUpdate()
    {
        HandleLook();
        HandleCameraShake();
        HandleHeadBob();
        HandleZoom();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * 0.01f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * 0.01f;

        smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothVelX, smoothing);
        smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothVelY, smoothing);

        xRot -= smoothY;
        xRot = Mathf.Clamp(xRot, -maxPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerBody.Rotate(Vector3.up * smoothX);
    }

    void HandleHeadBob()
    {
        if (controller == null || isShaking) return;

        if (!controller.isGrounded)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, defaultYPos, 0), Time.deltaTime * 8f);
            return;
        }

        if (movement != null && movement.isMoving)
        {
            timer += Time.deltaTime * walkBobSpeed;
            float bobOffset = Mathf.Sin(timer) * walkBobAmount;
            transform.localPosition = new Vector3(0, defaultYPos + bobOffset, 0);
        }
        else
        {
            timer += Time.deltaTime * idleBobSpeed;
            float bobOffset = Mathf.Sin(timer) * idleBobAmount;
            transform.localPosition = new Vector3(0, defaultYPos + bobOffset, 0);
        }
    }

    void HandleZoom()
    {
        if (playerCamera == null) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            targetFOV -= scrollInput * zoomSpeed * Time.deltaTime * 60f;
            targetFOV = Mathf.Clamp(targetFOV, minZoomFOV, maxZoomFOV);
        }

        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSmoothness);
        playerCamera.fieldOfView = currentFOV;
    }

    void HandleCameraShake()
    {
        if (isShaking)
        {
            shakeTimer += Time.deltaTime;
            float progress = shakeTimer / shakeDurationTotal;
            
            if (progress >= 1f)
            {
                isShaking = false;
                shakeOffset = Vector3.zero;
                transform.localPosition = new Vector3(0, defaultYPos, 0);
            }
            else
            {
                float strength = shakeDecay.Evaluate(progress) * shakeIntensity;
                shakeOffset = Vector3.Lerp(shakeOffset, Random.insideUnitSphere * strength, Time.deltaTime * 15f);
                transform.localPosition = new Vector3(shakeOffset.x, defaultYPos + shakeOffset.y, shakeOffset.z);
            }
        }
    }

    public void TriggerShake()
    {
        isShaking = true;
        shakeTimer = 0f;
        shakeDurationTotal = shakeDuration;
        shakeOffset = Vector3.zero;
    }

    public void SetZoom(float zoomFOV)
    {
        targetFOV = Mathf.Clamp(zoomFOV, minZoomFOV, maxZoomFOV);
    }

    public void ResetZoom()
    {
        targetFOV = defaultFOV;
    }
}