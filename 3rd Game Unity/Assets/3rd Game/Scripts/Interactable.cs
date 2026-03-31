using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Interactable : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool requireTriggerZone = true;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private GameObject[] objectsToDisable;
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Tp Settings")]
    public bool allowTp = false;
    [SerializeField] Transform tpLocation;
    [SerializeField] GameObject player;

    [Header("Text")]
    public string interactText;
    public TextMeshProUGUI textObject;
    public GameObject textDisplay;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;

    private bool playerInZone = false;
    private InputManager inputManager;

    private void OnTriggerEnter(Collider other)
    {
        textDisplay.SetActive(true);
        textObject.text = interactText;
        if (!requireTriggerZone) return;

        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            inputManager = other.GetComponent<InputManager>();

            if (showDebugMessages)
                Debug.Log($"Player entered {gameObject.name} trigger zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        textObject.text = "";
        textDisplay.SetActive(false);
        if (!requireTriggerZone) return;

        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            inputManager = null;

            if (showDebugMessages)
                Debug.Log($"Player exited {gameObject.name} trigger zone");
        }
    }

    private void Update()
    {
        // Check for interaction input
        if (CanInteract() && inputManager != null && inputManager.interactInput)
        {
            Interact();
        }
    }

    private bool CanInteract()
    {
        if (requireTriggerZone)
            return playerInZone;
        else
            return true; // Always can interact if no trigger zone required
    }

    private void Interact()
    {
        if (showDebugMessages)
            Debug.Log($"Interacted with {gameObject.name}");
        if (allowTp == true)
        {
            player.transform.position = tpLocation.position;
        }

            // Invoke the Unity Event (you can hook up functions in the Inspector)
            onInteract?.Invoke();

        // Reset the input flag so we don't trigger multiple times
        if (inputManager != null)
            inputManager.interactInput = false;
    }
}