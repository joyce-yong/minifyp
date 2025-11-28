using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectivePrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private string[] promptLines;
    [SerializeField] private float fadeWaitTime = 1f;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 1f;

    void Start()
    {
        if (promptText != null)
        {
            promptText.text = "";
            StartCoroutine(ShowPrompts());
        }
    }

    IEnumerator ShowPrompts()
    {
        yield return new WaitForSeconds(fadeWaitTime);

        foreach (string line in promptLines)
        {
            yield return StartCoroutine(TypewriterEffect(line));
            yield return new WaitForSeconds(delayBetweenLines);
            promptText.text = "";
        }

        Destroy(gameObject);
    }

    IEnumerator TypewriterEffect(string text)
    {
        promptText.text = "";
        foreach (char c in text)
        {
            promptText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }
}
