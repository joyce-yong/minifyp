using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 2f;
    [SerializeField] private bool playOnce = true;

    private bool hasPlayed = false;

    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnce && hasPlayed) return;

            hasPlayed = true;
            StartCoroutine(PlayDialogue());
        }
    }

    IEnumerator PlayDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);

            foreach (string line in dialogueLines)
            {
                yield return StartCoroutine(TypewriterEffect(line));
                yield return new WaitForSeconds(delayBetweenLines);
                dialogueText.text = "";
            }

            dialogueText.gameObject.SetActive(false);
        }
    }

    IEnumerator TypewriterEffect(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }
}
