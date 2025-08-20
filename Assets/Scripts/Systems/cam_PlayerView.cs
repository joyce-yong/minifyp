using UnityEngine;

public class cam_PlayerView : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 1.5f;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private float lookSmoothness = 12f;

    [Header("Head Bob")]
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private float bobVerticalAmount = 0.05f;
    [SerializeField] private float bobHorizontalAmount = 0.03f;
    [SerializeField] private float sprintBobMultiplier = 1.8f;
    [SerializeField] private float crouchBobMultiplier = 0.6f;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 0.02f;
    [SerializeField] private float maxSway = 0.06f;
    [SerializeField] private float swayResetSpeed = 3f;
    [SerializeField] private float movementSwayMultiplier = 1.2f;

    [Header("Landing Impact")]
    [SerializeField] private float landingImpactStrength = 0.2f;
    [SerializeField] private float landingRecoverySpeed = 8f;

    float pitch;
    float targetPitch;
    Vector3 originalPosition;
    float bobTimer;
    Vector3 swayPosition;
    Vector3 targetSwayPosition;
    Vector3 impactOffset;
    bool wasGrounded = true;
    
    g_PlayerMovement playerMovement;
    CharacterController playerController;

    void Start()
    {
        originalPosition = transform.localPosition;
        playerMovement = GetComponentInParent<g_PlayerMovement>();
        playerController = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        HandleMouseLook();
        CalculateSway();
        CalculateHeadBob();
        HandleLandingImpact();
        ApplyCameraEffects();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        playerBody.Rotate(Vector3.up * mouseX);
        targetPitch -= mouseY;
        targetPitch = Mathf.Clamp(targetPitch, -maxPitch, maxPitch);
        
        pitch = Mathf.Lerp(pitch, targetPitch, lookSmoothness * Time.deltaTime);
    }

    void CalculateSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        float movementMultiplier = playerMovement != null && playerMovement.isMoving ? movementSwayMultiplier : 1f;
        
        targetSwayPosition.x = -mouseX * swayAmount * movementMultiplier;
        targetSwayPosition.y = -mouseY * swayAmount * movementMultiplier;
        targetSwayPosition.z = mouseX * swayAmount * 0.5f * movementMultiplier;

        targetSwayPosition.x = Mathf.Clamp(targetSwayPosition.x, -maxSway, maxSway);
        targetSwayPosition.y = Mathf.Clamp(targetSwayPosition.y, -maxSway, maxSway);
        targetSwayPosition.z = Mathf.Clamp(targetSwayPosition.z, -maxSway, maxSway);

        swayPosition = Vector3.Lerp(swayPosition, targetSwayPosition, swayResetSpeed * Time.deltaTime);
    }

    void CalculateHeadBob()
    {
        if (playerMovement == null || !playerMovement.isMoving)
        {
            bobTimer = 0f;
            return;
        }

        float bobMultiplier = 1f;
        if (playerMovement.isSprinting) bobMultiplier = sprintBobMultiplier;
        else if (Input.GetKey(KeyCode.C)) bobMultiplier = crouchBobMultiplier;

        bobTimer += Time.deltaTime * bobFrequency * bobMultiplier;
    }

    void HandleLandingImpact()
    {
        if (playerController != null)
        {
            if (!wasGrounded && playerController.isGrounded)
            {
                impactOffset = Vector3.down * landingImpactStrength;
            }
            wasGrounded = playerController.isGrounded;
        }
    }

    void ApplyCameraEffects()
    {
        Vector3 bobOffset = Vector3.zero;
        
        if (playerMovement != null && playerMovement.isMoving)
        {
            bobOffset.y = Mathf.Sin(bobTimer) * bobVerticalAmount;
            bobOffset.x = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmount;
        }

        impactOffset = Vector3.Lerp(impactOffset, Vector3.zero, landingRecoverySpeed * Time.deltaTime);

        Vector3 finalPosition = originalPosition + bobOffset + swayPosition + impactOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * 6f);
        transform.localRotation = Quaternion.Euler(pitch, 0f, swayPosition.z * 2f);
    }
}