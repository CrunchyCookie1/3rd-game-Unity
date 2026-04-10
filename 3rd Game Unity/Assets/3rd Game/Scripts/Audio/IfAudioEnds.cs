using UnityEngine;
using UnityEngine.Events;

public class IfAudioEnds : MonoBehaviour
{
    public AudioSource audioSource;
    private bool hasTriggeredEnd = false;

    public UnityEvent onAudioEnd;

    void Update()
    {
        if (audioSource != null && !audioSource.isPlaying && !hasTriggeredEnd)
        {
            hasTriggeredEnd = true;
            Debug.Log("Audio has finished playing!");
            onAudioEnd?.Invoke();
        }

        // Reset trigger if audio starts playing again
        if (audioSource != null && audioSource.isPlaying)
        {
            hasTriggeredEnd = false;
        }
    }
}