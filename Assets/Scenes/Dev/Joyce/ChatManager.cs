using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [System.Serializable]
    private class ChatWrapper
    {
        public List<ChatMessage> messages;
    }

    private Dictionary<string, List<ChatMessage>> emotionMessages;
    private Dictionary<string, Queue<ChatMessage>> shuffledQueues;

    public TextMeshProUGUI chatText;
    public float messageInterval = 0.5f;
    public float stopDelay = 10f;

    // Controls visible stacking
    private Queue<string> visibleMessages = new Queue<string>();
    public int maxVisibleMessages = 5;

    private string currentEmotion = null;
    private float timer = 0f;
    private float stopTimer = 0f;

    void Start()
    {
        // Load messages from JSON
        TextAsset jsonFile = Resources.Load<TextAsset>("chatMessages");
        if (jsonFile == null)
        {
            Debug.LogError("chatMessages.json not found in Resources!");
            return;
        }

        string wrappedJson = "{\"messages\":" + jsonFile.text + "}";
        List<ChatMessage> allMessages = JsonUtility.FromJson<ChatWrapper>(wrappedJson).messages;

        // Group by emotion
        emotionMessages = new Dictionary<string, List<ChatMessage>>();
        foreach (var msg in allMessages)
        {
            if (!emotionMessages.ContainsKey(msg.emotion))
                emotionMessages[msg.emotion] = new List<ChatMessage>();

            emotionMessages[msg.emotion].Add(msg);
        }

        // Initialize shuffled queues
        shuffledQueues = new Dictionary<string, Queue<ChatMessage>>();
        foreach (var kvp in emotionMessages)
        {
            shuffledQueues[kvp.Key] = CreateShuffledQueue(kvp.Value);
        }
    }

    void Update()
    {
        if (currentEmotion == null) return;

        if (stopTimer > 0f)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0f)
            {
                currentEmotion = null;
                return;
            }
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ShowNextMessage(currentEmotion);
            timer = messageInterval;
        }
    }

    public void StartChat(string emotion)
    {
        currentEmotion = emotion;
        stopTimer = 0f;
        timer = 0f;
    }

    public void StopChat()
    {
        stopTimer = stopDelay;
    }

    private void ShowNextMessage(string emotion)
    {
        if (!shuffledQueues.ContainsKey(emotion)) return;

        if (shuffledQueues[emotion].Count == 0)
        {
            shuffledQueues[emotion] = CreateShuffledQueue(emotionMessages[emotion]);
        }

        ChatMessage msg = shuffledQueues[emotion].Dequeue();
        string newLine = $"<color=yellow>{msg.username}</color>: {msg.message}";

        // Add message to visible queue
        visibleMessages.Enqueue(newLine);

        // Remove oldest message if over the limit
        if (visibleMessages.Count > maxVisibleMessages)
            visibleMessages.Dequeue();

        // Rebuild text
        chatText.text = string.Join("\n", visibleMessages);
    }

    private Queue<ChatMessage> CreateShuffledQueue(List<ChatMessage> list)
    {
        List<ChatMessage> copy = new List<ChatMessage>(list);
        for (int i = 0; i < copy.Count; i++)
        {
            ChatMessage temp = copy[i];
            int rand = Random.Range(i, copy.Count);
            copy[i] = copy[rand];
            copy[rand] = temp;
        }
        return new Queue<ChatMessage>(copy);
    }
}
