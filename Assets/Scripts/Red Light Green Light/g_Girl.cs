using System.Collections;
using UnityEngine;

public class g_Girl : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] bool isGameStarted = false;
    [SerializeField] float preDeathDelay = 1.5f;
    [SerializeField] float musicFadeDuration = 1.0f;

    public bool IsGameStarted => isGameStarted;

    [Header("Doll Setting")]
    [SerializeField] AudioSource girlSingingAudioSource;
    [SerializeField] AudioSource rotationAudioSource;
    [SerializeField] AudioSource shotAudioSource;

    [SerializeField] AudioClip girlSinging;
    [SerializeField] AudioClip rotationSoundClip;
    [SerializeField] AudioClip shotClip;

    [SerializeField] float totalTime = 80;//can adjust total time
    [SerializeField] float breakTime = 4f;

    [SerializeField] Transform doll; 

    [SerializeField] float rotationAngle = 90f;
    [SerializeField] float initialRotationY = 0f;

    readonly float initialSoundDuration = 5.5f; //Initial girl singing speed
    readonly float finalSoundDuration = 2.5f;

    float elapsedTime = 0f;
    bool isPlaying = false;

    Coroutine rotationCoroutine = null;
    bool scanning = false;

    private g_GameStartTrigger startTrigger;
    g_PlayerMovement player;

    void Awake()
    {
        if (girlSingingAudioSource == null || girlSinging == null || 
            rotationAudioSource == null || rotationSoundClip == null || 
            shotAudioSource == null || shotClip == null)
        {
            Debug.LogError("Audio sources or Sound clips not assigned");
            return;
        }

        girlSingingAudioSource.clip = girlSinging;
        girlSingingAudioSource.loop = false;

        rotationAudioSource.clip = rotationSoundClip;
        rotationAudioSource.loop = false;

        shotAudioSource.clip = shotClip;
        shotAudioSource.loop = false;

        player = GameObject.FindWithTag("Player").GetComponent<g_PlayerMovement>();

        startTrigger = FindAnyObjectByType<g_GameStartTrigger>();
        if (startTrigger == null)
        {
            Debug.LogError("g_GameStartTrigger not found in the scene! Cannot reset game properly.");
        }

        this.enabled = false;
    }

    void Update()
    {
        if (!isGameStarted) return;
        if (elapsedTime < totalTime)
        {
        elapsedTime += Time.deltaTime;

            if (!isPlaying)
            {
                float currentSoundDuration = Mathf.Lerp(initialSoundDuration, finalSoundDuration, elapsedTime / totalTime);
                girlSingingAudioSource.pitch = initialSoundDuration / currentSoundDuration;
                girlSingingAudioSource.Play();
                isPlaying = true;

                Invoke(nameof(StopSound), currentSoundDuration);
            }
        }

        if (elapsedTime >= totalTime)
        {
            Debug.Log("Time Over. Player is killed!");
            if (!player.PlayerIsDead())
            {
                StartCoroutine(PreDeathSequence());
            }
            return;
            
        }

        if (scanning)
        {
            if (player.isMoving)
            {
                Debug.Log("Player moved. Player is killed!");
                StartCoroutine(PreDeathSequence());
            }
        }
    }
    public void StartGame()
    {
        if (isGameStarted) return; 

        isGameStarted = true;
        this.enabled = true;
    }
    void StopSound()
    {
        girlSingingAudioSource.Stop();
        Rotate();

        Invoke(nameof(ResumePlayback), breakTime);
    }

    void ResumePlayback()
    {
        isPlaying = false;
        scanning = false;
        Rotate(true);
    }

    void Rotate(bool rotateBack = false)
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationAudioSource.Play();
        rotationCoroutine = StartCoroutine(RotateOverTime(0.2f, rotateBack));
    }

    IEnumerator RotateOverTime(float seconds, bool rotateBack = false)
    {
        float elapsedTime = 0; //local one
        Quaternion startRotation = doll.rotation;
        Quaternion endRotation = Quaternion.Euler(0, rotateBack ? initialRotationY : rotationAngle, 0); //rotation angle

        while (elapsedTime < seconds)
        {
            doll.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scanning = !rotateBack;
    }

    IEnumerator PreDeathSequence()
    {
        shotAudioSource.Play(); 
        yield return new WaitForSeconds(preDeathDelay);
        player.KillPlayer();

    }

    public void PlayerWon()
    {
        if (!isGameStarted) return; 
        StopGameRound();

        StartCoroutine(FadeOutMusicAndStopGame());

        isGameStarted = false;
        this.enabled = false;

        Debug.Log("Player Won!");
    }

    IEnumerator FadeOutMusicAndStopGame()
    {
        float startVolume = girlSingingAudioSource.volume;
        float timer = 0f;

        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0f, timer / musicFadeDuration);
            girlSingingAudioSource.volume = newVolume;
            yield return null;
        }

        girlSingingAudioSource.Stop();
        girlSingingAudioSource.volume = startVolume; 

        
    }
    void StopGameRound()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
        CancelInvoke(); 

        if (girlSingingAudioSource.isPlaying)
        {
            
        }

        doll.rotation = Quaternion.Euler(0, initialRotationY, 0);
        isPlaying = false;
        scanning = false;
    }

    public void StopGame()
    {
        StopGameRound(); 

        isGameStarted = false;
        this.enabled = false;
        elapsedTime = 0f;

        girlSingingAudioSource.Stop();
        girlSingingAudioSource.volume = 1f; 

        if (startTrigger != null)
        {
            startTrigger.EnableTrigger();
        }

        Debug.Log("Game Stopped, awaiting player re-entry into start trigger.");
    }

    public void ResetGameRound()
    {
        StopGameRound(); 
    }
}
