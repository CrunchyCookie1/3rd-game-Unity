using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SquareProxyAudio : MonoBehaviour
{
    [Header("Radio System")]
    [Tooltip("If true, cycles through the audioClips array. If false, plays the AudioSource's assigned clip only.")]
    public bool useRadioSystem = false;

    [Tooltip("Array of clips to cycle through (only used if useRadioSystem is true).")]
    public AudioClip[] audioClips;

    [Tooltip("Optional delay in seconds between clips.")]
    public float delayBetweenClips = 0.1f;

    private AudioSource audioSource;
    private Coroutine radioCoroutine;
    private bool playerInZone = false;
    private int currentIndex = 0;
    private bool hasStarted = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (useRadioSystem)
        {
            audioSource.loop = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = true;

        if (useRadioSystem)
        {
            // If the coroutine is still alive (paused mid-clip), just unpause.
            if (radioCoroutine != null)
            {
                if (audioSource.clip != null && !audioSource.isPlaying)
                {
                    audioSource.UnPause();
                }
                return;
            }

            // First time entering (or after a full reset) — start the coroutine.
            radioCoroutine = StartCoroutine(RadioSequence());
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                // On very first entry, Play() starts from 0.
                // On subsequent entries, UnPause() resumes from where it was.
                if (audioSource.time > 0f)
                    audioSource.UnPause();
                else
                    audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = false;

        if (useRadioSystem)
        {
            // Pause instead of stop so it can resume later.
            // Do NOT stop the coroutine — it will simply wait.
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    private IEnumerator RadioSequence()
    {
        // Safety: if no clips, exit immediately
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning($"{name}: useRadioSystem is true but no audio clips assigned.");
            radioCoroutine = null;
            yield break;
        }

        // Single-clip case: loop it like a normal track
        if (audioClips.Length == 1)
        {
            if (!hasStarted)
            {
                audioSource.clip = audioClips[0];
                audioSource.loop = true;
                audioSource.Play();
                hasStarted = true;
            }
            else
            {
                audioSource.UnPause();
            }

            // Keep the coroutine alive indefinitely.
            // Pausing/resuming is handled by OnTriggerEnter/Exit.
            while (true) yield return null;
        }

        // Multiple clips: cycle through them
        while (true)
        {
            // If we don't have a clip yet, or we're starting a fresh one, assign it.
            if (audioSource.clip != audioClips[currentIndex])
            {
                audioSource.clip = audioClips[currentIndex];
                audioSource.Play();
            }
            else if (!audioSource.isPlaying)
            {
                // Resuming the same clip after a pause
                audioSource.UnPause();
            }

            // Wait for this clip to finish naturally.
            // Note: when paused, isPlaying is false — so we also check
            // that we're not paused (via playerInZone) before advancing.
            while (audioSource.isPlaying || !playerInZone)
            {
                // If paused (not in zone), just idle here.
                if (!playerInZone && !audioSource.isPlaying)
                {
                    yield return null;
                    continue;
                }

                // If the clip finished while player is in zone, break out to advance.
                if (playerInZone && !audioSource.isPlaying)
                    break;

                yield return null;
            }

            // Optional gap between tracks (only when player is still in zone)
            if (delayBetweenClips > 0f && playerInZone)
                yield return new WaitForSeconds(delayBetweenClips);

            // Advance index, wrapping back to 0
            currentIndex = (currentIndex + 1) % audioClips.Length;
        }
    }
}