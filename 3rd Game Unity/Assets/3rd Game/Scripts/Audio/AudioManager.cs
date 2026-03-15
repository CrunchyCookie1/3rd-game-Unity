using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void PlaySFX(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        StartCoroutine(PlaySFXCoroutine(audioClip, position, volume));
    }

    IEnumerator PlaySFXCoroutine(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioClip.length * 2);

        Destroy(audioSource);
    }
}
