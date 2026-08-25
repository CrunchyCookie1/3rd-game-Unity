using UnityEngine;
using System.Collections;

public class RandomAudio : MonoBehaviour
{
    public AudioClip[] randomAudioClips;
    private AudioSource audioSource;
    private bool isPlayingSequence = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on " + gameObject.name);
        }
    }

    public void PlayRandomAudio()
    {
        if (audioSource == null || randomAudioClips == null || randomAudioClips.Length == 0)
            return;

        // Start the continuous playback sequence
        if (!isPlayingSequence)
        {
            isPlayingSequence = true;
            StartCoroutine(PlayRandomSequence());
        }
    }

    private IEnumerator PlayRandomSequence()
    {
        while (isPlayingSequence)
        {
            // Select and play a random clip
            int randomIndex = Random.Range(0, randomAudioClips.Length);
            AudioClip selectedClip = randomAudioClips[randomIndex];

            if (selectedClip != null)
            {
                audioSource.clip = selectedClip;
                audioSource.Play();

                // Wait for the clip to finish playing
                yield return new WaitForSeconds(selectedClip.length);
            }
            else
            {
                // If clip is null, wait a frame and try again
                yield return null;
            }
        }
    }

    public void StopRandomAudio()
    {
        isPlayingSequence = false;
        if (audioSource != null)
            audioSource.Stop();
    }
}