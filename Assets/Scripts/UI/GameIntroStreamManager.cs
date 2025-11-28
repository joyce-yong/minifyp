using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StreamDialogueLine
{
    [TextArea(3, 10)]
    public string text;
    public AudioClip voiceLine;
    public SwitchCondition switchCondition = SwitchCondition.AfterAudio;
    public float customWaitTime = 2f;
}

public class GameIntroStreamManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public CanvasGroup streamStartingCanvas;
    public CanvasGroup tutorialCanvas;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Dialogue Lines")]
    public List<StreamDialogueLine> dialogueLines = new List<StreamDialogueLine>();

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.05f;

    [Header("Canvas Fade Settings")]
    public float canvasFadeDuration = 1.5f;
    public float tutorialCanvasFadeDuration = 1f;
    public float dialogueFadeOutDuration = 0.5f;

    [Header("Player Control")]
    public Player_Car_Movement playerCarMovement;

    [Header("Chat Control")]
    public ChatManager chatManager;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool waitingForInput = false;
    private Coroutine currentDialogueCoroutine = null;

    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (streamStartingCanvas != null)
        {
            streamStartingCanvas.alpha = 1f;
        }

        if (tutorialCanvas != null)
        {
            tutorialCanvas.alpha = 0f;
            tutorialCanvas.gameObject.SetActive(false);
        }

        if (dialogueLines.Count > 0)
        {
            StartCoroutine(PlayIntroStream());
        }
    }

    IEnumerator PlayIntroStream()
    {
        for (currentLineIndex = 0; currentLineIndex < dialogueLines.Count; currentLineIndex++)
        {
            StreamDialogueLine line = dialogueLines[currentLineIndex];

            if (line.voiceLine != null && audioSource != null)
            {
                audioSource.clip = line.voiceLine;
                audioSource.Play();
            }

            yield return StartCoroutine(TypeWriterEffect(line.text));

            switch (line.switchCondition)
            {
                case SwitchCondition.AfterAudio:
                    if (line.voiceLine != null && audioSource != null)
                    {
                        yield return new WaitWhile(() => audioSource.isPlaying);
                    }
                    else
                    {
                        yield return new WaitForSeconds(2f);
                    }
                    break;

                case SwitchCondition.AfterTime:
                    yield return new WaitForSeconds(line.customWaitTime);
                    break;

                case SwitchCondition.WaitForInput:
                    waitingForInput = true;
                    yield return new WaitUntil(() => !waitingForInput);
                    break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (dialogueText != null)
        {
            dialogueText.DOFade(0f, dialogueFadeOutDuration);
        }

        yield return new WaitForSeconds(dialogueFadeOutDuration);

        if (chatManager != null)
        {
            chatManager.StopIntroSequence();
            chatManager.StartChat("highway");
        }

        yield return new WaitForSeconds(0.3f);

        if (streamStartingCanvas != null)
        {
            streamStartingCanvas.DOFade(0f, canvasFadeDuration).OnComplete(() =>
            {
                if (tutorialCanvas != null)
                {
                    tutorialCanvas.gameObject.SetActive(true);
                    tutorialCanvas.DOFade(1f, tutorialCanvasFadeDuration);
                }

                if (playerCarMovement != null)
                {
                    playerCarMovement.UnlockControls();
                }
            });
        }
        else
        {
            if (tutorialCanvas != null)
            {
                tutorialCanvas.gameObject.SetActive(true);
                tutorialCanvas.DOFade(1f, tutorialCanvasFadeDuration);
            }

            if (playerCarMovement != null)
            {
                playerCarMovement.UnlockControls();
            }
        }
    }

    IEnumerator TypeWriterEffect(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(TypeWriterEffect(dialogueLines[currentLineIndex].text));
                dialogueText.text = dialogueLines[currentLineIndex].text;
                isTyping = false;
            }
            else if (waitingForInput)
            {
                waitingForInput = false;
            }
        }
    }

    public void PlaySingleDialogueLine(StreamDialogueLine line)
    {
        if (line == null)
        {
            Debug.LogWarning("Cannot play null dialogue line!");
            return;
        }

        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
        }

        currentDialogueCoroutine = StartCoroutine(PlaySingleLineCoroutine(line));
    }

    public void PlayMultipleDialogueLines(List<StreamDialogueLine> lines)
    {
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning("Cannot play null or empty dialogue lines!");
            return;
        }

        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
        }

        currentDialogueCoroutine = StartCoroutine(PlayMultipleLinesCoroutine(lines));
    }

    IEnumerator PlaySingleLineCoroutine(StreamDialogueLine line)
    {
        if (dialogueText != null)
        {
            dialogueText.DOFade(1f, 0.3f);
        }

        if (line.voiceLine != null && audioSource != null)
        {
            audioSource.clip = line.voiceLine;
            audioSource.Play();
        }

        yield return StartCoroutine(TypeWriterEffect(line.text));

        switch (line.switchCondition)
        {
            case SwitchCondition.AfterAudio:
                if (line.voiceLine != null && audioSource != null)
                {
                    yield return new WaitWhile(() => audioSource.isPlaying);
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                }
                break;

            case SwitchCondition.AfterTime:
                yield return new WaitForSeconds(line.customWaitTime);
                break;

            case SwitchCondition.WaitForInput:
                waitingForInput = true;
                yield return new WaitUntil(() => !waitingForInput);
                break;
        }

        if (dialogueText != null)
        {
            dialogueText.DOFade(0f, dialogueFadeOutDuration);
        }

        yield return new WaitForSeconds(dialogueFadeOutDuration);

        currentDialogueCoroutine = null;
    }

    IEnumerator PlayMultipleLinesCoroutine(List<StreamDialogueLine> lines)
    {
        if (dialogueText != null)
        {
            dialogueText.DOFade(1f, 0.3f);
        }

        for (int i = 0; i < lines.Count; i++)
        {
            StreamDialogueLine line = lines[i];

            if (line.voiceLine != null && audioSource != null)
            {
                audioSource.clip = line.voiceLine;
                audioSource.Play();
            }

            yield return StartCoroutine(TypeWriterEffect(line.text));

            switch (line.switchCondition)
            {
                case SwitchCondition.AfterAudio:
                    if (line.voiceLine != null && audioSource != null)
                    {
                        yield return new WaitWhile(() => audioSource.isPlaying);
                    }
                    else
                    {
                        yield return new WaitForSeconds(2f);
                    }
                    break;

                case SwitchCondition.AfterTime:
                    yield return new WaitForSeconds(line.customWaitTime);
                    break;

                case SwitchCondition.WaitForInput:
                    waitingForInput = true;
                    yield return new WaitUntil(() => !waitingForInput);
                    break;
            }
        }

        if (dialogueText != null)
        {
            dialogueText.DOFade(0f, dialogueFadeOutDuration);
        }

        yield return new WaitForSeconds(dialogueFadeOutDuration);

        currentDialogueCoroutine = null;
    }
}
