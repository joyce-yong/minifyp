using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CutsceneLine
{
    [TextArea(3, 10)]
    public string text;
    public AudioClip voiceLine;
    public SwitchCondition switchCondition = SwitchCondition.AfterAudio;
    public float customWaitTime = 2f;
}

public enum SwitchCondition
{
    AfterAudio,
    AfterTime,
    WaitForInput
}

public class IntroCutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image fadeImage;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Dialogue Lines")]
    public List<CutsceneLine> dialogueLines = new List<CutsceneLine>();

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.05f;

    [Header("Scene Transition")]
    public string nextSceneName;
    public float fadeDuration = 1f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool waitingForInput = false;

    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
            fadeImage.gameObject.SetActive(true);
        }

        if (dialogueLines.Count > 0)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        for (currentLineIndex = 0; currentLineIndex < dialogueLines.Count; currentLineIndex++)
        {
            CutsceneLine line = dialogueLines[currentLineIndex];

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

        if (fadeImage != null)
        {
            fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                if (!string.IsNullOrEmpty(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
            });
        }
        else
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
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
}
