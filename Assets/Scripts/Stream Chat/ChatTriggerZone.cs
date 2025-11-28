using UnityEngine;

public class ChatTriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string carTag = "Car";
    public bool triggerOnce = true;

    [Header("Chat Emotion")]
    [Tooltip("Emotion to switch chat to: highway, parking, fear, happy, amaze, anger, sarcastic")]
    public string chatEmotion = "parking";
    public bool boostViewers = false;

    [Header("Optional: Stop Chat on Exit")]
    public bool stopChatOnExit = false;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag(carTag))
        {
            ChatManager chatManager = FindAnyObjectByType<ChatManager>();
            if (chatManager != null)
            {
                if (boostViewers)
                {
                    chatManager.TriggerChatAndBoost(chatEmotion);
                }
                else
                {
                    chatManager.StartChat(chatEmotion);
                }
            }
            else
            {
                Debug.LogWarning("ChatTriggerZone: No ChatManager found in scene!");
            }

            hasTriggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (stopChatOnExit && other.CompareTag(carTag))
        {
            ChatManager chatManager = FindAnyObjectByType<ChatManager>();
            if (chatManager != null)
            {
                chatManager.StopChat();
            }
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = hasTriggered ? Color.gray : GetEmotionColor();
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

    Color GetEmotionColor()
    {
        switch (chatEmotion.ToLower())
        {
            case "fear": return new Color(0.5f, 0f, 0.5f); // Purple
            case "happy": return Color.green;
            case "amaze": return Color.cyan;
            case "anger": return Color.red;
            case "parking": return new Color(1f, 0.5f, 0f); // Orange
            case "highway": return Color.yellow;
            case "sarcastic": return Color.magenta;
            default: return Color.white;
        }
    }
}
