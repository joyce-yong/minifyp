using UnityEngine;

public interface IInteractable
{
    void Interact();
    string GetInteractionText();
}

public class g_interaction_system : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayers = -1;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    
    private IInteractable currentInteractable;
    private RaycastHit hit;
    
    public float InteractionRange => interactionRange;
    public LayerMask InteractionLayers => interactionLayers;
    
    void Update()
    {
        CheckForInteractable();
        HandleInput();
    }
    
    void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayers))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                }
            }
            else
            {
                currentInteractable = null;
            }
        }
        else
        {
            currentInteractable = null;
        }
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(interactionKey) && currentInteractable != null)
        {
            g_ProximityInteractionManager proximityManager = FindObjectOfType<g_ProximityInteractionManager>();
            if (proximityManager != null)
            {
                proximityManager.OnObjectInteracted(currentInteractable);
            }
            
            currentInteractable.Interact();
            
            Invoke(nameof(ClearCurrentInteractable), 0.1f);
        }
    }
    
    void ClearCurrentInteractable()
    {
        currentInteractable = null;
    }
    
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = currentInteractable != null ? Color.green : Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange);
            
            if (currentInteractable != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }
        }
    }
    
    public bool HasInteractable()
    {
        return currentInteractable != null;
    }
    
    public string GetCurrentInteractionText()
    {
        return currentInteractable?.GetInteractionText() ?? "";
    }
}