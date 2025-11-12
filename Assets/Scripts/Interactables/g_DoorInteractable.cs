using UnityEngine;

public class g_DoorInteractable : MonoBehaviour, IInteractable
{
   [SerializeField] private string interactionText = "Press E to open/close door";
   [SerializeField] private string door_open_name = "side_door_open";
   [SerializeField] private string door_close_name = "side_door_close";
   [SerializeField] private string door_closed_name = "side_door_closed";
   
   private Animator animator;
   private bool isOpen = false;
   private bool isAnimating = false;
   
   private void Start()
   {
       animator = GetComponent<Animator>();
       animator.Play(door_closed_name);
   }

    public void Interact()
    {
        if (isAnimating) return;

        isAnimating = true;

        if (isOpen)
        {
            animator.Play(door_close_name);
            isOpen = false;
        }
        else
        {
            animator.Play(door_open_name);
            isOpen = true;
        }

        Invoke(nameof(ResetAnimation), GetAnimationLength());
    }


    private float GetAnimationLength()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        return clipInfo.Length > 0 ? clipInfo[0].clip.length : 1f;
    }
   
   private void ResetAnimation()
   {
       isAnimating = false;
   }
   
   public string GetInteractionText()
   {
       return interactionText;
   }
}