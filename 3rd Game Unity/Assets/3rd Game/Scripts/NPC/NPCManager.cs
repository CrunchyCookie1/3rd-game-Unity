using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using QuestSystem;

public class NPCManager : MonoBehaviour
{
    PlayerControls playerControls;
    QuestProgression questProgression;
    QuestManager2 questManager2;

    [Header("Dialogue Options")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;

    [Header("NPC Interactions")]
    public GameObject interactionZone;
    public GameObject cameraPos;
    public GameObject playerVisualDisabled;
    public GameObject displayDialogue;
    public GameObject interactionPanel;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip dialogueAdvanceSound;
    public bool playSoundOnNewLine = true;

    // Dialogue tracking variables
    private int currentDialogueIndex = 0;
    private int currentSequenceIndex = 0; // Track which sequence we're on
    private bool conversationEnded = false;
    private bool isInConversation = false;

    private bool isPlayerInZone = false;

    // References to player components
    private GameObject player;
    private GameObject playerCamera;
    private InputManager playerInputManager;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private PlayerManager playerManager;
    private Rigidbody playerRigidbody;

    [System.Serializable]
    public class DialogueEntry
    {
        public string speakerName;
        [TextArea(3, 10)]
        public string line;
        public AudioClip lineSound;
    }

    [System.Serializable]
    public class DialogueSequence
    {
        public string sequenceName;
        public DialogueEntry[] dialogueEntries;
    }

    public DialogueSequence[] dialogueSequences;

    [Header("Quest Settings")]
    public string questToGive;
    public string questID;
    public bool giveQuestOnConversationEnd = true;
    public QuestManager2 theQuestManager2;

    public GameObject npcDisable;
    public GameObject npcEnable;

    private void Awake()
    {
        playerControls = new PlayerControls();

        if (dialogueSequences == null || dialogueSequences.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: No dialogue sequences assigned!");
        }

        if (cameraPos != null)
            cameraPos.SetActive(false);

        if (displayDialogue != null)
            displayDialogue.SetActive(false);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
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

    public void SetPlayerInZone(bool inZone, GameObject enteringPlayer = null)
    {
        isPlayerInZone = inZone;

        if (inZone)
        {
            player = enteringPlayer;
            playerControls.Enable();
            interactionPanel.SetActive(true);

            if (player != null)
            {
                playerCamera = player.GetComponentInChildren<Camera>()?.gameObject;
                if (playerCamera == null)
                    playerCamera = Camera.main?.gameObject;

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
            AdvanceDialogue();
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
        // Reset to first sequence
        currentSequenceIndex = 0;
        currentDialogueIndex = 0;

        // Check if we have a valid sequence
        if (!IsValidSequence())
        {
            Debug.LogError("No dialogue entries to display!");
            return;
        }

        isInConversation = true;
        conversationEnded = false;

        interactionPanel.SetActive(false);

        if (playerManager != null)
            playerManager.isInteracting = true;

        displayDialogue.SetActive(true);

        if (playerCamera != null)
            playerCamera.SetActive(false);

        if (playerVisualDisabled != null)
            playerVisualDisabled.SetActive(false);

        if (cameraPos != null)
            cameraPos.SetActive(true);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        if (playerInputManager != null)
        {
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

        if (playerLocomotion != null)
            playerLocomotion.isSprinting = false;

        if (animatorManager != null)
        {
            animatorManager.UpdatedAnimatorValues(0, 0, false);
            Animator animator = animatorManager.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("Vertical", 0);
                animator.SetFloat("Horizontal", 0);
            }
        }

        if (playerInputManager != null)
            playerInputManager.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        DisplayCurrentLine();
        PlayLineSound();

        Debug.Log($"Started conversation with sequence: {dialogueSequences[currentSequenceIndex].sequenceName}");
    }

    private bool IsValidSequence()
    {
        return dialogueSequences != null &&
               dialogueSequences.Length > 0 &&
               currentSequenceIndex < dialogueSequences.Length &&
               dialogueSequences[currentSequenceIndex].dialogueEntries != null &&
               dialogueSequences[currentSequenceIndex].dialogueEntries.Length > 0;
    }

    private void AdvanceDialogue()
    {
        if (conversationEnded) return;

        DialogueEntry[] currentEntries = dialogueSequences[currentSequenceIndex].dialogueEntries;

        // Check if we have more lines in the current sequence
        if (currentDialogueIndex < currentEntries.Length - 1)
        {
            // Move to next line in current sequence
            currentDialogueIndex++;
            DisplayCurrentLine();

            if (audioSource != null)
                audioSource.Stop();

            PlayLineSound();
            Debug.Log($"Advanced to dialogue index {currentDialogueIndex + 1} in sequence {currentSequenceIndex + 1}");
        }
        else
        {
            // We've finished the current sequence, try to move to the next sequence
            if (currentSequenceIndex < dialogueSequences.Length - 1)
            {
                // Move to next sequence
                currentSequenceIndex++;
                currentDialogueIndex = 0;

                // Check if the next sequence is valid
                if (IsValidSequence())
                {
                    DisplayCurrentLine();

                    if (audioSource != null)
                        audioSource.Stop();

                    PlayLineSound();
                    Debug.Log($"Moved to next sequence: {dialogueSequences[currentSequenceIndex].sequenceName}");
                }
                else
                {
                    Debug.Log("Next sequence is invalid, ending conversation");
                    conversationEnded = true;
                    EndConversation();
                }
            }
            else
            {
                // No more sequences, end conversation
                Debug.Log("Reached end of all dialogue sequences, ending conversation");
                conversationEnded = true;
                EndConversation();
            }
        }
    }

    private void DisplayCurrentLine()
    {
        if (!IsValidSequence())
        {
            Debug.LogWarning("No valid dialogue sequence to display!");
            return;
        }

        DialogueEntry[] currentEntries = dialogueSequences[currentSequenceIndex].dialogueEntries;

        if (currentEntries == null || currentDialogueIndex >= currentEntries.Length)
        {
            Debug.LogWarning("No dialogue entry to display!");
            return;
        }

        DialogueEntry currentEntry = currentEntries[currentDialogueIndex];

        if (nameText != null)
        {
            nameText.text = currentEntry.speakerName;
            Debug.Log($"Set name to: {currentEntry.speakerName}");
        }
        else
        {
            Debug.LogError("nameText is not assigned in inspector!");
        }

        if (dialogueText != null)
        {
            dialogueText.text = currentEntry.line;
            Debug.Log($"Set dialogue to: {currentEntry.line}");
        }
        else
        {
            Debug.LogError("dialogueText is not assigned in inspector!");
        }
    }

    private void PlayLineSound()
    {
        if (!playSoundOnNewLine || audioSource == null) return;

        if (!IsValidSequence()) return;

        DialogueEntry[] currentEntries = dialogueSequences[currentSequenceIndex].dialogueEntries;

        if (currentEntries != null && currentDialogueIndex < currentEntries.Length)
        {
            DialogueEntry currentEntry = currentEntries[currentDialogueIndex];

            if (currentEntry.lineSound != null)
            {
                audioSource.PlayOneShot(currentEntry.lineSound);
                Debug.Log($"Playing specific sound for line: {currentEntry.line}");
                return;
            }
        }

        // Play default sound if no specific sound found
        if (dialogueAdvanceSound != null)
        {
            audioSource.PlayOneShot(dialogueAdvanceSound);
        }
    }

    // Helper method to jump to a specific sequence
    public void SetDialogueSequence(int sequenceIndex)
    {
        if (sequenceIndex >= 0 && sequenceIndex < dialogueSequences.Length)
        {
            currentSequenceIndex = sequenceIndex;
            currentDialogueIndex = 0;
            Debug.Log($"Set to dialogue sequence: {dialogueSequences[sequenceIndex].sequenceName}");

            // Update display if in conversation
            if (isInConversation)
            {
                DisplayCurrentLine();
                PlayLineSound();
            }
        }
        else
        {
            Debug.LogWarning($"Invalid sequence index: {sequenceIndex}");
        }
    }

    // Helper method to get current sequence name
    public string GetCurrentSequenceName()
    {
        if (currentSequenceIndex < dialogueSequences.Length)
            return dialogueSequences[currentSequenceIndex].sequenceName;
        return "None";
    }

    private void EndConversation()
    {
        isInConversation = false;

        if (playerManager != null)
            playerManager.isInteracting = false;

        displayDialogue.SetActive(false);

        if (audioSource != null)
            audioSource.Stop();

        interactionPanel.SetActive(true);

        if (playerCamera != null)
            playerCamera.SetActive(true);

        if (playerVisualDisabled != null)
            playerVisualDisabled.SetActive(true);

        if (cameraPos != null)
            cameraPos.SetActive(false);

        if (playerInputManager != null)
            playerInputManager.enabled = true;

        // Give quest if assigned
        if (theQuestManager2 != null && !string.IsNullOrEmpty(questToGive))
        {
            theQuestManager2.questID = questToGive;
            theQuestManager2.AssignQuest();
            Debug.Log($"Quest given: {questToGive}");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (npcEnable != null)
            npcEnable.SetActive(true);

        if (npcDisable != null)
            npcDisable.SetActive(false);

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