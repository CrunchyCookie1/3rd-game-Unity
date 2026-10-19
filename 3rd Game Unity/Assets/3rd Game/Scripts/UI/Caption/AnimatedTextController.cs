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
        [Tooltip("Fires when this specific sentence finishes typing.")]
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
    public InputManager inputManager;   // Drag your InputManager here in the Inspector
    public bool dialogueActive = false; // Set true when dialogue begins

    [Header("Sentence Audio")]
    public AudioSource dialogueAudioSource;  // Assign an AudioSource in the Inspector

    [Header("Sentence Finished Event")]
    public bool enableSentenceFinishedEvent = true; // Toggle this to turn the events on/off

    public UnityEvent onCaptionOpened;

    private Coroutine typingCoroutine;
    private Coroutine autoAdvanceCoroutine;
    private Coroutine sentenceEventCoroutine;
    private string currentSentence = "";

    public void OpenCaption()
    {
        dialogueAnimator.SetTrigger("Enter");
        isAnimationPlaying = true;
        dialogueActive = true;
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

        // Handle interact input while dialogue is active
        if (dialogueActive && inputManager != null && inputManager.interactInput)
        {
            inputManager.interactInput = false; // consume the press so it fires only once
            HandleInteract();
        }
    }

    private void HandleInteract()
    {
        // Cancel any pending auto-advance or delayed sentence event since the player is advancing manually
        CancelAutoAdvance();
        CancelDelayedSentenceEvent();

        // If the sentence is still typing, complete it instantly
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentSentence;

            // Stop the per-sentence audio if it's still playing
            if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
                dialogueAudioSource.Stop();

            // Fire this sentence's finished event immediately (no delay, since player skipped)
            FireSentenceFinishedEvent(Index);

            Index++;
            nextText = true;
            return;
        }

        // Otherwise advance to the next sentence
        NextSentence();
    }

    public void NextSentence()
    {
        if (Index <= sentences.Length - 1 && nextText == true)
        {
            currentSentence = sentences[Index].sentence;
            dialogueText.text = "";

            // Play the audio clip tied to this sentence, if one exists
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

        // If the entry wants a delay, schedule it; otherwise fire immediately
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

    // Call this to turn the sentence-finished event system on or off at runtime
    public void SetSentenceFinishedEventEnabled(bool enabled)
    {
        enableSentenceFinishedEvent = enabled;
    }

    IEnumerator WriteSentence()
    {
        foreach (char character in currentSentence.ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(dialogueSpeed);
        }

        // Fire this sentence's event now that it's finished typing naturally
        // (may be delayed depending on the sentence's settings)
        FireSentenceFinishedEvent(Index);

        // Check if this sentence should auto-advance
        TryScheduleAutoAdvance(Index);

        Index++;
        nextText = true;
        typingCoroutine = null;
    }
}