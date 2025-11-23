using UnityEngine;

public class g_HealthPillPickable : MonoBehaviour, IInteractable
{
    public int healAmount = 1;
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            g_PlayerHealth playerHealth = player.GetComponent<g_PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                if (pickupSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }

                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }

    public string GetInteractionText()
    {
        return "Pick up Health Pill";
    }
}
