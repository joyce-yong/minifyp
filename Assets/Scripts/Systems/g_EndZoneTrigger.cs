using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class g_EndZoneTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CaptureManager captureManager;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Dialogue Settings")]
    [SerializeField] private string[] incompleteDialogueLines;
    [SerializeField] private float dialogueDisplayTime = 3f;
    [SerializeField] private float typewriterSpeed = 0.05f;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float fadeDuration = 2f;

    private bool isShowingDialogue = false;
    private bool isTransitioning = false;

    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(false);
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
        }

        if (captureManager == null)
        {
            captureManager = FindObjectOfType<CaptureManager>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            if (captureManager != null && captureManager.HasCompletedAllCaptures())
            {
                StartCoroutine(TransitionToNextScene());
            }
            else
            {
                if (!isShowingDialogue && incompleteDialogueLines.Length > 0)
                {
                    StartCoroutine(ShowRandomDialogue());
                }
            }
        }
    }

    IEnumerator ShowRandomDialogue()
    {
        isShowingDialogue = true;
        string randomLine = incompleteDialogueLines[Random.Range(0, incompleteDialogueLines.Length)];

        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
            yield return StartCoroutine(TypewriterEffect(randomLine));
            yield return new WaitForSeconds(dialogueDisplayTime);
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(false);
        }

        isShowingDialogue = false;
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

    IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;

        if (fadeCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
