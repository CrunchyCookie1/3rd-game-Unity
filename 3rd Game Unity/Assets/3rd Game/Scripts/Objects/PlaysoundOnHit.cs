using System.Collections;
using UnityEngine;

public class PlaysoundOnHit : MonoBehaviour
{
    public AudioClip hitSound;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.Stop();
            audioSource.Play();
            audioSource.loop = true;
            Debug.LogWarning("Hit sound played on collision with player.");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeOut(audioSource, 0.5f)); // Fade out over 0.5 seconds
            Debug.LogWarning("Stopped hitting Stop sound on collision with player.");
        }
    }

    public IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.volume = startVolume;
    }
}
