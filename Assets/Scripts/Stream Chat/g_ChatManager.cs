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

    [Header("Chat Settings")]
    public TextMeshProUGUI chatText;
    public Vector2 messageIntervalRange = new Vector2(0.4f, 1.0f);
    public float stopDelay = 10f;
    public TextAsset emojiJson;
    public int maxVisibleMessages = 5;

    private Queue<string> visibleMessages = new Queue<string>();
    private string currentEmotion = null;
    private float timer = 0f;
    private float stopTimer = 0f;

    [Header("Viewer Settings")]
    public TextMeshProUGUI viewerCountText;
    public int startingViewers = 10;
    public float normalIncreaseRate = 0.5f;
    public float boostIncreaseRate = 5f;
    public float boostDuration = 3f;

    [Header("Intro Sequence")]
    public bool enableIntroSequence = false;
    public float introMessageInterval = 1.5f;
    public int introViewerIncrement = 5;

    private int currentViewers;
    private float viewerTimer = 0f;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private bool isIntroActive = false;

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

        currentViewers = startingViewers;
        UpdateViewerUI();

        if (enableIntroSequence)
        {
            StartIntroSequence();
        }
    }

    void Update()
    {
        if (isIntroActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ShowNextMessage("intro");
                timer = introMessageInterval;

                if (Random.value > 0.7f)
                {
                    currentViewers += introViewerIncrement;
                    UpdateViewerUI();
                }
            }
            return;
        }

        if (currentEmotion != null)
        {
            if (stopTimer > 0f)
            {
                stopTimer -= Time.deltaTime;
                if (stopTimer <= 0f)
                {
                    currentEmotion = null;
                }
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ShowNextMessage(currentEmotion);
                timer = Random.Range(messageIntervalRange.x, messageIntervalRange.y);
            }
        }

        viewerTimer += Time.deltaTime;

        if (viewerTimer >= 1f)
        {
            viewerTimer = 0f;

            if (Random.value < 0.3f)
                return;

            int randomChange;
            if (isBoosting)
            {
                randomChange = Random.Range(15, 30);
            }
            else
            {
                randomChange = Random.Range(-5, 2);
            }

            currentViewers = Mathf.Max(startingViewers, currentViewers + randomChange);
            UpdateViewerUI();
        }

        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
            }
        }
    }

    public void TriggerChatAndBoost(string emotion)
    {
        StartChat(emotion);
        BoostViewers();
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

    public void StartIntroSequence()
    {
        isIntroActive = true;
        timer = 0f;
    }

    public void StopIntroSequence()
    {
        isIntroActive = false;
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
                Color randomColor = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
                usernameColors[msg.username] = randomColor;
            }
        }
    }

    private void LoadEmojis()
    {
        emojiCategories = new Dictionary<string, string[]>();

        EmojiCategory data = JsonUtility.FromJson<EmojiCategory>(emojiJson.text);
        emojiCategories["happy"] = data.happy;
        emojiCategories["fear"] = data.fear;
        emojiCategories["sarcastic"] = data.sarcastic;
        emojiCategories["amaze"] = data.amaze;
        emojiCategories["anger"] = data.anger;
        emojiCategories["highway"] = data.highway;
        emojiCategories["parking"] = data.parking;
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

    private void UpdateViewerUI()
    {
        if (viewerCountText != null)
            viewerCountText.text = currentViewers.ToString("N0");
    }

    private void BoostViewers()
    {
        isBoosting = true;
        boostTimer = boostDuration;
    }

    [System.Serializable]
    private class EmojiCategory
    {
        public string[] happy;
        public string[] fear;
        public string[] sarcastic;
        public string[] amaze;
        public string[] anger;
        public string[] highway;
        public string[] parking;
    }
}
