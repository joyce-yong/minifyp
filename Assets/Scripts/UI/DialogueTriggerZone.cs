using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueTriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string carTag = "Car";
    public bool triggerOnce = true;

    [Header("Dialogue Settings")]
    public List<StreamDialogueLine> dialogueLines = new List<StreamDialogueLine>();

    [Header("References")]
    public GameIntroStreamManager dialogueManager;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag(carTag))
        {
            TriggerDialogue();
            hasTriggered = true;
        }
    }

    void TriggerDialogue()
    {
        if (dialogueManager != null && dialogueLines.Count > 0)
        {
            dialogueManager.PlayMultipleDialogueLines(dialogueLines);
        }
        else
        {
            Debug.LogWarning("DialogueTriggerZone: Missing dialogue manager or dialogue lines!");
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = hasTriggered ? Color.gray : Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
