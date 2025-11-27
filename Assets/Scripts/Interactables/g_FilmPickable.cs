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
        Debug.Log("Film Pickable Interact() called!");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
            return;
        }

        g_FilmCounter filmCounter = player.GetComponentInChildren<g_FilmCounter>();
        if (filmCounter == null)
        {
            filmCounter = player.GetComponent<g_FilmCounter>();
        }

        if (filmCounter == null)
        {
            Debug.LogError("g_FilmCounter not found on player or children! Add it to player.");
            return;
        }

        Debug.Log("Adding " + filmAmount + " films to counter");
        filmCounter.AddFilm(filmAmount);

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Destroying film pickable");
        Destroy(gameObject);
    }

    public string GetInteractionText()
    {
        return "Pick up Film (" + filmAmount + "x)";
    }
}
