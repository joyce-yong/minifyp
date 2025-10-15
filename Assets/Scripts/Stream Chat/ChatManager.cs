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
    private Dictionary<string, Color> usernameColors;
    private Dictionary<string, string[]> emojiCategories;

    public TextMeshProUGUI chatText;
    public Vector2 messageIntervalRange = new Vector2(0.4f, 1.0f);
    public float stopDelay = 10f;
    public TextAsset emojiJson; 

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

        emotionMessages = new Dictionary<string, List<ChatMessage>>();
        foreach (var msg in allMessages)
        {
            if (!emotionMessages.ContainsKey(msg.emotion))
                emotionMessages[msg.emotion] = new List<ChatMessage>();

            emotionMessages[msg.emotion].Add(msg);
        }

        shuffledQueues = new Dictionary<string, Queue<ChatMessage>>();
        foreach (var kvp in emotionMessages)
        {
            shuffledQueues[kvp.Key] = CreateShuffledQueue(kvp.Value);
        }

        AssignUsernameColors(allMessages);

        LoadEmojis();
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
            timer = Random.Range(messageIntervalRange.x, messageIntervalRange.y); //randomise the interval between chat spawn
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
        string colorHex = ColorUtility.ToHtmlStringRGB(usernameColors[msg.username]);
        string emoji = GetRandomEmoji(msg.emotion);

        string newLine = $"<b><color=#{colorHex}>{msg.username}</color></b>: {msg.message}{emoji}";

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
            (copy[i], copy[rand]) = (copy[rand], copy[i]);
        }
        return new Queue<ChatMessage>(copy);
    }

    private void AssignUsernameColors(List<ChatMessage> messages)
    {
        usernameColors = new Dictionary<string, Color>();

        foreach (var msg in messages)
        {
            if (!usernameColors.ContainsKey(msg.username))
            {
                // Generate random color but keep it bright
                Color randomColor = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
                usernameColors[msg.username] = randomColor;
            }
        }
    }


    private void LoadEmojis()
    {
        emojiCategories = new Dictionary<string, string[]>();

        EmojiCategory data = JsonUtility.FromJson<EmojiCategory>(emojiJson.text);
        emojiCategories["excited"] = data.excited;
        emojiCategories["scared"] = data.scared;
    }

    private string GetRandomEmoji(string emotion)
    {
        if (emojiCategories == null || !emojiCategories.ContainsKey(emotion))
            return "";

        if (Random.value > 0.6f)
        {
            string[] emojis = emojiCategories[emotion];
            if (emojis.Length > 0)
                return " " + emojis[Random.Range(0, emojis.Length)];
        }
        return "";
    }

    [System.Serializable]
    private class EmojiCategory
    {
        public string[] excited;
        public string[] scared;
    }
}
