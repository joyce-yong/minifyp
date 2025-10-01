using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public string emotion; // e.g. "excited", "scared"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<ChatManager>().StartChat(emotion);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<ChatManager>().StopChat();
        }
    }
}
