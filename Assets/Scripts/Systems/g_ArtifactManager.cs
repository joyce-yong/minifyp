using UnityEngine;
using TMPro;

public class g_ArtifactManager : MonoBehaviour
{
    [SerializeField] private GameObject noteHudCanvas;
    [SerializeField] private TextMeshProUGUI artifactTextLabel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    
    private bool isNoteVisible = false;
    
    void Start()
    {
        if (noteHudCanvas != null)
            noteHudCanvas.SetActive(false);
    }
    
    void Update()
    {
        if (isNoteVisible && Input.GetKeyDown(KeyCode.Escape))
        {
            HideNote();
        }
    }
    
    public void ShowNote(string text)
    {
        if (noteHudCanvas != null)
        {
            noteHudCanvas.SetActive(true);
            isNoteVisible = true;
            
            if (artifactTextLabel != null)
                artifactTextLabel.text = text;
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeIn());
            }
        }
    }
    
    public void HideNote()
    {
        if (canvasGroup != null)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            if (noteHudCanvas != null)
                noteHudCanvas.SetActive(false);
            isNoteVisible = false;
        }
    }
    
    private System.Collections.IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    
    private System.Collections.IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        
        if (noteHudCanvas != null)
            noteHudCanvas.SetActive(false);
        isNoteVisible = false;
    }
}