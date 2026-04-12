using UnityEngine;

public class ProximityAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform playerTransform;

    [Header("Distance Settings")]
    [SerializeField] private float maxDistance = 20f;    // Max distance where sound starts
    [SerializeField] private float minDistance = 2f;     // Min distance where sound is max volume
    [SerializeField] private AnimationCurve volumeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Optional")]
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private float maxVolume = 1f;

    private void Start()
    {
        // Find player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // Setup audio source
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; // Make it 3D sound
            if (playOnAwake)
                audioSource.Play();
        }
    }

    private void Update()
    {
        if (playerTransform == null || audioSource == null) return;

        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Calculate volume based on distance
        float t = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
        float volume = volumeCurve.Evaluate(t) * maxVolume;

        // Apply volume
        audioSource.volume = volume;
    }

    // Visualize the audio range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}