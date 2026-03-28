using UnityEngine;
using UnityEngine.Events;

public class EnterBuildingZone : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private bool useCustomPosition = false;
    [SerializeField] private Vector3 teleportPosition = Vector3.zero;
    [SerializeField] private bool useSpawnPointObject = false;
    [SerializeField] private string spawnPointTag = "SpawnPoint";
    [SerializeField] private bool preservePlayerRotation = false;
    [SerializeField] private Vector3 customRotation = Vector3.zero;
    [SerializeField] private bool teleportToSpecificTag = false;
    [SerializeField] private string targetTag = "TeleportDestination";

    [Header("Cleanup Settings")]
    [SerializeField] private bool cleanupAfterTeleport = true;
    [SerializeField] private bool unloadOldArea = true;
    [SerializeField] private bool loadNewArea = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private string promptText = "Press E to teleport";
    [SerializeField] private float promptShowDelay = 0f;

    [Header("Interaction Settings")]
    [SerializeField] private bool requireButtonHold = false;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private bool canInteractMultipleTimes = true;
    [SerializeField] private bool destroyAfterInteraction = false;
    [SerializeField] private float teleportDelay = 0f;
    [SerializeField] private bool disableDuringTeleport = false;

    [Header("Effects")]
    [SerializeField] private ParticleSystem teleportEffect;
    [SerializeField] private AudioClip teleportSound;

    [Header("Events")]
    public UnityEvent onInteractionStart;
    public UnityEvent onTeleportStart;
    public UnityEvent onTeleportComplete;
    public UnityEvent onPlayerEnterZone;
    public UnityEvent onPlayerExitZone;

    // Private variables
    private bool isPlayerInZone = false;
    private bool hasInteracted = false;
    private float holdTimer = 0f;
    private bool isInteracting = false;
    private bool isTeleporting = false;
    private InputManager inputManager;
    private GameObject player;
    private AudioSource audioSource;
    private Vector3 oldPosition;

    private UnityEngine.UI.Text promptUIText;
    private UnityEngine.UI.Image holdProgressImage;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inputManager = player.GetComponent<InputManager>();
        }

        if (teleportSound != null && audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (interactionPrompt != null)
        {
            promptUIText = interactionPrompt.GetComponentInChildren<UnityEngine.UI.Text>();
            holdProgressImage = interactionPrompt.GetComponentInChildren<UnityEngine.UI.Image>();
            UpdatePromptUI();
        }
    }

    private void Update()
    {
        if (!isPlayerInZone || !inputManager || isTeleporting) return;

        if (requireButtonHold)
        {
            HandleHoldInteraction();
        }
        else
        {
            HandleInstantInteraction();
        }
    }

    private void HandleInstantInteraction()
    {
        if (inputManager.interactInput && !hasInteracted)
        {
            StartTeleport();
        }
    }

    private void HandleHoldInteraction()
    {
        if (inputManager.interactInput)
        {
            if (!isInteracting)
            {
                isInteracting = true;
                holdTimer = 0f;
                onInteractionStart?.Invoke();
            }

            holdTimer += Time.deltaTime;
            UpdateHoldProgress(holdTimer / holdDuration);

            if (holdTimer >= holdDuration && !hasInteracted)
            {
                StartTeleport();
            }
        }
        else if (isInteracting)
        {
            isInteracting = false;
            holdTimer = 0f;
            UpdateHoldProgress(0f);
        }
    }

    private void StartTeleport()
    {
        if (hasInteracted && !canInteractMultipleTimes) return;
        if (isTeleporting) return;

        hasInteracted = true;
        oldPosition = player.transform.position;
        onTeleportStart?.Invoke();

        PlayTeleportEffects();

        if (teleportDelay > 0)
        {
            Invoke(nameof(ExecuteTeleport), teleportDelay);
        }
        else
        {
            ExecuteTeleport();
        }
    }

    private void ExecuteTeleport()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found!");
                return;
            }
        }

        isTeleporting = true;

        if (disableDuringTeleport)
        {
            DisablePlayerMovement(true);
        }

        Transform destination = GetTeleportDestination();
        Vector3 newPosition;

        if (destination != null)
        {
            newPosition = destination.position;
            player.transform.position = newPosition;

            if (!preservePlayerRotation)
            {
                player.transform.rotation = customRotation != Vector3.zero ?
                    Quaternion.Euler(customRotation) : destination.rotation;
            }

            Debug.Log($"Player teleported to: {newPosition}");
        }
        else if (useCustomPosition)
        {
            newPosition = teleportPosition;
            player.transform.position = newPosition;

            if (!preservePlayerRotation)
            {
                player.transform.rotation = customRotation != Vector3.zero ?
                    Quaternion.Euler(customRotation) : Quaternion.identity;
            }

            Debug.Log($"Player teleported to custom position: {newPosition}");
        }
        else
        {
            Debug.LogError("No teleport destination set!");
            return;
        }

        // Reset velocity
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Trigger cleanup
        if (cleanupAfterTeleport)
        {
            if (unloadOldArea)
            {
                // Unload the old area
                TeleportCleanupManager.OnPlayerTeleported(newPosition);
            }

            if (loadNewArea)
            {
                // Force immediate cleanup to load new area
                TeleportCleanupManager.ForceCleanup();
            }
        }

        onTeleportComplete?.Invoke();

        if (disableDuringTeleport)
        {
            Invoke(nameof(EnablePlayerMovement), 0.1f);
        }

        isTeleporting = false;

        if (canInteractMultipleTimes)
        {
            hasInteracted = false;
        }

        if (destroyAfterInteraction)
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            Destroy(gameObject, 0.1f);
        }
    }

    private Transform GetTeleportDestination()
    {
        if (teleportDestination != null)
            return teleportDestination;

        if (teleportToSpecificTag && !string.IsNullOrEmpty(targetTag))
        {
            GameObject destObject = GameObject.FindGameObjectWithTag(targetTag);
            if (destObject != null)
                return destObject.transform;
        }

        if (useSpawnPointObject && !string.IsNullOrEmpty(spawnPointTag))
        {
            GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
            if (spawnPoint != null)
                return spawnPoint.transform;
        }

        return null;
    }

    private void PlayTeleportEffects()
    {
        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, player.transform.position, Quaternion.identity);
        }

        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }
    }

    private void DisablePlayerMovement(bool disable)
    {
        if (player == null) return;

        PlayerLocomotion locomotion = player.GetComponent<PlayerLocomotion>();
        if (locomotion != null)
            locomotion.enabled = !disable;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = !disable;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = disable;
    }

    private void EnablePlayerMovement()
    {
        DisablePlayerMovement(false);
    }

    private void UpdateHoldProgress(float progress)
    {
        if (holdProgressImage != null)
            holdProgressImage.fillAmount = progress;
    }

    private void UpdatePromptUI()
    {
        if (promptUIText != null)
        {
            if (requireButtonHold)
                promptUIText.text = $"{promptText} (Hold)";
            else
                promptUIText.text = promptText;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            onPlayerEnterZone?.Invoke();

            if (canInteractMultipleTimes)
            {
                hasInteracted = false;
            }

            if (interactionPrompt != null)
            {
                if (promptShowDelay > 0)
                    Invoke(nameof(ShowPrompt), promptShowDelay);
                else
                    ShowPrompt();
            }
        }
    }

    private void ShowPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            isInteracting = false;
            holdTimer = 0f;
            onPlayerExitZone?.Invoke();

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            if (promptShowDelay > 0)
                CancelInvoke(nameof(ShowPrompt));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (teleportDestination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(teleportDestination.position, 0.5f);
            Gizmos.DrawLine(transform.position, teleportDestination.position);
        }
        else if (useCustomPosition)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(teleportPosition, 0.5f);
        }
    }
}