using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ui_InteractionDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private Color interactColor = Color.yellow;
    
    private g_interaction_system interactionSystem;
    private Color originalIconColor;
    private Color originalTextColor;
    private bool isVisible = false;
    private Sequence currentSequence;
    
    void Start()
    {
        interactionSystem = FindObjectOfType<g_interaction_system>();
        originalIconColor = actionIcon.color;
        originalTextColor = actionLabel.color;
        canvasGroup.alpha = 0f;
    }
    
    void Update()
    {
        bool hasInteractable = interactionSystem.HasInteractable();
        
        if (hasInteractable && !isVisible)
        {
            ShowUI();
        }
        else if (!hasInteractable && isVisible)
        {
            HideUI();
        }
        
        if (hasInteractable)
        {
            actionLabel.text = interactionSystem.GetCurrentInteractionText();
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayInteractFeedback();
            }
        }
    }
    
    void ShowUI()
    {
        isVisible = true;
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Append(canvasGroup.DOFade(1f, fadeInDuration));
    }
    
    void HideUI()
    {
        isVisible = false;
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Append(canvasGroup.DOFade(0f, fadeOutDuration));
    }
    
    void PlayInteractFeedback()
    {
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        
        currentSequence.Append(actionIcon.DOColor(interactColor, 0.1f));
        currentSequence.Join(actionLabel.DOColor(interactColor, 0.1f));
        currentSequence.Append(actionIcon.DOColor(originalIconColor, 0.15f));
        currentSequence.Join(actionLabel.DOColor(originalTextColor, 0.15f));
    }
    
    void OnDestroy()
    {
        currentSequence?.Kill();
    }
}