using UnityEngine;

public class g_FilmPickable : MonoBehaviour, IInteractable
{
    public int filmAmount = 6;
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
            g_FilmCounter filmCounter = player.GetComponentInChildren<g_FilmCounter>();
            if (filmCounter == null)
            {
                filmCounter = player.GetComponent<g_FilmCounter>();
            }

            if (filmCounter != null)
            {
                filmCounter.AddFilm(filmAmount);

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
        return "Pick up Film (" + filmAmount + "x)";
    }
}
