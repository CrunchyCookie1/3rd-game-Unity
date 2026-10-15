using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicTrackList : MonoBehaviour
{
    public AudioClip[] audioClips; // Your array of clips
    private AudioSource audioSource;
    private int currentIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // Ensure it doesn't loop individual clips
        audioSource.playOnAwake = false;

        if (audioClips.Length > 0)
        {
            StartCoroutine(PlayRadioSequence());
        }
    }

    IEnumerator PlayRadioSequence()
    {
        while (true) // Infinite loop to cycle like a radio
        {
            // 1. Assign the current clip from the array
            audioSource.clip = audioClips[currentIndex];

            // 2. Play the clip
            audioSource.Play();

            // 3. Wait until the AudioSource finishes playing this clip
            // This waits frame-by-frame while isPlaying is true
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            // 4. Optional: Wait a tiny bit to ensure clean transition
            yield return new WaitForSeconds(0.1f);

            // 5. Move to the next clip, wrapping back to 0 if at the end
            currentIndex = (currentIndex + 1) % audioClips.Length;
        }
    }
}