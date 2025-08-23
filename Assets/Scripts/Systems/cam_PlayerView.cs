using UnityEngine;

public class cam_PlayerView : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] Transform playerBody;
    [SerializeField] float mouseSensitivity = 1.5f;
    [SerializeField] float maxPitch = 85f;
    [SerializeField] float lookSmoothness = 12f;

    [Header("Head Bob (Idle)")]
    [SerializeField] float idlePitchAmp = 0.6f;
    [SerializeField] float idleRollAmp = 0.25f;
    [SerializeField] float idleFreq = 0.8f;

    [Header("Head Bob (Walk)")]
    [SerializeField] float walkPitchAmp = 1.35f;
    [SerializeField] float walkRollAmp = 0.6f;
    [SerializeField] float walkFreq = 3.2f;

    [Header("Modifiers")]
    [SerializeField] float sprintAmpMultiplier = 1.25f;
    [SerializeField] float sprintFreqMultiplier = 1.15f;
    [SerializeField] float crouchAmpMultiplier = 0.65f;
    [SerializeField] float crouchFreqMultiplier = 0.8f;
    [SerializeField] float bobBlendSpeed = 8f;

    [Header("Sway")]
    [SerializeField] float swayAmount = 0.02f;
    [SerializeField] float maxSway = 0.06f;
    [SerializeField] float swayResetSpeed = 3f;
    [SerializeField] float movementSwayMultiplier = 1.2f;

    [Header("Landing Impact")]
    [SerializeField] float landingImpactStrength = 0.2f;
    [SerializeField] float landingRecoverySpeed = 8f;

    g_PlayerMovement playerMovement;
    CharacterController playerController;

    float targetPitch;
    float pitch;
    Vector3 originalPosition;
    Vector3 swayPosition;
    Vector3 targetSwayPosition;
    Vector3 impactOffset;
    bool wasGrounded = true;

    float bobTime;
    float curPitchAmp;
    float curRollAmp;
    float curFreq;

    void Start()
    {
        originalPosition = transform.localPosition;
        playerMovement = GetComponentInParent<g_PlayerMovement>();
        playerController = GetComponentInParent<CharacterController>();
        curPitchAmp = idlePitchAmp;
        curRollAmp = idleRollAmp;
        curFreq = idleFreq;
    }

    void Update()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;
        playerBody.Rotate(Vector3.up * mx);
        targetPitch = Mathf.Clamp(targetPitch - my, -maxPitch, maxPitch);
        pitch = Mathf.Lerp(pitch, targetPitch, lookSmoothness * Time.deltaTime);

        float rx = Input.GetAxis("Mouse X");
        float ry = Input.GetAxis("Mouse Y");
        float moveMul = playerMovement != null && playerMovement.isMoving ? movementSwayMultiplier : 1f;
        targetSwayPosition.x = -rx * swayAmount * moveMul;
        targetSwayPosition.y = -ry * swayAmount * moveMul;
        targetSwayPosition.z = rx * swayAmount * 0.5f * moveMul;
        targetSwayPosition.x = Mathf.Clamp(targetSwayPosition.x, -maxSway, maxSway);
        targetSwayPosition.y = Mathf.Clamp(targetSwayPosition.y, -maxSway, maxSway);
        targetSwayPosition.z = Mathf.Clamp(targetSwayPosition.z, -maxSway, maxSway);
        swayPosition = Vector3.Lerp(swayPosition, targetSwayPosition, swayResetSpeed * Time.deltaTime);

        if (playerController != null)
        {
            if (!wasGrounded && playerController.isGrounded) impactOffset = Vector3.down * landingImpactStrength;
            wasGrounded = playerController.isGrounded;
        }

        bool moving = playerMovement != null && playerMovement.isMoving;
        bool sprint = playerMovement != null && playerMovement.isSprinting;
        bool crouch = Input.GetKey(KeyCode.C);

        float tgtPitchAmp = moving ? walkPitchAmp : idlePitchAmp;
        float tgtRollAmp = moving ? walkRollAmp : idleRollAmp;
        float tgtFreq = moving ? walkFreq : idleFreq;

        if (sprint) { tgtPitchAmp *= sprintAmpMultiplier; tgtRollAmp *= sprintAmpMultiplier; tgtFreq *= sprintFreqMultiplier; }
        if (crouch) { tgtPitchAmp *= crouchAmpMultiplier; tgtRollAmp *= crouchAmpMultiplier; tgtFreq *= crouchFreqMultiplier; }

        curPitchAmp = Mathf.Lerp(curPitchAmp, tgtPitchAmp, bobBlendSpeed * Time.deltaTime);
        curRollAmp = Mathf.Lerp(curRollAmp, tgtRollAmp, bobBlendSpeed * Time.deltaTime);
        curFreq = Mathf.Lerp(curFreq, tgtFreq, bobBlendSpeed * Time.deltaTime);

        bobTime += Time.deltaTime * Mathf.Max(0.01f, curFreq);

        float w = 2f * Mathf.PI;
        float s1 = Mathf.Sin(bobTime * w);
        float s2 = Mathf.Sin(bobTime * w * 2f + 0.35f);
        float c1 = Mathf.Cos(bobTime * w + 1.2f);

        float pitchBob = curPitchAmp * (0.7f * s1 + 0.3f * s2);
        float rollBob = curRollAmp * (0.6f * c1 + 0.4f * s1);

        impactOffset = Vector3.Lerp(impactOffset, Vector3.zero, landingRecoverySpeed * Time.deltaTime);

        Vector3 finalPosition = originalPosition + swayPosition + impactOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * 8f);
        transform.localRotation = Quaternion.Euler(pitch + pitchBob, 0f, swayPosition.z * 2f + rollBob);
    }
}
