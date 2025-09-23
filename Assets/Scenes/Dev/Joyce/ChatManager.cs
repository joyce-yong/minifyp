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

    private List<ChatMessage> allMessages;

    public TextMeshProUGUI chatText; 
    public float messageInterval = 0.5f; 
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
    }

    void Update()
    {
        if (allMessages == null || allMessages.Count == 0) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ShowRandomMessage();
            timer = messageInterval;
        }
    }

    void ShowRandomMessage()
    {
        int index = Random.Range(0, allMessages.Count);
        ChatMessage msg = allMessages[index];

        chatText.text += $"\n<color=yellow>{msg.username}</color>: {msg.message}";
    }
}
