using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NPCManager : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("Dialogue Options")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;

    [Header("NPC Interactions")]
    public GameObject interactionZone; // Reference to the zone object
    public GameObject cameraPos; // This should be a camera GameObject, not just a position
    public GameObject playerVisualDisabled;
    public GameObject displayDialogue;
    public GameObject interactionPanel;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to an AudioSource component
    public AudioClip dialogueAdvanceSound; // Sound to play when advancing to new line
    public bool playSoundOnNewLine = true; // Toggle to enable/disable sound

    // Dialogue tracking variables
    private int currentDialogueIndex = 0;
    private int currentLineIndex = 0;
    private bool conversationEnded = false;

    private bool isPlayerInZone = false;
    private bool isInConversation = false;

    // References to player components
    private GameObject player;
    private GameObject playerCamera;
    private InputManager playerInputManager;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private PlayerManager playerManager;
    private Rigidbody playerRigidbody;

    [System.Serializable]
    public class Dialogue
    {
        public string[] lines;
        public string name;
        public AudioClip[] lineSounds; // Optional: different sound per line
    }

    public Dialogue[] myDataArray;

    private void Awake()
    {
        playerControls = new PlayerControls();

        // Ensure the NPC camera is disabled at start
        if (cameraPos != null)
        {
            cameraPos.SetActive(false);
        }

        // Ensure display dialogue is disabled at start
        if (displayDialogue != null)
        {
            displayDialogue.SetActive(false);
        }

        // Try to get or add AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.PlayerActions.Interact.performed += OnInteractPerformed;
            playerControls.PlayerActions.Exit.performed += OnExitPerformed;
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.PlayerActions.Interact.performed -= OnInteractPerformed;
            playerControls.PlayerActions.Exit.performed -= OnExitPerformed;
        }
    }

    // Public method to be called by the InteractionZone script
    public void SetPlayerInZone(bool inZone, GameObject enteringPlayer = null)
    {
        isPlayerInZone = inZone;

        if (inZone)
        {
            player = enteringPlayer;
            playerControls.Enable();
            interactionPanel.SetActive(true);

            // Find player components
            if (player != null)
            {
                // Find player camera (usually the main camera or a child camera)
                playerCamera = player.GetComponentInChildren<Camera>()?.gameObject;
                if (playerCamera == null)
                    playerCamera = Camera.main?.gameObject;

                // Get all player scripts
                playerInputManager = player.GetComponent<InputManager>();
                playerLocomotion = player.GetComponent<PlayerLocomotion>();
                animatorManager = player.GetComponent<AnimatorManager>();
                playerManager = player.GetComponent<PlayerManager>();
                playerRigidbody = player.GetComponent<Rigidbody>();
            }
        }
        else if (!isInConversation)
        {
            interactionPanel.SetActive(false);
            playerControls.Disable();
            player = null;
            playerCamera = null;
            playerInputManager = null;
            playerLocomotion = null;
            animatorManager = null;
            playerManager = null;
            playerRigidbody = null;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Interact key pressed");
        if (isPlayerInZone && !isInConversation)
        {
            StartConversation();
        }
        else if (isInConversation)
        {
            // If we're in conversation and press interact, advance dialogue
            AdvanceDialogue();
        }
        else
        {
            Debug.Log("Interact pressed but player is not in interaction zone");
        }
    }

    private void OnExitPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Exit key pressed");
        if (isInConversation)
        {
            EndConversation();
        }
    }

    private void StartConversation()
    {
        isInConversation = true;

        // Reset dialogue indices
        currentDialogueIndex = 0;
        currentLineIndex = 0;
        conversationEnded = false;

        interactionPanel.SetActive(false);

        // Set player as interacting to stop movement (uses your existing system)
        if (playerManager != null)
        {
            playerManager.isInteracting = true;
        }

        displayDialogue.SetActive(true);

        // Disable player camera
        if (playerCamera != null)
        {
            playerCamera.SetActive(false);
        }

        // Disable player visuals
        if (playerVisualDisabled != null)
        {
            playerVisualDisabled.SetActive(false);
        }

        // Enable NPC conversation camera
        if (cameraPos != null)
        {
            cameraPos.SetActive(true);
        }

        // Completely stop all movement
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        // Reset all input values in InputManager
        if (playerInputManager != null)
        {
            // Reset all input values
            playerInputManager.movementInput = Vector2.zero;
            playerInputManager.cameraInput = Vector2.zero;
            playerInputManager.horizontalInput = 0;
            playerInputManager.verticlalInput = 0;
            playerInputManager.moveAmount = 0;
            playerInputManager.sprintInput = false;
            playerInputManager.jumpInput = false;
            playerInputManager.cameraInputX = 0;
            playerInputManager.cameraInputY = 0;
        }

        // Reset locomotion values
        if (playerLocomotion != null)
        {
            playerLocomotion.isSprinting = false;
            playerLocomotion.isJumping = false;
        }

        // Reset animations to idle
        if (animatorManager != null)
        {
            animatorManager.UpdatedAnimatorValues(0, 0, false);

            // Force idle animation
            Animator animator = animatorManager.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("Vertical", 0);
                animator.SetFloat("Horizontal", 0);
            }
        }

        // Disable the InputManager script LAST after resetting all values
        if (playerInputManager != null)
        {
            playerInputManager.enabled = false;
        }

        // Unlock and show cursor for conversation
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        // Display the first line of dialogue
        DisplayCurrentLine();

        // Play sound for first line
        PlayLineSound();

        Debug.Log("Started conversation with NPC");
    }

    private void AdvanceDialogue()
    {
        if (conversationEnded) return;

        // Check if we have more lines in the current dialogue
        if (currentLineIndex < myDataArray[currentDialogueIndex].lines.Length - 1)
        {
            // Move to next line in current dialogue
            currentLineIndex++;
            DisplayCurrentLine();

            audioSource.Stop();
            // Play sound when advancing to new line
            PlayLineSound();

            Debug.Log("Advanced to next line: " + currentLineIndex);
        }
        else
        {
            // Check if we have more dialogues in the array
            if (currentDialogueIndex < myDataArray.Length - 1)
            {
                // Move to next dialogue
                currentDialogueIndex++;
                currentLineIndex = 0;
                DisplayCurrentLine();

                // Play sound for first line of new dialogue
                PlayLineSound();

                Debug.Log("Moved to next dialogue: " + currentDialogueIndex);
            }
            else
            {
                // Reached the end of all dialogues - end conversation
                Debug.Log("Reached end of all dialogues, ending conversation");
                conversationEnded = true;
                EndConversation();
            }
        }
    }

    private void DisplayCurrentLine()
    {
        // Make sure we have valid data
        if (myDataArray == null || myDataArray.Length == 0)
        {
            Debug.LogWarning("No dialogue data assigned!");
            return;
        }

        // Display the current line
        Dialogue currentDialogue = myDataArray[currentDialogueIndex];

        // Set name text
        if (nameText != null)
        {
            nameText.text = currentDialogue.name;
        }

        // Set dialogue text
        if (dialogueText != null && currentLineIndex < currentDialogue.lines.Length)
        {
            dialogueText.text = currentDialogue.lines[currentLineIndex];
        }
    }

    private void PlayLineSound()
    {
        if (!playSoundOnNewLine || audioSource == null) return;

        // Check if we have line-specific sounds
        Dialogue currentDialogue = myDataArray[currentDialogueIndex];

        if (currentDialogue.lineSounds != null &&
            currentDialogue.lineSounds.Length > currentLineIndex &&
            currentDialogue.lineSounds[currentLineIndex] != null)
        {
            audioSource.Stop();
            // Play line-specific sound
            audioSource.PlayOneShot(currentDialogue.lineSounds[currentLineIndex]);
        }
        else if (dialogueAdvanceSound != null)
        {
            // Play default advance sound
            audioSource.PlayOneShot(dialogueAdvanceSound);
        }
    }

    private void EndConversation()
    {
        isInConversation = false;

        // Reset player interacting state
        if (playerManager != null)
        {
            playerManager.isInteracting = false;
        }

        displayDialogue.SetActive(false);

        audioSource.Stop();

        interactionPanel.SetActive(true);

        // Enable player camera
        if (playerCamera != null)
        {
            playerCamera.SetActive(true);
        }

        // Enable player visuals
        if (playerVisualDisabled != null)
        {
            playerVisualDisabled.SetActive(true);
        }

        // Disable NPC conversation camera
        if (cameraPos != null)
        {
            cameraPos.SetActive(false);
        }

        // Re-enable player input
        if (playerInputManager != null)
        {
            playerInputManager.enabled = true;
        }

        // Return cursor to game state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Ended conversation with NPC");
    }

    private void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.PlayerActions.Interact.performed -= OnInteractPerformed;
            playerControls.PlayerActions.Exit.performed -= OnExitPerformed;
            playerControls.Disable();
        }
    }
}