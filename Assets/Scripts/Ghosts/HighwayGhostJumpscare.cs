using UnityEngine;
using DG.Tweening;

public class HighwayGhostJumpscare : MonoBehaviour
{
    [Header("Jumpscare Settings")]
    public Transform ghostMesh;
    public float moveDistance = 10f;
    public float moveDuration = 1.5f;
    public Ease moveEase = Ease.OutQuad;

    [Header("Trigger Settings")]
    public string carTag = "Car";
    public bool triggerOnce = true;

    private bool hasTriggered = false;
    private Vector3 startPosition;

    void Start()
    {
        if (ghostMesh != null)
        {
            startPosition = ghostMesh.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag(carTag))
        {
            TriggerJumpscare();
            hasTriggered = true;
        }
    }

    void TriggerJumpscare()
    {
        if (ghostMesh == null) return;

        Vector3 targetPosition = startPosition + new Vector3(0f, 0f, moveDistance);
        ghostMesh.DOMove(targetPosition, moveDuration).SetEase(moveEase);
    }

    public void ResetJumpscare()
    {
        if (ghostMesh != null)
        {
            ghostMesh.position = startPosition;
            hasTriggered = false;
        }
    }
}
