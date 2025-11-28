using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialPrompt
{
    [TextArea(2, 5)]
    public string promptText;
    public float displayDuration = 3f;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
}

public class TutorialPromptManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI promptText;
    public CanvasGroup promptCanvasGroup;

    [Header("Tutorial Prompts")]
    public List<TutorialPrompt> tutorialPrompts = new List<TutorialPrompt>();

    [Header("Auto Start")]
    public bool autoStartOnEnable = true;
    public float delayBeforeStart = 0.5f;

    private int currentPromptIndex = 0;
    private bool isPlaying = false;

    void OnEnable()
    {
        if (autoStartOnEnable)
        {
            StartCoroutine(DelayedStart());
        }
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        StartTutorialPrompts();
    }

    public void StartTutorialPrompts()
    {
        if (tutorialPrompts.Count == 0)
        {
            Debug.LogWarning("No tutorial prompts added to the list!");
            return;
        }

        if (!isPlaying)
        {
            StartCoroutine(PlayTutorialSequence());
        }
    }

    IEnumerator PlayTutorialSequence()
    {
        isPlaying = true;

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
        }

        for (currentPromptIndex = 0; currentPromptIndex < tutorialPrompts.Count; currentPromptIndex++)
        {
            TutorialPrompt prompt = tutorialPrompts[currentPromptIndex];

            if (promptText != null)
            {
                promptText.text = prompt.promptText;
            }

            if (promptCanvasGroup != null)
            {
                promptCanvasGroup.DOFade(1f, prompt.fadeInDuration);
            }

            yield return new WaitForSeconds(prompt.fadeInDuration);

            yield return new WaitForSeconds(prompt.displayDuration);

            if (promptCanvasGroup != null)
            {
                promptCanvasGroup.DOFade(0f, prompt.fadeOutDuration);
            }

            yield return new WaitForSeconds(prompt.fadeOutDuration);
        }

        isPlaying = false;
        OnTutorialComplete();
    }

    void OnTutorialComplete()
    {
        if (promptText != null)
        {
            promptText.text = "";
        }
    }

    public void StopTutorial()
    {
        StopAllCoroutines();
        isPlaying = false;

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.DOFade(0f, 0.3f);
        }

        if (promptText != null)
        {
            promptText.text = "";
        }
    }

    public void RestartTutorial()
    {
        StopTutorial();
        currentPromptIndex = 0;
        StartTutorialPrompts();
    }
}
