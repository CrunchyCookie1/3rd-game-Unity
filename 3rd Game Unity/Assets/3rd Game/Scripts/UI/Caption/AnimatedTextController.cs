using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class AnimatedTextController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueSentence
    {
        [TextArea(3, 10)]
        public string sentence;

        public AudioClip audioClip;

        [Header("Sentence Finished Event")]
        [Tooltip("Fires when this specific sentence finishes typing (and its audio, if any, has finished playing).")]
        public UnityEvent onSentenceFinished;

        [Tooltip("If true, the On Sentence Finished event will be delayed by the value below.")]
        public bool delaySentenceFinishedEvent = false;

        [Tooltip("How long to wait (in seconds) before firing On Sentence Finished. Only used if Delay Sentence Finished Event is true.")]
        public float sentenceFinishedEventDelay = 0.5f;

        [Header("Auto Advance")]
        [Tooltip("If true, this sentence will automatically move to the next one when it finishes.")]
        public bool autoAdvance = false;

        [Tooltip("How long to wait (in seconds) after this sentence finishes before moving on. Only used if Auto Advance is true.")]
        public float autoAdvanceDelay = 1f;
    }

    public TextMeshProUGUI dialogueText;

    public DialogueSentence[] sentences;
    [SerializeField] private int Index = 0;
    public float dialogueSpeed = 0.05f;
    public Animator dialogueAnimator;
    private bool startDialogue = true;
    private bool nextText = true;
    private bool isAnimationPlaying = false;

    [Header("Input")]
    public InputManager inputManager;
    public bool dialogueActive = false;

    [Header("Sentence Audio")]
    public AudioSource dialogueAudioSource;

    [Header("Sentence Finished Event")]
    public bool enableSentenceFinishedEvent = true;

    public UnityEvent onCaptionOpened;

    private Coroutine typingCoroutine;
    private Coroutine autoAdvanceCoroutine;
    private Coroutine sentenceEventCoroutine;
    private string currentSentence = "";

    [Header("Open Caption Delay")]
    [Tooltip("How long to wait (in seconds) after OpenCaption is called before the caption animation starts.")]
    public float openCaptionDelay = 1f;

    private Coroutine openCaptionCoroutine;

    public void OpenCaption()
    {
        if (openCaptionCoroutine != null)
            StopCoroutine(openCaptionCoroutine);

        openCaptionCoroutine = StartCoroutine(OpenCaptionDelayed());
    }

    private IEnumerator OpenCaptionDelayed()
    {
        if (openCaptionDelay > 0f)
            yield return new WaitForSeconds(openCaptionDelay);

        dialogueAnimator.SetTrigger("Enter");
        isAnimationPlaying = true;
        dialogueActive = true;

        openCaptionCoroutine = null;
    }

    private void Update()
    {
        if (isAnimationPlaying)
        {
            AnimatorStateInfo stateInfo = dialogueAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1.0f && !dialogueAnimator.IsInTransition(0))
            {
                Debug.Log("Animation finished playing.");
                onCaptionOpened?.Invoke();
                isAnimationPlaying = false;
            }
        }

        if (dialogueActive && inputManager != null && inputManager.interactInput)
        {
            inputManager.interactInput = false;
            HandleInteract();
        }
    }

    private void HandleInteract()
    {
        CancelAutoAdvance();
        CancelDelayedSentenceEvent();

        // Player skipped mid-type: instantly complete text, stop audio, fire event, advance
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentSentence;

            if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
                dialogueAudioSource.Stop();

            FireSentenceFinishedEvent(Index);

            Index++;
            nextText = true;
            return;
        }

        // Player pressed interact while text is done but audio might still be playing:
        // treat it as a skip of the remaining audio and advance.
        if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
        {
            dialogueAudioSource.Stop();
        }

        NextSentence();
    }

    public void NextSentence()
    {
        if (Index <= sentences.Length - 1 && nextText == true)
        {
            currentSentence = sentences[Index].sentence;
            dialogueText.text = "";

            PlaySentenceAudio(Index);

            typingCoroutine = StartCoroutine(WriteSentence());
            nextText = false;
        }
    }

    private void PlaySentenceAudio(int sentenceIndex)
    {
        if (dialogueAudioSource == null) return;
        if (sentences == null) return;
        if (sentenceIndex < 0 || sentenceIndex >= sentences.Length) return;

        AudioClip clip = sentences[sentenceIndex].audioClip;
        if (clip == null) return;

        dialogueAudioSource.Stop();
        dialogueAudioSource.clip = clip;
        dialogueAudioSource.Play();
    }

    private void FireSentenceFinishedEvent(int sentenceIndex)
    {
        if (!enableSentenceFinishedEvent) return;
        if (sentences == null) return;
        if (sentenceIndex < 0 || sentenceIndex >= sentences.Length) return;

        DialogueSentence entry = sentences[sentenceIndex];

        if (entry.delaySentenceFinishedEvent)
        {
            CancelDelayedSentenceEvent();
            sentenceEventCoroutine = StartCoroutine(FireSentenceEventAfterDelay(entry, entry.sentenceFinishedEventDelay));
        }
        else
        {
            entry.onSentenceFinished?.Invoke();
        }
    }

    private IEnumerator FireSentenceEventAfterDelay(DialogueSentence entry, float delay)
    {
        yield return new WaitForSeconds(delay);

        sentenceEventCoroutine = null;
        entry.onSentenceFinished?.Invoke();
    }

    private void CancelDelayedSentenceEvent()
    {
        if (sentenceEventCoroutine != null)
        {
            StopCoroutine(sentenceEventCoroutine);
            sentenceEventCoroutine = null;
        }
    }

    private void TryScheduleAutoAdvance(int sentenceIndex)
    {
        if (sentences == null) return;
        if (sentenceIndex < 0 || sentenceIndex >= sentences.Length) return;
        if (!sentences[sentenceIndex].autoAdvance) return;

        CancelAutoAdvance();
        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterDelay(sentences[sentenceIndex].autoAdvanceDelay));
    }

    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        autoAdvanceCoroutine = null;
        NextSentence();
    }

    private void CancelAutoAdvance()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }
    }

    public void ExitCaption()
    {
        CancelAutoAdvance();
        CancelDelayedSentenceEvent();

        dialogueText.text = "";
        dialogueAnimator.SetTrigger("Exit");
        dialogueActive = false;

        if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
            dialogueAudioSource.Stop();
    }

    public void SetSentenceFinishedEventEnabled(bool enabled)
    {
        enableSentenceFinishedEvent = enabled;
    }

    IEnumerator WriteSentence()
    {
        foreach (char character in currentSentence.ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSecondsRealtime(dialogueSpeed);
        }

        // Text finished typing — wait for the sentence's audio (if any) to finish playing.
        yield return WaitForSentenceAudioToFinish();

        // Both text AND audio are now complete.
        FireSentenceFinishedEvent(Index);
        TryScheduleAutoAdvance(Index);

        Index++;
        nextText = true;
        typingCoroutine = null;
    }

    /// <summary>
    /// Yields each frame while the sentence audio source is still playing.
    /// Returns immediately if there's no AudioSource or nothing is playing.
    /// Uses a reference to the current clip so that a clip change mid-wait
    /// doesn't incorrectly keep us waiting on the new clip.
    /// </summary>
    private IEnumerator WaitForSentenceAudioToFinish()
    {
        if (dialogueAudioSource == null) yield break;

        AudioClip clipThatMustFinish = dialogueAudioSource.isPlaying ? dialogueAudioSource.clip : null;
        if (clipThatMustFinish == null) yield break;

        while (dialogueAudioSource != null
               && dialogueAudioSource.isPlaying
               && dialogueAudioSource.clip == clipThatMustFinish)
        {
            yield return null;
        }
    }
}