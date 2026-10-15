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

        [Tooltip("Fires when this specific sentence finishes typing.")]
        public UnityEvent onSentenceFinished;

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
        // Cancel any pending auto-advance since the player is manually advancing
        CancelAutoAdvance();

        // If the sentence is still typing, complete it instantly
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentSentence;

            // Stop the per-sentence audio if it's still playing
            if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
                dialogueAudioSource.Stop();

            // Fire this sentence's finished event
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

        sentences[sentenceIndex].onSentenceFinished?.Invoke();
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
        FireSentenceFinishedEvent(Index);

        // Check if this sentence should auto-advance
        TryScheduleAutoAdvance(Index);

        Index++;
        nextText = true;
        typingCoroutine = null;
    }
}