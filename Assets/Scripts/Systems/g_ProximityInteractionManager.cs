using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class g_ProximityInteractionManager : MonoBehaviour
{
    [Header("Proximity Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject proximityUIPrefab;
    [SerializeField] private float maxProximityDistance = 5f;
    [SerializeField] private LayerMask interactionLayers = -1;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private float scaleInDuration = 0.25f;
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 1f;
    
    private g_interaction_system interactionSystem;
    private List<ProximityInteractable> allInteractables = new List<ProximityInteractable>();
    private Dictionary<IInteractable, ProximityInteractableUI> activeUIs = new Dictionary<IInteractable, ProximityInteractableUI>();
    private Dictionary<IInteractable, float> interactionCooldowns = new Dictionary<IInteractable, float>();
    private float cooldownDuration = 1f;
    
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        interactionSystem = FindObjectOfType<g_interaction_system>();
        FindAllInteractables();
    }
    
    void Update()
    {
        UpdateCooldowns();
        UpdateProximityInteractions();
    }
    
    void UpdateCooldowns()
    {
        List<IInteractable> toRemove = new List<IInteractable>();
        List<IInteractable> keys = new List<IInteractable>(interactionCooldowns.Keys);
        
        foreach (var key in keys)
        {
            interactionCooldowns[key] = interactionCooldowns[key] - Time.deltaTime;
            if (interactionCooldowns[key] <= 0f)
                toRemove.Add(key);
        }
        
        foreach (var key in toRemove)
            interactionCooldowns.Remove(key);
    }
    
    void FindAllInteractables()
    {
        IInteractable[] interactables = FindObjectsOfType<MonoBehaviour>() as IInteractable[];
        foreach (MonoBehaviour obj in FindObjectsOfType<MonoBehaviour>())
        {
            if (obj is IInteractable interactable)
            {
                ProximityInteractable proximityData = new ProximityInteractable
                {
                    interactable = interactable,
                    transform = obj.transform,
                    collider = obj.GetComponent<Collider>()
                };
                allInteractables.Add(proximityData);
            }
        }
    }
    
    void UpdateProximityInteractions()
    {
        Vector3 playerPos = playerCamera.transform.position;
        
        for (int i = allInteractables.Count - 1; i >= 0; i--)
        {
            var proximityData = allInteractables[i];
            
            if (proximityData.transform == null || !proximityData.transform.gameObject.activeInHierarchy)
            {
                if (activeUIs.ContainsKey(proximityData.interactable))
                {
                    if (activeUIs[proximityData.interactable] != null)
                        activeUIs[proximityData.interactable].HideWithInteraction();
                    activeUIs.Remove(proximityData.interactable);
                }
                allInteractables.RemoveAt(i);
                continue;
            }
            
            float distance = Vector3.Distance(playerPos, proximityData.transform.position);
            
            g_ProximityPromptUtil promptUtil = proximityData.transform.GetComponent<g_ProximityPromptUtil>();
            float proximityDist = promptUtil != null ? promptUtil.GetProximityDistance() : maxProximityDistance;
            
            bool inProximity = distance <= proximityDist;
            bool hasUI = activeUIs.ContainsKey(proximityData.interactable);
            bool isLookingAt = IsLookingAtObject(proximityData);
            bool isOnCooldown = interactionCooldowns.ContainsKey(proximityData.interactable);
            
            if (inProximity && !hasUI && !isOnCooldown)
            {
                CreateProximityUI(proximityData);
            }
            else if ((!inProximity || isOnCooldown) && hasUI)
            {
                RemoveProximityUI(proximityData.interactable);
            }
            
            if (hasUI && activeUIs.TryGetValue(proximityData.interactable, out ProximityInteractableUI ui))
            {
                ui.UpdateLookingState(isLookingAt);
            }
        }
    }
    
    bool IsLookingAtObject(ProximityInteractable proximityData)
    {
        if (interactionSystem == null) return false;
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionSystem.InteractionRange, interactionLayers))
        {
            IInteractable hitInteractable = hit.collider.GetComponent<IInteractable>();
            return hitInteractable == proximityData.interactable;
        }
        return false;
    }
    
    void CreateProximityUI(ProximityInteractable proximityData)
    {
        if (proximityUIPrefab == null) return;
        
        GameObject uiInstance = Instantiate(proximityUIPrefab, proximityData.transform.position, Quaternion.identity);
        ProximityInteractableUI uiComponent = uiInstance.GetComponent<ProximityInteractableUI>();
        
        if (uiComponent == null)
        {
            uiComponent = uiInstance.AddComponent<ProximityInteractableUI>();
        }
        
        uiComponent.Initialize(playerCamera, proximityData.transform, fadeInDuration, fadeOutDuration, scaleInDuration, pulseScale, pulseDuration);
        activeUIs[proximityData.interactable] = uiComponent;
    }
    
    void RemoveProximityUI(IInteractable interactable)
    {
        if (activeUIs.TryGetValue(interactable, out ProximityInteractableUI ui))
        {
            ui.Hide(() => {
                if (ui != null && ui.gameObject != null)
                    Destroy(ui.gameObject);
            });
            activeUIs.Remove(interactable);
        }
    }
    
    public void RefreshInteractables()
    {
        allInteractables.Clear();
        FindAllInteractables();
    }
    
    public void OnObjectInteracted(IInteractable interactable)
    {
        if (activeUIs.TryGetValue(interactable, out ProximityInteractableUI ui))
        {
            ui.HideWithInteraction();
            activeUIs.Remove(interactable);
        }
        
        interactionCooldowns[interactable] = cooldownDuration;
    }
}

[System.Serializable]
public class ProximityInteractable
{
    public IInteractable interactable;
    public Transform transform;
    public Collider collider;
}