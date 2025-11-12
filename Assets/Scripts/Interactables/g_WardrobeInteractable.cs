using UnityEngine;

public class g_WardrobeInteractable : MonoBehaviour, IInteractable
{
   [SerializeField] private string interactionText = "Press E to open/close wardrobe";
   [SerializeField] private string wardrobe_open_name = "wardrobe_door_open";
   [SerializeField] private string wardrobe_close_name = "wardrobe_door_close";
   [SerializeField] private string wardrobe_closed_name = "wardrobe_door_closed";
   
   private Animator animator;
   private bool isOpen = false;
   private bool isAnimating = false;
   
   private void Start()
   {
       animator = GetComponent<Animator>();
       isOpen = false;
       animator.Play(wardrobe_closed_name);
   }
   
   public void Interact()
   {
       if (isAnimating) return;
       
       isAnimating = true;
       
       if (isOpen)
       {
           animator.Play(wardrobe_close_name);
           isOpen = false;
       }
       else
       {
           animator.Play(wardrobe_open_name);
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