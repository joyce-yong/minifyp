using UnityEngine;

public class g_RespawnCheckpoint : MonoBehaviour
{
    public static Vector3 CheckpointPosition { get; private set; }
    public static Quaternion CheckpointRotation { get; private set; }
    public static float RespawnOffsetDistance = 3f; 

    [Header("Respawn Settings")]
    [SerializeField] float respawnOffset = 3f; 
    [Tooltip("If checked, this checkpoint can only be set once.")]
    [SerializeField] bool oneTimeUse = true;

    private bool isActivated = false;

    private void Start()
    {
        RespawnOffsetDistance = respawnOffset;
        if (CheckpointPosition == Vector3.zero)
        {
            SetCheckpointData(transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (!oneTimeUse || !isActivated))
        {
            SetCheckpointData(transform.position, transform.rotation);
            isActivated = true;
            Debug.Log("Checkpoint Set!");
        }
    }

    private void SetCheckpointData(Vector3 pos, Quaternion rot)
    {
        CheckpointPosition = pos;
        CheckpointRotation = rot;
        RespawnOffsetDistance = respawnOffset;
    }

    public static Vector3 GetRespawnPoint()
    {
        Vector3 backwards = CheckpointRotation * Vector3.back;

        return CheckpointPosition + backwards * RespawnOffsetDistance;
    }
}