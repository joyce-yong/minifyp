using UnityEngine;

public class g_ExampleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText = "Press E to interact";
    
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
    
    public string GetInteractionText()
    {
        return interactionText;
    }
}