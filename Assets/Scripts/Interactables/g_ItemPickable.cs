using UnityEngine;

public class g_ItemPickable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText = "Press E to interact";

    private g_InventoryManager inventoryManager;
    public g_ItemSO itemScriptableObject;

    void Start()
    {
        inventoryManager = FindObjectOfType<g_InventoryManager>();
    }

    void Update()
    {

    }
    
    public void Interact()
    {
        if (inventoryManager != null)
        {
            inventoryManager.ItemPicked(gameObject);
            Debug.Log("Interacted with " + gameObject.name);
        }
        else
        {
            Debug.LogError("Inventory Manager not found in scene!");
        }
    }
    
    public string GetInteractionText()
    {
        return interactionText;
    }
}
