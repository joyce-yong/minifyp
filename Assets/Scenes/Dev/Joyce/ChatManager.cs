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

    // Username color system
    private Dictionary<string, string> usernameColors = new Dictionary<string, string>();
    private string[] colorOptions = { "#4AA8FF", "#52E78E", "#FFB74D", "#FF6F91", "#C792EA", "#64B5F6", "#F06292", "#AED581" };

    public TextMeshProUGUI chatText;
    public float messageInterval = 0.5f;
    public float stopDelay = 10f;

    private Queue<string> visibleMessages = new Queue<string>();
    public int maxVisibleMessages = 5;

    private string currentEmotion = null;
    private float timer = 0f;
    private float stopTimer = 0f;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("chatMessages");
        if (jsonFile == null)
        {
            Debug.LogError("chatMessages.json must be in Resources folder");
            return;
        }

        string wrappedJson = "{\"messages\":" + jsonFile.text + "}";
        List<ChatMessage> allMessages = JsonUtility.FromJson<ChatWrapper>(wrappedJson).messages;

        // Group messages by emotion
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
            Debug.Log($"Reshuffled chat list for emotion: {emotion}");
        }

        ChatMessage msg = shuffledQueues[emotion].Dequeue();

        // Assign random color if username doesn’t have one yet
        if (!usernameColors.ContainsKey(msg.username))
        {
            string randomColor = colorOptions[Random.Range(0, colorOptions.Length)];
            usernameColors[msg.username] = randomColor;
        }

        string userColor = usernameColors[msg.username];
        string newLine = $"<color={userColor}>{msg.username}</color>: {msg.message}";

        // Stack visible messages
        visibleMessages.Enqueue(newLine);
        if (visibleMessages.Count > maxVisibleMessages)
            visibleMessages.Dequeue();

        chatText.text = string.Join("\n", visibleMessages);
    }

    private Queue<ChatMessage> CreateShuffledQueue(List<ChatMessage> list)
    {
        List<ChatMessage> copy = new List<ChatMessage>(list);

        for (int i = copy.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            ChatMessage temp = copy[i];
            copy[i] = copy[rand];
            copy[rand] = temp;
        }

        return new Queue<ChatMessage>(copy);
    }
}
