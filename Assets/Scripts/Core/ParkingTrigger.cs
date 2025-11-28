using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ParkingTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string carTag = "Car";

    [Header("Car Slow Down")]
    public float slowDownDuration = 3f;
    public float targetSpeed = 0f;

    [Header("Spotlights")]
    public List<Light> spotlights = new List<Light>();
    public float delayBetweenLights = 0.3f;
    public float lightFadeDuration = 0.5f;

    [Header("Canvas Fade")]
    public CanvasGroup fadeCanvas;
    public float canvasFadeDelay = 1f;
    public float canvasFadeDuration = 2f;

    [Header("Scene Transition")]
    public string nextSceneName;
    public float sceneChangeDelay = 1f;

    private bool hasTriggered = false;
    private Player_Car_Movement carMovement;

    void Start()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(carTag))
        {
            hasTriggered = true;
            carMovement = other.GetComponent<Player_Car_Movement>();

            StartCoroutine(ParkingSequence());
        }
    }

    IEnumerator ParkingSequence()
    {
        // Step 1: Slowly stop the car
        if (carMovement != null)
        {
            carMovement.LockControls();

            float elapsed = 0f;
            float startSpeed = carMovement.GetCurrentSpeed();

            while (elapsed < slowDownDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / slowDownDuration;
                carMovement.SetSpeed(Mathf.Lerp(startSpeed, targetSpeed, t));
                yield return null;
            }

            carMovement.SetSpeed(targetSpeed);
        }
        else
        {
            yield return new WaitForSeconds(slowDownDuration);
        }

        // Step 2: Turn off spotlights one by one
        if (spotlights.Count > 0)
        {
            for (int i = 0; i < spotlights.Count; i++)
            {
                if (spotlights[i] != null)
                {
                    spotlights[i].DOIntensity(0f, lightFadeDuration);
                }
                yield return new WaitForSeconds(delayBetweenLights);
            }

            // Wait for the last light to finish fading
            yield return new WaitForSeconds(lightFadeDuration);
        }

        // Step 3: Fade in canvas
        yield return new WaitForSeconds(canvasFadeDelay);

        if (fadeCanvas != null)
        {
            fadeCanvas.DOFade(1f, canvasFadeDuration);
            yield return new WaitForSeconds(canvasFadeDuration);
        }

        // Step 4: Change scene
        yield return new WaitForSeconds(sceneChangeDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = hasTriggered ? Color.gray : new Color(1f, 0.5f, 0f); // Orange
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // Draw lines to spotlights
        if (spotlights.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (Light spotlight in spotlights)
            {
                if (spotlight != null)
                {
                    Gizmos.DrawLine(transform.position, spotlight.transform.position);
                }
            }
        }
    }
}
