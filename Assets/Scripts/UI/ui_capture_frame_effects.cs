using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CaptureFrameEffects : MonoBehaviour
{
    [Header("Frame Images")]
    public RawImage captureFrameFlash;
    public RawImage captureFrameVisible;
    public Image captureMask;
    
    [Header("Flash Settings")]
    public float flashDuration = 0.15f;
    public float frameDuration = 0.5f;
    public float freezeDuration = 2f;
    
    [Header("Mask Settings")]
    public Color maskColor = new Color(0, 0, 0, 0.7f);
    
    [Header("Background Flash")]
    public Image backgroundFlash;
    public Color flashColor = Color.white;
    public int flashCount = 3;
    public float flashInterval = 0.1f;
    
    [Header("Audio")]
    public AudioSource captureSound;
    public AudioClip shutterSound;
    public AudioClip freezeSound;

    void Start()
    {
        if (captureFrameFlash) captureFrameFlash.gameObject.SetActive(false);
        if (captureFrameVisible) captureFrameVisible.gameObject.SetActive(false);
        if (captureMask) captureMask.gameObject.SetActive(false);
        if (backgroundFlash) backgroundFlash.gameObject.SetActive(false);
        
        SetupMask();
    }

    void SetupMask()
    {
        if (captureMask)
        {
            captureMask.color = new Color(maskColor.r, maskColor.g, maskColor.b, 0);
            captureMask.raycastTarget = false;
        }
        
        if (backgroundFlash)
        {
            backgroundFlash.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
            backgroundFlash.raycastTarget = false;
        }
    }

    public void PlayCaptureSequence(GhostLockOn ghost)
    {
        StartCoroutine(CaptureSequence(ghost));
    }

    System.Collections.IEnumerator CaptureSequence(GhostLockOn ghost)
    {
        if (captureSound && shutterSound)
            captureSound.PlayOneShot(shutterSound);
        
        yield return StartCoroutine(FlashEffect());
        
        yield return StartCoroutine(ShowFrame());
        
        if (ghost)
        {
            ghost.gameObject.layer = LayerMask.NameToLayer("Default");
            FreezeGhost(ghost);
        }
        
        yield return new WaitForSeconds(freezeDuration);
        
        if (captureSound && freezeSound)
            captureSound.PlayOneShot(freezeSound);
        
        HideFrame();
        
        if (ghost)
        {
            ghost.Capture();
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (captureFrameFlash)
        {
            captureFrameFlash.gameObject.SetActive(true);
            captureFrameFlash.color = Color.white;
            
            yield return new WaitForSeconds(flashDuration);
            
            captureFrameFlash.gameObject.SetActive(false);
        }
        
        if (backgroundFlash)
        {
            StartCoroutine(BackgroundFlashSequence());
        }
    }

    System.Collections.IEnumerator BackgroundFlashSequence()
    {
        backgroundFlash.gameObject.SetActive(true);

        for (int i = 0; i < flashCount; i++)
        {
            if (backgroundFlash == null) yield break;
            backgroundFlash.DOKill();
            backgroundFlash.DOFade(0.8f, flashInterval * 0.3f);
            yield return new WaitForSeconds(flashInterval * 0.3f);

            if (backgroundFlash == null) yield break;
            backgroundFlash.DOKill();
            backgroundFlash.DOFade(0f, flashInterval * 0.7f);
            yield return new WaitForSeconds(flashInterval * 0.7f);
        }

        if (backgroundFlash != null)
            backgroundFlash.gameObject.SetActive(false);
    }

    System.Collections.IEnumerator ShowFrame()
    {
        if (captureFrameVisible)
        {
            captureFrameVisible.DOKill();
            captureFrameVisible.gameObject.SetActive(true);
            captureFrameVisible.color = Color.white;
        }

        if (captureMask)
        {
            captureMask.DOKill();
            captureMask.gameObject.SetActive(true);
            captureMask.DOColor(maskColor, 0.3f);
        }

        yield return new WaitForSeconds(frameDuration);
    }

    void HideFrame()
    {
        if (captureFrameVisible)
        {
            captureFrameVisible.DOKill();
            RawImage frame = captureFrameVisible;
            captureFrameVisible.DOFade(0f, 0.3f).OnComplete(() =>
            {
                if (frame != null)
                    frame.gameObject.SetActive(false);
            });
        }

        if (captureMask)
        {
            captureMask.DOKill();
            Image mask = captureMask;
            captureMask.DOFade(0f, 0.3f).OnComplete(() =>
            {
                if (mask != null)
                    mask.gameObject.SetActive(false);
            });
        }
    }

    void FreezeGhost(GhostLockOn ghost)
    {
        Animator animator = ghost.GetComponent<Animator>();
        if (animator)
        {
            animator.enabled = false;
        }

        Rigidbody rb = ghost.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
        }

        MonoBehaviour[] scripts = ghost.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != ghost && script.enabled)
            {
                script.enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        if (captureFrameFlash != null) captureFrameFlash.DOKill();
        if (captureFrameVisible != null) captureFrameVisible.DOKill();
        if (captureMask != null) captureMask.DOKill();
        if (backgroundFlash != null) backgroundFlash.DOKill();
    }
}