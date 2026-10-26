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

    [Header("Save")]
    [SerializeField] private string saveKey;
    [SerializeField] private bool saveValue = true;

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
            return true;
    }

    private void Interact()
    {
        if (showDebugMessages)
            Debug.Log($"Interacted with {gameObject.name}");

        if (allowTp == true)
        {
            player.transform.position = tpLocation.position;
        }

        if (SaveManager.Instance != null && !string.IsNullOrEmpty(saveKey))
            SaveManager.Instance.SaveFlag(saveKey, saveValue);

        onInteract?.Invoke();

        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (inputManager != null)
            inputManager.interactInput = false;
    }
}