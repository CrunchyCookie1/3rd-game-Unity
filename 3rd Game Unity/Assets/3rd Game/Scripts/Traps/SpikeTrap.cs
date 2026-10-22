using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public AudioClip[] audioClips;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Called by the Animation Event. The int is the index into audioClips.
    public void PlaySoundByIndex(int clipIndex)
    {
        if (audioClips == null || audioClips.Length == 0) return;
        if (clipIndex < 0 || clipIndex >= audioClips.Length)
        {
            Debug.LogWarning($"SpikeTrap: clipIndex {clipIndex} is out of range (array length {audioClips.Length}).");
            return;
        }

        audioSource.PlayOneShot(audioClips[clipIndex]);
    }
}