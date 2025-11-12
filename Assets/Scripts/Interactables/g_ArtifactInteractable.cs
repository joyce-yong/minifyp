using UnityEngine;

public class g_ArtifactInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionText = "[E]";
    [SerializeField] [TextArea(3, 10)] private string artifactText = "Ancient artifact text goes here...\n\nThis is where you can write multiple lines of story text that will be displayed when the player interacts with this artifact.";
    
    private g_ArtifactManager artifactManager;
    private bool hasBeenInteracted = false;
    
    private void Start()
    {
        artifactManager = FindObjectOfType<g_ArtifactManager>();
        if (artifactManager == null)
        {
            Debug.LogError("g_ArtifactManager not found in scene! Please add it to manage artifact notes.");
        }
    }
    
    public void Interact()
    {
        if (artifactManager != null)
        {
            artifactManager.ShowNote(artifactText);
            
            if (!hasBeenInteracted)
            {
                hasBeenInteracted = true;
                gameObject.SetActive(false);
            }
        }
    }
    
    public string GetInteractionText()
    {
        return interactionText;
    }
}