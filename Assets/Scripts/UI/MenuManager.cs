using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public RectTransform mainMenuPanel;
    public RectTransform optionsPanel;

    [Header("Scene Transition")]
    public Image fadeImage;
    public string nextSceneName;
    public float fadeDuration = 1f;

    [Header("Animation Settings")]
    public float slideDuration = 0.5f;
    public Ease slideEase = Ease.OutCubic;

    private Vector2 mainMenuShowPos = Vector2.zero;
    private Vector2 mainMenuHidePos = new Vector2(-800f, 0f);
    private Vector2 optionsShowPos = Vector2.zero;
    private Vector2 optionsHidePos = new Vector2(2000f, 0f);

    void Start()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.anchoredPosition = mainMenuHidePos;
            mainMenuPanel.DOAnchorPos(mainMenuShowPos, slideDuration).SetEase(slideEase);
        }

        if (optionsPanel != null)
        {
            optionsPanel.anchoredPosition = optionsHidePos;
        }

        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
            fadeImage.gameObject.SetActive(true);
        }
    }

    public void OpenOptions()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.DOAnchorPos(mainMenuHidePos, slideDuration).SetEase(slideEase);
        }

        if (optionsPanel != null)
        {
            optionsPanel.DOAnchorPos(optionsShowPos, slideDuration).SetEase(slideEase);
        }
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.DOAnchorPos(optionsHidePos, slideDuration).SetEase(slideEase);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.DOAnchorPos(mainMenuShowPos, slideDuration).SetEase(slideEase);
        }
    }

    public void StartGame()
    {
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
