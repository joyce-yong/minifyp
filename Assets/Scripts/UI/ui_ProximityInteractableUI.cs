using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProximityInteractableUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage indicatorIcon;
    [SerializeField] private RawImage interactIcon;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private Camera playerCamera;
    private Transform targetTransform;
    private Canvas canvas;
    private g_ProximityPromptUtil promptUtil;
    
    private float fadeInDuration;
    private float fadeOutDuration;
    private float scaleInDuration;
    private float interactDistance = 2f;
    private float indicatorMaxScale = 0.5f;
    private float interactMaxScale = 0.6f;
    private Color normalInteractColor = Color.white;
    private Color lookingAtColor = new Color(1f, 0.86f, 0.38f, 1f);
    
    private bool isVisible = false;
    private bool isInteractState = false;
    private Sequence currentSequence;
    
    public void Initialize(Camera camera, Transform target, float fadeIn, float fadeOut, float scaleIn, float pulse, float pulseDur)
    {
        playerCamera = camera;
        targetTransform = target;
        fadeInDuration = fadeIn;
        fadeOutDuration = fadeOut;
        scaleInDuration = scaleIn;
        
        promptUtil = targetTransform.GetComponent<g_ProximityPromptUtil>();
        if (promptUtil != null)
        {
            interactDistance = promptUtil.GetInteractDistance();
            indicatorMaxScale = promptUtil.GetIndicatorScale();
            interactMaxScale = promptUtil.GetInteractScale();
            normalInteractColor = promptUtil.GetNormalColor();
            lookingAtColor = promptUtil.GetLookingColor();
        }
        
        SetupUI();
        Show();
    }
    
    void SetupUI()
    {
        canvas = GetComponent<Canvas>();
        
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = playerCamera;
        }
        
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        if (indicatorIcon == null || interactIcon == null)
        {
            RawImage[] images = GetComponentsInChildren<RawImage>();
            foreach (var img in images)
            {
                if (img.name.ToLower().Contains("indicator") && indicatorIcon == null)
                    indicatorIcon = img;
                else if (img.name.ToLower().Contains("interact") && interactIcon == null)
                    interactIcon = img;
            }
        }
        
        if (interactIcon != null)
        {
            interactIcon.gameObject.SetActive(false);
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        if (indicatorIcon != null)
        {
            indicatorIcon.transform.localScale = Vector3.zero;
        }
        
        if (promptUtil != null)
            promptUtil.ApplyAdjustments(this);
    }
    
    void Update()
    {
        if (playerCamera != null && targetTransform != null)
        {
            Vector3 basePosition = targetTransform.position;
            Vector3 targetPos = promptUtil != null ? promptUtil.GetAdjustedPosition(basePosition) : basePosition;
            transform.position = targetPos;
            
            Vector3 directionToCamera = (playerCamera.transform.position - transform.position).normalized;
            
            if (indicatorIcon != null)
            {
                Vector3 indicatorPos = promptUtil != null ? promptUtil.GetIndicatorPosition(targetPos) : targetPos;
                indicatorIcon.transform.position = indicatorPos;
                
                if (promptUtil != null)
                {
                    indicatorIcon.transform.rotation = promptUtil.GetIndicatorRotation(Quaternion.LookRotation(-directionToCamera));
                }
                else
                {
                    indicatorIcon.transform.rotation = Quaternion.LookRotation(-directionToCamera);
                }
            }
            
            if (interactIcon != null)
            {
                Vector3 interactPos = promptUtil != null ? promptUtil.GetInteractPosition(targetPos) : targetPos;
                interactIcon.transform.position = interactPos;
                
                if (promptUtil != null)
                {
                    interactIcon.transform.rotation = promptUtil.GetInteractRotation(Quaternion.LookRotation(-directionToCamera));
                }
                else
                {
                    interactIcon.transform.rotation = Quaternion.LookRotation(-directionToCamera);
                }
            }
            
            CheckDistanceForInteract();
        }
        else if (targetTransform == null)
        {
            Destroy(gameObject);
        }
    }
    
    public void Show()
    {
        if (isVisible) return;
        isVisible = true;
        
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        
        currentSequence.Append(canvasGroup.DOFade(1f, fadeInDuration));
        if (indicatorIcon != null)
        {
            indicatorIcon.transform.localScale = Vector3.zero;
            currentSequence.Join(indicatorIcon.transform.DOScale(Vector3.one * indicatorMaxScale, scaleInDuration).SetEase(Ease.OutBack));
        }
    }
    
    public void Hide(System.Action onComplete = null)
    {
        if (!isVisible) return;
        isVisible = false;
        
        currentSequence?.Kill();
        
        currentSequence = DOTween.Sequence();
        currentSequence.Append(canvasGroup.DOFade(0f, fadeOutDuration));
        if (indicatorIcon != null)
            currentSequence.Join(indicatorIcon.transform.DOScale(Vector3.zero, fadeOutDuration));
        if (interactIcon != null && interactIcon.gameObject.activeInHierarchy)
            currentSequence.Join(interactIcon.transform.DOScale(Vector3.zero, fadeOutDuration));
        currentSequence.OnComplete(() => onComplete?.Invoke());
    }
    
    public void UpdateInteractState(bool shouldShowInteract)
    {
        if (isInteractState == shouldShowInteract) return;
        isInteractState = shouldShowInteract;
        
        if (interactIcon != null)
        {
            if (shouldShowInteract)
            {
                interactIcon.gameObject.SetActive(true);
                interactIcon.transform.localScale = Vector3.zero;
                interactIcon.transform.DOScale(Vector3.one * interactMaxScale, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                interactIcon.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => {
                    if (interactIcon != null)
                        interactIcon.gameObject.SetActive(false);
                });
            }
        }
    }
    
    public void UpdateLookingState(bool isLookingAt)
    {
        if (interactIcon != null && interactIcon.gameObject.activeInHierarchy)
        {
            Color targetColor = isLookingAt ? lookingAtColor : normalInteractColor;
            interactIcon.DOColor(targetColor, 0.2f);
        }
    }
    
    public void HideWithInteraction()
    {
        isVisible = false;
        currentSequence?.Kill();
        
        currentSequence = DOTween.Sequence();
        if (indicatorIcon != null)
            currentSequence.Append(indicatorIcon.transform.DOScale(Vector3.zero, 0.05f));
        if (interactIcon != null && interactIcon.gameObject.activeInHierarchy)
            currentSequence.Join(interactIcon.transform.DOScale(Vector3.zero, 0.05f));
        currentSequence.Join(canvasGroup.DOFade(0f, 0.05f));
        currentSequence.OnComplete(() => {
            if (gameObject != null)
                Destroy(gameObject);
        });
    }
    
    public void CheckDistanceForInteract()
    {
        if (playerCamera != null && targetTransform != null)
        {
            float distance = Vector3.Distance(playerCamera.transform.position, targetTransform.position);
            bool shouldShowInteract = distance <= interactDistance;
            UpdateInteractState(shouldShowInteract);
        }
    }
    
    void StopAllAnimations()
    {
        currentSequence?.Kill();
    }
    
    void OnDestroy()
    {
        StopAllAnimations();
    }
}