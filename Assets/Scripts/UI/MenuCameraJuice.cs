using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuCameraJuice : MonoBehaviour
{
    [Header("Background Settings")]
    public RectTransform backgroundPanel;

    [Header("Movement Settings")]
    public float moveAmount = 30f;
    public float smoothSpeed = 2f;

    private Vector2 originalPosition;
    private Vector2 targetPosition;

    void Start()
    {
        if (backgroundPanel != null)
        {
            originalPosition = backgroundPanel.anchoredPosition;
            targetPosition = originalPosition;
        }
    }

    void Update()
    {
        if (backgroundPanel == null) return;

        Vector3 mousePos = Input.mousePosition;
        float screenCenterX = Screen.width * 0.5f;
        float screenCenterY = Screen.height * 0.5f;

        float offsetX = (mousePos.x - screenCenterX) / screenCenterX;
        float offsetY = (mousePos.y - screenCenterY) / screenCenterY;

        offsetX = Mathf.Clamp(offsetX, -1f, 1f);
        offsetY = Mathf.Clamp(offsetY, -1f, 1f);

        targetPosition = originalPosition + new Vector2(offsetX * moveAmount, offsetY * moveAmount);

        backgroundPanel.anchoredPosition = Vector2.Lerp(backgroundPanel.anchoredPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
