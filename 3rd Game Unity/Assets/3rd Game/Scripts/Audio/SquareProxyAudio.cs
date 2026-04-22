using UnityEngine;

public class SquareProxyAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.UnPause();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
}
