using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public string emotion;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChatManager chatManager = FindAnyObjectByType<ChatManager>();
            chatManager.TriggerChatAndBoost(emotion);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Object.FindAnyObjectByType<ChatManager>().StopChat();
        }
    }
}
