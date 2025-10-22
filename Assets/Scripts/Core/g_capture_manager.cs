using UnityEngine;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    public float captureRange = 10f;
    public float alignThreshold = 50f;
    public Camera playerCamera;
    public LayerMask ghostLayer;
    public RectTransform playerReticle;
    public RawImage playerReticleImage;
    public Canvas captureOverlay;
    public cam_PlayerView cameraView;

    GhostLockOn currentTarget;
    bool captureMode;
    Color originalReticleColor;

    void Start()
    {
        originalReticleColor = playerReticleImage.color;
        captureOverlay.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            captureMode = !captureMode;
            captureOverlay.enabled = captureMode;
            if (!captureMode && currentTarget)
            {
                currentTarget.HideUI();
                currentTarget = null;
            }
            playerReticleImage.color = originalReticleColor;
        }

        if (!captureMode)
        {
            if (currentTarget)
            {
                currentTarget.HideUI();
                currentTarget = null;
            }
            return;
        }

        GhostLockOn newTarget = FindClosestGhost();
        
        if (newTarget != currentTarget)
        {
            if (currentTarget) currentTarget.HideUI();
            currentTarget = newTarget;
            if (currentTarget) currentTarget.ShowUI();
        }

        if (currentTarget && IsValidTarget(currentTarget))
        {
            Vector3 targetWorldPos = currentTarget.GetTargetPosition();
            Vector3 screenPos = playerCamera.WorldToScreenPoint(targetWorldPos);
            float distance = Vector2.Distance(screenPos, playerReticle.position);
            
            if (distance < alignThreshold)
            {
                currentTarget.Progress += Time.deltaTime;
                
                if (currentTarget.Progress >= currentTarget.LockOnTime)
                {
                    currentTarget.ReadyToCapture = true;
                    playerReticleImage.color = Color.red;
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (cameraView != null) cameraView.TriggerShake();
                        currentTarget.Capture();
                        currentTarget = null;
                        playerReticleImage.color = originalReticleColor;
                    }
                }
            }
            else
            {
                if (currentTarget.Progress > 0f)
                {
                    currentTarget.Progress = Mathf.Max(0f, currentTarget.Progress - Time.deltaTime * 2f);
                    currentTarget.ReadyToCapture = false;
                    playerReticleImage.color = originalReticleColor;
                }
            }
        }
        else if (currentTarget)
        {
            currentTarget.HideUI();
            currentTarget = null;
            playerReticleImage.color = originalReticleColor;
        }
    }

    GhostLockOn FindClosestGhost()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, captureRange, ghostLayer);
        float minDist = Mathf.Infinity;
        GhostLockOn best = null;
        
        foreach (var h in hits)
        {
            GhostLockOn g = h.GetComponent<GhostLockOn>();
            if (g && g.gameObject.activeInHierarchy)
            {
                Vector3 dir = (g.transform.position - playerCamera.transform.position).normalized;
                if (Vector3.Dot(playerCamera.transform.forward, dir) > 0.5f)
                {
                    float d = Vector3.Distance(transform.position, g.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        best = g;
                    }
                }
            }
        }
        
        return best;
    }

    bool IsValidTarget(GhostLockOn g)
    {
        if (!g || !g.gameObject.activeInHierarchy) return false;
        float d = Vector3.Distance(transform.position, g.transform.position);
        return d <= captureRange;
    }
}