using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextAsset dialogueCSV;
    public TextMeshProUGUI dialogueLabel;
    public CanvasGroup dialogueCanvasGroup;
    public float typingSpeed = 0.05f;
    public float delayBetweenLines = 1f;

    private List<DialogueLine> allDialogueLines;
    private List<DialogueLine> currentSequence;
    private int currentLineIndex = 0;
    private bool isPlaying = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (dialogueCSV != null)
        {
            allDialogueLines = DialogueParser.ParseCSV(dialogueCSV.text);
        }

        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0f;
            dialogueCanvasGroup.interactable = false;
            dialogueCanvasGroup.blocksRaycasts = false;
        }
    }

    public void PlayDialogue(int id)
    {
        if (isPlaying) return;

        currentSequence = DialogueParser.GetDialogueSequence(allDialogueLines, id);

        if (currentSequence.Count > 0)
        {
            isPlaying = true;
            currentLineIndex = 0;
            ShowDialogueUI();
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        while (currentLineIndex < currentSequence.Count)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeText(currentSequence[currentLineIndex].dialogue));
            yield return typingCoroutine;

            yield return new WaitForSeconds(delayBetweenLines);

            currentLineIndex++;
        }

        HideDialogueUI();
        isPlaying = false;
    }

    IEnumerator TypeText(string text)
    {
        dialogueLabel.text = "";

        foreach (char c in text)
        {
            dialogueLabel.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void ShowDialogueUI()
    {
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 1f;
            dialogueCanvasGroup.interactable = true;
            dialogueCanvasGroup.blocksRaycasts = true;
        }
    }

    void HideDialogueUI()
    {
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0f;
            dialogueCanvasGroup.interactable = false;
            dialogueCanvasGroup.blocksRaycasts = false;
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
