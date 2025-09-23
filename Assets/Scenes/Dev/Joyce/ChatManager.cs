using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [System.Serializable]
    private class ChatWrapper
    {
        public List<ChatMessage> messages;
    }

    [System.Serializable]
    public class ChatMessage
    {
        public string username;
        public string message;
        public string emotion;
    }

    private List<ChatMessage> allMessages;
    private string currentEmotion = null;
    private List<ChatMessage> filteredMessages;

    public TextMeshProUGUI chatText;
    public ScrollRect scrollRect; 
    public float messageInterval = 3f;
    private float timer;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("chatMessages");
        if (jsonFile != null)
        {
            string wrappedJson = "{\"messages\":" + jsonFile.text + "}";
            allMessages = JsonUtility.FromJson<ChatWrapper>(wrappedJson).messages;
        }
        else
        {
            Debug.LogError("chatMessages.json not found in Resources/");
        }

        timer = messageInterval;
        filteredMessages = new List<ChatMessage>();
    }

    void Update()
    {
        if (allMessages == null || allMessages.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.E)) SetCurrentEmotion("excited");
        if (Input.GetKeyDown(KeyCode.S)) SetCurrentEmotion("scared");
        if (Input.GetKeyDown(KeyCode.C)) SetCurrentEmotion("challenge");

        if (!string.IsNullOrEmpty(currentEmotion))
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ShowNextMessage();
                timer = messageInterval;
            }
        }
    }

    void SetCurrentEmotion(string emotion)
    {
        currentEmotion = emotion;
        ResetFilteredMessages();
    }

    void ResetFilteredMessages()
    {
        filteredMessages = allMessages.FindAll(m => m.emotion.ToLower() == currentEmotion.ToLower());
    }

    void ShowNextMessage()
    {
        if (filteredMessages.Count == 0) ResetFilteredMessages();

        int index = Random.Range(0, filteredMessages.Count);
        ChatMessage msg = filteredMessages[index];
        filteredMessages.RemoveAt(index);

        chatText.text += $"\n<color=yellow>{msg.username}</color>: {msg.message}";

        // Scroll to bottom smoothly
        Canvas.ForceUpdateCanvases(); // Force layout update
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
