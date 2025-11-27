using UnityEngine;
using DG.Tweening;

public class FloatAnimation : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatHeight = 30f;
    public float floatDuration = 2f;
    public Ease floatEase = Ease.InOutSine;

    [Header("Rotation Settings")]
    public bool enableRotation = false;
    public Vector3 rotationAmount = new Vector3(0f, 360f, 0f);
    public float rotationDuration = 4f;
    public Ease rotationEase = Ease.Linear;

    private Vector2 startPosition;
    private RectTransform rectTransform;
    private Transform regularTransform;
    private bool isUIElement;
    private Tween floatTween;
    private Tween rotateTween;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            isUIElement = true;
            startPosition = rectTransform.anchoredPosition;
            Debug.Log($"FloatAnimation: UI Element detected. Start Position: {startPosition}");
        }
        else
        {
            isUIElement = false;
            regularTransform = transform;
            startPosition = regularTransform.localPosition;
            Debug.Log($"FloatAnimation: 3D Object detected. Start Position: {startPosition}");
        }
    }

    void Start()
    {
        Debug.Log($"FloatAnimation: Starting float animation. Height: {floatHeight}, Duration: {floatDuration}");
        StartFloating();
    }

    public void StartFloating()
    {
        if (floatTween != null)
        {
            floatTween.Kill();
        }

        if (isUIElement)
        {
            floatTween = rectTransform.DOAnchorPosY(startPosition.y + floatHeight, floatDuration)
                .SetEase(floatEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }
        else
        {
            floatTween = regularTransform.DOLocalMoveY(startPosition.y + floatHeight, floatDuration)
                .SetEase(floatEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        if (enableRotation)
        {
            if (rotateTween != null)
            {
                rotateTween.Kill();
            }

            Transform targetTransform = isUIElement ? rectTransform : regularTransform;
            rotateTween = targetTransform.DOLocalRotate(rotationAmount, rotationDuration, RotateMode.LocalAxisAdd)
                .SetEase(rotationEase)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);
        }
    }

    public void StopFloating()
    {
        if (floatTween != null)
        {
            floatTween.Kill();
        }

        if (rotateTween != null)
        {
            rotateTween.Kill();
        }

        if (isUIElement)
        {
            rectTransform.DOAnchorPos(startPosition, 0.5f).SetEase(Ease.OutQuad);
        }
        else
        {
            regularTransform.DOLocalMove(startPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    void OnDestroy()
    {
        if (floatTween != null)
        {
            floatTween.Kill();
        }

        if (rotateTween != null)
        {
            rotateTween.Kill();
        }
    }
}
