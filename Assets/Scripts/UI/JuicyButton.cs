using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class JuicyButton : MonoBehaviour
{
    [Header("Button References")]
    public List<Button> buttons = new List<Button>();

    [Header("Animation Settings")]
    public float scaleAmount = 1.15f;
    public float slideAmount = 10f;
    public float animationDuration = 0.2f;
    public Ease easeType = Ease.OutBack;

    [Header("Color Settings")]
    public bool useColorTint = true;
    public Color hoverColor = Color.white;

    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<RectTransform, Vector3> originalPositions = new Dictionary<RectTransform, Vector3>();
    private Dictionary<Image, Color> originalColors = new Dictionary<Image, Color>();
        
    void Start()
    {
        foreach (Button button in buttons)
        {
            if (button == null) continue;

            GameObject buttonObj = button.gameObject;
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Image buttonImage = button.GetComponent<Image>();

            originalScales[buttonObj] = buttonObj.transform.localScale;
            originalPositions[rectTransform] = rectTransform.anchoredPosition;

            if (buttonImage != null && useColorTint)
            {
                originalColors[buttonImage] = buttonImage.color;
            }

            Image[] childImages = button.GetComponentsInChildren<Image>();
            foreach (Image img in childImages)
            {
                if (img != buttonImage && useColorTint)
                {
                    originalColors[img] = img.color;
                }
            }

            EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = buttonObj.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnButtonHover(buttonObj); });
            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnButtonExit(buttonObj); });
            trigger.triggers.Add(pointerExit);
        }
    }

    void OnButtonHover(GameObject buttonObj)
    {
        if (!originalScales.ContainsKey(buttonObj)) return;

        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();

        buttonObj.transform.DOScale(originalScales[buttonObj] * scaleAmount, animationDuration).SetEase(easeType);
        rectTransform.DOAnchorPos(originalPositions[rectTransform] + new Vector3(-slideAmount, 0f, 0f), animationDuration).SetEase(easeType);

        if (useColorTint)
        {
            Image[] allImages = buttonObj.GetComponentsInChildren<Image>();
            foreach (Image img in allImages)
            {
                if (originalColors.ContainsKey(img))
                {
                    img.DOColor(hoverColor, animationDuration).SetEase(easeType);
                }
            }
        }
    }

    void OnButtonExit(GameObject buttonObj)
    {
        if (!originalScales.ContainsKey(buttonObj)) return;

        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();

        buttonObj.transform.DOScale(originalScales[buttonObj], animationDuration).SetEase(Ease.OutQuad);
        rectTransform.DOAnchorPos(originalPositions[rectTransform], animationDuration).SetEase(Ease.OutQuad);

        if (useColorTint)
        {
            Image[] allImages = buttonObj.GetComponentsInChildren<Image>();
            foreach (Image img in allImages)
            {
                if (originalColors.ContainsKey(img))
                {
                    img.DOColor(originalColors[img], animationDuration).SetEase(Ease.OutQuad);
                }
            }
        }
    }
}
