using UnityEngine;

public class g_HandAnimator : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;
    [SerializeField] private g_interaction_system interactionSystem;
    
    void Start()
    {
        if (handAnimator == null)
            handAnimator = GetComponent<Animator>();
            
        if (interactionSystem == null)
            interactionSystem = FindObjectOfType<g_interaction_system>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactionSystem.HasInteractable())
        {
            PlayInteractAnimation();
        }
    }
    
    void PlayInteractAnimation()
    {
        handAnimator.SetTrigger("interact");
    }
    
    public void ReturnToIdle()
    {
        handAnimator.SetTrigger("idle");
    }
}