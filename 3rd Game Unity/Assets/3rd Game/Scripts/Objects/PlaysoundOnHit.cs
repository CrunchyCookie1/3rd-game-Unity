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
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
            Debug.Log("Hit sound played on collision with player.");
        }
    }
}
