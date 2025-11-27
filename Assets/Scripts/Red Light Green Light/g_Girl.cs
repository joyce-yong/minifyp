using System.Collections;
using UnityEngine;

public class g_Girl : MonoBehaviour
{
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
    }

    void Update()
    {
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
            //shotAudioSource.Play(); //die sound
            Debug.LogError("Time Over. Player is killed!");
            
        }

        if (scanning)
        {
            if (player.isMoving)
            {
                //shotAudioSource.Play(); 
                Debug.LogError("Player moved. Player is killed!");
            }
        }
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
}
