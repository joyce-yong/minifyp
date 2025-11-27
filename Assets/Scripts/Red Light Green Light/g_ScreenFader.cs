using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class g_ScreenFader : MonoBehaviour
{
    public static g_ScreenFader Instance;

    [SerializeField] Image fadeImage;
    [SerializeField] Color fadeColor = Color.black;
    [SerializeField] float fadeDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); 
            fadeImage.raycastTarget = false; 
        }
        else
        {
            Debug.LogError("Fade Image not assigned to ScreenFader!");
        }
    }

    public IEnumerator FadeOutIn(System.Action midFadeAction)
    {
        yield return StartCoroutine(Fade(1f, fadeDuration));
        midFadeAction?.Invoke();
        yield return StartCoroutine(Fade(0f, fadeDuration));
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, targetAlpha);
    }
}