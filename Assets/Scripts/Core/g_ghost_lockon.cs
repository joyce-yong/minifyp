using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GhostLockOn : MonoBehaviour
{
    public Canvas worldCanvas;
    public RawImage reticleImage;
    public float LockOnTime = 2f;
    public float Progress;
    public bool ReadyToCapture;
    
    [Header("Target Point")]
    public Transform targetPoint;
    public bool showGizmo = true;
    
    float rotationSpeed = 0f;
    float pulseTimer = 0f;
    Vector3 initialScale;
    bool isVisible = false;

    void Start()
    {
        worldCanvas.enabled = false;
        worldCanvas.renderMode = RenderMode.WorldSpace;
        initialScale = reticleImage.transform.localScale;
        reticleImage.transform.localScale = Vector3.zero;
        
        if (targetPoint == null)
        {
            GameObject pivot = new GameObject("TargetPoint");
            pivot.transform.SetParent(transform);
            pivot.transform.localPosition = Vector3.zero;
            targetPoint = pivot.transform;
        }
    }

    void Update()
    {
        if (isVisible)
        {
            Vector3 directionToCamera = Camera.main.transform.position - worldCanvas.transform.position;
            worldCanvas.transform.rotation = Quaternion.LookRotation(directionToCamera);
            
            if (!ReadyToCapture && Progress > 0f)
            {
                float t = Progress / LockOnTime;
                reticleImage.transform.localScale = Vector3.Lerp(initialScale, initialScale * 0.3f, t);
                reticleImage.color = Color.Lerp(Color.white, Color.red, t);
                
                rotationSpeed = Mathf.Lerp(0f, 360f, t);
                reticleImage.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }
            else if (ReadyToCapture)
            {
                pulseTimer += Time.deltaTime * 8f;
                float pulse = 1f + Mathf.Sin(pulseTimer) * 0.15f;
                reticleImage.transform.localScale = initialScale * 0.3f * pulse;
                reticleImage.color = Color.red;
            }
        }
    }

    public Vector3 GetTargetPosition()
    {
        return targetPoint != null ? targetPoint.position : transform.position;
    }

    public void ShowUI()
    {
        if (!isVisible)
        {
            worldCanvas.enabled = true;
            isVisible = true;
            reticleImage.transform.DOScale(initialScale, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void HideUI()
    {
        if (isVisible)
        {
            isVisible = false;
            reticleImage.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                worldCanvas.enabled = false;
                Progress = 0f;
                ReadyToCapture = false;
                reticleImage.transform.rotation = Quaternion.identity;
                reticleImage.color = Color.white;
                rotationSpeed = 0f;
                pulseTimer = 0f;
            });
        }
    }

    public void Capture()
    {
        HideUI();
        gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (showGizmo && targetPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPoint.position, 0.2f);
            Gizmos.DrawLine(transform.position, targetPoint.position);
        }
    }
}