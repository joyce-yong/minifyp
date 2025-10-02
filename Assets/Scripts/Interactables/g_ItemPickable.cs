using UnityEngine;

public class g_ItemPickable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText = "Press E to interact";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
    
    public string GetInteractionText()
    {
        return interactionText;
    }
}
