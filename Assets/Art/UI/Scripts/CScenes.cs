using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.XR;

public class CScenes : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public List<GameObject> pages;
    private int currentPageIndex = 0;

    private List<GameObject> currentSteps = new List<GameObject>();
    private int currentStepIndex = 0;

    [Header("Audio Settings")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    [Header("Countdown Settings")]
    public TMP_Text countdownText;
    public int countdownStart = 5;
    private bool firstStepCountdownDone = false;
    private bool inputLocked = false;

    [Header("Fade Settings")]
    public Image black;
    [Tooltip("Animator state name for appear animation (must match state name in Animator Controller).")]
    public string appearStateName = "Appear";

    private InputDevice rightController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        InitializeControllers();
        ShowPage(0);
    }



    void InitializeControllers()
    {
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            rightController = rightHandedControllers[0];
        }
    }

    void Update()
    {
        if (inputLocked) return;

        if (!rightController.isValid)
            InitializeControllers();

        bool triggerPressed = false;
        if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
        {
            ShowNextStep();
        }

        if (Input.GetMouseButtonDown(0))
        {
            ShowNextStep();
        }
    }

    void ShowPage(int index)
    {
        foreach (var page in pages) page.SetActive(false);
        currentPageIndex = index;

        if (currentPageIndex >= pages.Count)
        {
            EndCutscene();
            return;
        }

        GameObject currentPage = pages[currentPageIndex];
        currentPage.SetActive(true);

        currentSteps.Clear();
        foreach (Transform child in currentPage.transform)
        {
            if (child.CompareTag("CutscenesElement"))
            {
                child.gameObject.SetActive(false);
                currentSteps.Add(child.gameObject);
            }
        }

        Button nextButton = currentPage.transform.Find("Next")?.GetComponent<Button>();
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNextPage);
        }

        currentStepIndex = 0;

        // --- First page special handling ---
        if (currentPageIndex == 0 && !firstStepCountdownDone)
        {
            GameObject firstStep = currentSteps[0];
            firstStep.SetActive(true);

            // Play Step0 animation immediately
            StartCoroutine(PlayAppearStateNextFrame(firstStep));

            // Start countdown on Step0
            StartCoroutine(CountdownAndAutoStep1());
        }
        else
        {
            ShowNextStep();
        }
    }

    IEnumerator CountdownAndAutoStep1()
    {
        inputLocked = true;
        countdownText.gameObject.SetActive(true);

        int count = countdownStart;
        while (count >= 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.gameObject.SetActive(false);
        firstStepCountdownDone = true;

        // Automatically go to Step1 after countdown
        yield return new WaitForSeconds(0.3f);

        if (currentSteps.Count > 1)
        {
            GameObject step1 = currentSteps[1];
            step1.SetActive(true);
            StartCoroutine(PlayAppearStateNextFrame(step1));
            currentStepIndex = 2;
        }

        inputLocked = false;
    }

    void ShowNextStep()
    {
        if (currentStepIndex < currentSteps.Count)
        {
            // Stop all currently playing audios in this page
            AudioSource[] allAudios = pages[currentPageIndex].GetComponentsInChildren<AudioSource>();
            foreach (var a in allAudios)
            {
                if (a.isPlaying) a.Stop();
            }

            GameObject step = currentSteps[currentStepIndex];
            step.SetActive(true);
            StartCoroutine(PlayAppearStateNextFrame(step));

            if (clickSound != null)
                audioSource.PlayOneShot(clickSound);

            currentStepIndex++;

            // Enable next button when last step on page done
            if (currentStepIndex == currentSteps.Count)
            {
                Button nextButton = pages[currentPageIndex].transform.Find("Next")?.GetComponent<Button>();
                if (nextButton != null)
                    nextButton.gameObject.SetActive(true);
            }
        }
    }

    void ShowNextPage()
    {
        // If this is the last page, don't hide it — just fade and change scene
        if (currentPageIndex >= pages.Count - 1)
        {
            StartCoroutine(FadeOutAndChangeScene());
            return;
        }

        // Otherwise: hide current page and show next
        pages[currentPageIndex].SetActive(false);
        ShowPage(currentPageIndex + 1);
    }


    void EndCutscene()
    {
        StartCoroutine(FadeOutAndChangeScene());
    }

    IEnumerator FadeOutAndChangeScene()
    {
        inputLocked = true;
        black.gameObject.SetActive(true);

        // ensure alpha starts at 0
        Color color = black.color;
        color.a = 0f;
        black.color = color;

        float duration = 1.5f; // fade time in seconds
        float t = 0f;

        // gradually increase alpha
        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / duration);
            black.color = color;
            yield return null;
        }

        color.a = 1f;
        black.color = color;

        // wait a moment before switching scenes
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("SampleScene");
    }



    IEnumerator PlayAppearStateNextFrame(GameObject step)
    {
        yield return null; // wait one frame after activation

        Animator stepAnim = step.GetComponent<Animator>();
        if (stepAnim != null)
        {
            stepAnim.enabled = true;
            stepAnim.Rebind();

            if (!string.IsNullOrEmpty(appearStateName))
            {
                stepAnim.Play(appearStateName, 0, 0f);
            }
            else
            {
                stepAnim.ResetTrigger("Appear");
                stepAnim.SetTrigger("Appear");
            }
        }

        AudioSource stepAudio = step.GetComponent<AudioSource>();
        if (stepAudio != null)
        {
            stepAudio.Stop();
            stepAudio.Play();
        }
    }
}
