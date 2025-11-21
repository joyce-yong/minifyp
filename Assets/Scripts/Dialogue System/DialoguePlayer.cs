using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    public int dialogueId;
    public bool playOnce = true;

    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnce && hasPlayed) return;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.PlayDialogue(dialogueId);
                hasPlayed = true;
            }
        }
    }
}
