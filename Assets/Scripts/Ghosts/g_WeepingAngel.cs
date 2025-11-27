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
            Vector3 dirToPlayer = player.position - angelMesh.position;
            dirToPlayer.y = 0f;

            if (dirToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer, Vector3.up);

                float currentX = angelMesh.eulerAngles.x;
                float currentZ = angelMesh.eulerAngles.z;

                angelMesh.rotation = Quaternion.Slerp(angelMesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                Vector3 euler = angelMesh.eulerAngles;
                euler.x = currentX;
                euler.z = currentZ;
                angelMesh.eulerAngles = euler;
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
