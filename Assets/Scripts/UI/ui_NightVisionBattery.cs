using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ui_NightVisionBattery : MonoBehaviour
{
    [SerializeField] private GameObject normalVisionVolume;
    [SerializeField] private GameObject nightVisionVolume;
    [SerializeField] private GameObject phoneCameraOverlay;
    [SerializeField] private List<GameObject> batteryBars;
    [SerializeField] private KeyCode toggleKey = KeyCode.Q;
    [SerializeField] private float maxBars = 6f;
    [SerializeField] private float drainBarsPerSecond = 0.5f;
    [SerializeField] private cam_PlayerView playerCamera; 

    private float currentBars;
    public bool nightVisionOn { get; private set; } 

    void Awake()
    {
        currentBars = maxBars;
        nightVisionOn = false;
        ApplyVision(false);
        RefreshBars();

        if (playerCamera == null)
            playerCamera = FindObjectOfType<cam_PlayerView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool previousState = nightVisionOn;
            
            if (!nightVisionOn && currentBars <= 0f) return;
            nightVisionOn = !nightVisionOn;
            if (nightVisionOn && currentBars <= 0f) nightVisionOn = false;
            
            ApplyVision(nightVisionOn);
            
            // Reset zoom when toggling night vision
            if (previousState && !nightVisionOn && playerCamera != null)
                playerCamera.ResetZoom();
        }

        if (nightVisionOn)
        {
            currentBars -= drainBarsPerSecond * Time.deltaTime;
            if (currentBars <= 0f)
            {
                currentBars = 0f;
                nightVisionOn = false;
                ApplyVision(false);
                
                // Reset zoom when battery runs out
                if (playerCamera != null)
                    playerCamera.ResetZoom();
            }
            RefreshBars();
        }
    }

    private void ApplyVision(bool enableNight)
    {
        if (nightVisionVolume) nightVisionVolume.SetActive(enableNight);
        if (normalVisionVolume) normalVisionVolume.SetActive(!enableNight);
        if (phoneCameraOverlay) phoneCameraOverlay.SetActive(enableNight);
    }

    private void RefreshBars()
    {
        int activeBars = Mathf.CeilToInt(Mathf.Clamp(currentBars, 0f, maxBars));
        for (int i = 0; i < batteryBars.Count; i++)
        {
            bool shouldBeActive = i < activeBars;
            if (batteryBars[i] && batteryBars[i].activeSelf != shouldBeActive)
                batteryBars[i].SetActive(shouldBeActive);
        }
    }

    public void AddBatteries(int amount)
    {
        currentBars = Mathf.Clamp(currentBars + amount, 0f, maxBars);
        RefreshBars();
    }

    // Public method to check if night vision is on (for other scripts)
    public bool IsNightVisionActive()
    {
        return nightVisionOn;
    }
}