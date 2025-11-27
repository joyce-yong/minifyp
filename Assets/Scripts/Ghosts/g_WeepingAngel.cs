using UnityEngine;

public class g_WeepingAngel : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] float detectionRadius = 15f;
    [SerializeField] float viewAngle = 60f;

    [Header("Movement")]
    [SerializeField] float rotationSpeed = 2f;

    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform angelMesh;

    Transform player;
    bool hasBeenTriggered;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (angelMesh == null)
            angelMesh = transform;
    }

    void Update()
    {
        if (player == null || playerCamera == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (!hasBeenTriggered && distToPlayer <= detectionRadius)
        {
            Vector3 dirToAngel = (transform.position - playerCamera.transform.position).normalized;
            Vector3 playerForward = playerCamera.transform.forward;
            float angleToAngel = Vector3.Angle(playerForward, dirToAngel);

            if (angleToAngel <= viewAngle)
            {
                hasBeenTriggered = true;
            }
        }

        if (!hasBeenTriggered) return;

        bool playerLookingAtAngel = IsPlayerLookingAtAngel();

        if (!playerLookingAtAngel)
        {
            Vector3 dirToPlayer = (player.position - angelMesh.position).normalized;
            dirToPlayer.y = 0f;
            dirToPlayer.Normalize();

            if (dirToPlayer != Vector3.zero)
            {
                float targetYRotation = Mathf.Atan2(dirToPlayer.x, dirToPlayer.z) * Mathf.Rad2Deg;
                float currentYRotation = angelMesh.eulerAngles.y;
                float newYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, rotationSpeed * Time.deltaTime);

                angelMesh.rotation = Quaternion.Euler(angelMesh.eulerAngles.x, newYRotation, angelMesh.eulerAngles.z);
            }
        }
    }

    bool IsPlayerLookingAtAngel()
    {
        Vector3 dirToAngel = (transform.position - playerCamera.transform.position).normalized;
        float angleToAngel = Vector3.Angle(playerCamera.transform.forward, dirToAngel);

        return angleToAngel <= viewAngle;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = hasBeenTriggered ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
