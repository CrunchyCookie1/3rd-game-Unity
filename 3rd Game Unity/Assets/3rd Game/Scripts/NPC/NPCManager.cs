using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using QuestSystem;

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
    private int currentSpeakerIndex = 0; // 0 = NPC, 1 = Player
    private bool conversationEnded = false;
    private bool isInConversation = false;
    public int conversationIndex;

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
    public class Dialogue
    {
        public string[] lines;
        public string name;
        public AudioClip[] lineSounds; // Optional: different sound per line
    }

    [System.Serializable]
    public class PlayerDialogue
    {
        public string[] lines;
        public string name;
        public AudioClip[] lineSounds; // Optional: different sound per line
    }

    public Dialogue[] NpcDialogue;
    public PlayerDialogue[] playerDialogue;

    // New structure to organize the conversation sequence
    [System.Serializable]
    public class ConversationSequence
    {
        public SpeakerType speaker;
        public int dialogueIndex; // Which dialogue array to use
        public int lineIndex; // Which line in that dialogue
    }

    public enum SpeakerType
    {
        NPC,
        Player
    }

    // Build this sequence programmatically or define it in the inspector
    private ConversationSequence[] conversationSequence;

    private void Awake()
    {
        playerControls = new PlayerControls();

        // Validate dialogue arrays before building sequence
        if (NpcDialogue == null || NpcDialogue.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: No NPC dialogue assigned! Please assign dialogue in the inspector.");
        }

        if (playerDialogue == null || playerDialogue.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: No player dialogue assigned! Please assign dialogue in the inspector.");
        }

        if (conversationIndex <= 0)
        {
            Debug.LogWarning($"{gameObject.name}: conversationIndex is {conversationIndex}. Setting to default value of 2.");
            conversationIndex = 2; // Set a safe default
        }

        // Rest of your Awake code...
        if (cameraPos != null)
        {
            cameraPos.SetActive(false);
        }

        if (displayDialogue != null)
        {
            displayDialogue.SetActive(false);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        BuildConversationSequence();
    }

    private void BuildConversationSequence()
    {
        // Safety checks to prevent crashes
        if (NpcDialogue == null || NpcDialogue.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: NpcDialogue is null or empty! Cannot build conversation sequence.");
            conversationSequence = new ConversationSequence[0];
            return;
        }

        if (playerDialogue == null || playerDialogue.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: playerDialogue is null or empty! Cannot build conversation sequence.");
            conversationSequence = new ConversationSequence[0];
            return;
        }

        int totalLines = 0;

        // Safe counting with null checks
        for (int i = 0; i < NpcDialogue.Length; i++)
        {
            if (NpcDialogue[i] != null && NpcDialogue[i].lines != null)
                totalLines += NpcDialogue[i].lines.Length;
            else
                Debug.LogWarning($"NpcDialogue[{i}] has null lines array");
        }

        for (int i = 0; i < playerDialogue.Length; i++)
        {
            if (playerDialogue[i] != null && playerDialogue[i].lines != null)
                totalLines += playerDialogue[i].lines.Length;
            else
                Debug.LogWarning($"playerDialogue[{i}] has null lines array");
        }

        if (totalLines == 0)
        {
            Debug.LogError($"{gameObject.name}: No dialogue lines found! Cannot build conversation sequence.");
            conversationSequence = new ConversationSequence[0];
            return;
        }

        conversationSequence = new ConversationSequence[totalLines];
        int sequenceIndex = 0;

        int npcIndex = 0;
        int playerIndex = 0;
        int npcLineInDialogue = 0;
        int playerLineInDialogue = 0;

        int maxIterations = totalLines * 2; // Safety limit to prevent infinite loops
        int iterations = 0;

        // Use conversationIndex with a safe default
        int npcLinesPerTurn = Mathf.Max(1, conversationIndex); // Ensure at least 1 line per turn

        while (sequenceIndex < totalLines && iterations < maxIterations)
        {
            iterations++;

            // Safety check: break if we've exhausted all NPC dialogue
            if (npcIndex >= NpcDialogue.Length || NpcDialogue[npcIndex] == null || NpcDialogue[npcIndex].lines == null)
            {
                Debug.LogWarning("Exhausted NPC dialogue or found null entry");
                break;
            }

            // Add NPC lines
            int npcLinesAdded = 0;
            for (int i = 0; i < npcLinesPerTurn && sequenceIndex < totalLines && npcIndex < NpcDialogue.Length; i++)
            {
                // Additional safety check for current NPC dialogue
                if (NpcDialogue[npcIndex] == null || NpcDialogue[npcIndex].lines == null)
                {
                    Debug.LogError($"NpcDialogue[{npcIndex}] is null or has null lines! Skipping.");
                    npcIndex++;
                    break;
                }

                if (npcLineInDialogue >= NpcDialogue[npcIndex].lines.Length)
                {
                    npcLineInDialogue = 0;
                    npcIndex++;
                    if (npcIndex >= NpcDialogue.Length) break;

                    // Check next NPC dialogue
                    if (NpcDialogue[npcIndex] == null || NpcDialogue[npcIndex].lines == null)
                    {
                        Debug.LogError($"NpcDialogue[{npcIndex}] is null or has null lines after increment! Breaking.");
                        break;
                    }
                }

                if (npcIndex < NpcDialogue.Length && npcLineInDialogue < NpcDialogue[npcIndex].lines.Length)
                {
                    conversationSequence[sequenceIndex] = new ConversationSequence
                    {
                        speaker = SpeakerType.NPC,
                        dialogueIndex = npcIndex,
                        lineIndex = npcLineInDialogue
                    };

                    npcLineInDialogue++;
                    sequenceIndex++;
                    npcLinesAdded++;
                }
                else
                {
                    break;
                }
            }

            // Add player line (only if we added at least one NPC line and haven't reached the end)
            if (sequenceIndex < totalLines && playerIndex < playerDialogue.Length)
            {
                // Safety check for player dialogue
                if (playerDialogue[playerIndex] == null || playerDialogue[playerIndex].lines == null)
                {
                    Debug.LogError($"playerDialogue[{playerIndex}] is null or has null lines! Skipping.");
                    playerIndex++;
                }
                else if (playerLineInDialogue >= playerDialogue[playerIndex].lines.Length)
                {
                    playerLineInDialogue = 0;
                    playerIndex++;
                    if (playerIndex < playerDialogue.Length && (playerDialogue[playerIndex] == null || playerDialogue[playerIndex].lines == null))
                    {
                        Debug.LogError($"playerDialogue[{playerIndex}] is null or has null lines after increment! Breaking.");
                        break;
                    }
                }

                if (playerIndex < playerDialogue.Length &&
                    playerDialogue[playerIndex] != null &&
                    playerDialogue[playerIndex].lines != null &&
                    playerLineInDialogue < playerDialogue[playerIndex].lines.Length)
                {
                    conversationSequence[sequenceIndex] = new ConversationSequence
                    {
                        speaker = SpeakerType.Player,
                        dialogueIndex = playerIndex,
                        lineIndex = playerLineInDialogue
                    };

                    playerLineInDialogue++;
                    sequenceIndex++;
                }
            }

            // Break if we've exhausted both arrays
            if (npcIndex >= NpcDialogue.Length && playerIndex >= playerDialogue.Length)
                break;

            // Prevent infinite loop if no progress was made
            if (npcLinesAdded == 0 && sequenceIndex < totalLines)
            {
                Debug.LogError($"{gameObject.name}: No progress made in conversation building! Breaking to prevent crash.");
                break;
            }
        }

        if (iterations >= maxIterations)
        {
            Debug.LogError($"{gameObject.name}: Maximum iterations reached while building conversation! Check your dialogue configuration.");
        }

        Debug.Log($"Built conversation sequence with {conversationSequence.Length} total lines");
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

            // Find player components
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
        currentSpeakerIndex = 0;
        conversationEnded = false;

        interactionPanel.SetActive(false);

        // Set player as interacting to stop movement
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

            Animator animator = animatorManager.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("Vertical", 0);
                animator.SetFloat("Horizontal", 0);
            }
        }

        // Disable the InputManager script
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

        // Move to next line in sequence
        if (currentSpeakerIndex < conversationSequence.Length - 1)
        {
            currentSpeakerIndex++;
            DisplayCurrentLine();

            audioSource.Stop();
            PlayLineSound();

            Debug.Log($"Advanced to line {currentSpeakerIndex}, Speaker: {conversationSequence[currentSpeakerIndex].speaker}");
        }
        else
        {
            // Reached the end of all dialogues - end conversation
            Debug.Log("Reached end of all dialogues, ending conversation");
            conversationEnded = true;
            EndConversation();
        }
    }

    private void DisplayCurrentLine()
    {
        if (conversationSequence == null || conversationSequence.Length == 0)
        {
            Debug.LogWarning("No conversation sequence built!");
            return;
        }

        ConversationSequence current = conversationSequence[currentSpeakerIndex];

        if (current.speaker == SpeakerType.NPC)
        {
            // Display NPC dialogue
            if (NpcDialogue != null && current.dialogueIndex < NpcDialogue.Length)
            {
                Dialogue npcDialogue = NpcDialogue[current.dialogueIndex];

                if (nameText != null)
                {
                    nameText.text = npcDialogue.name;
                }

                if (dialogueText != null && current.lineIndex < npcDialogue.lines.Length)
                {
                    dialogueText.text = npcDialogue.lines[current.lineIndex];
                }
            }
        }
        else if (current.speaker == SpeakerType.Player)
        {
            // Display player dialogue
            if (playerDialogue != null && current.dialogueIndex < playerDialogue.Length)
            {
                PlayerDialogue playerDialogueObj = playerDialogue[current.dialogueIndex];

                if (nameText != null)
                {
                    nameText.text = playerDialogueObj.name;
                }

                if (dialogueText != null && current.lineIndex < playerDialogueObj.lines.Length)
                {
                    dialogueText.text = playerDialogueObj.lines[current.lineIndex];
                }
            }
        }
    }

    private void PlayLineSound()
    {
        if (!playSoundOnNewLine || audioSource == null) return;

        ConversationSequence current = conversationSequence[currentSpeakerIndex];

        if (current.speaker == SpeakerType.NPC)
        {
            // Play NPC line sound
            if (NpcDialogue != null && current.dialogueIndex < NpcDialogue.Length)
            {
                Dialogue npcDialogue = NpcDialogue[current.dialogueIndex];

                if (npcDialogue.lineSounds != null &&
                    npcDialogue.lineSounds.Length > current.lineIndex &&
                    npcDialogue.lineSounds[current.lineIndex] != null)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(npcDialogue.lineSounds[current.lineIndex]);
                    return;
                }
            }
        }
        else if (current.speaker == SpeakerType.Player)
        {
            // Play player line sound
            if (playerDialogue != null && current.dialogueIndex < playerDialogue.Length)
            {
                PlayerDialogue playerDialogueObj = playerDialogue[current.dialogueIndex];

                if (playerDialogueObj.lineSounds != null &&
                    playerDialogueObj.lineSounds.Length > current.lineIndex &&
                    playerDialogueObj.lineSounds[current.lineIndex] != null)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(playerDialogueObj.lineSounds[current.lineIndex]);
                    return;
                }
            }
        }

        // Play default sound if no specific sound found
        if (dialogueAdvanceSound != null)
        {
            audioSource.PlayOneShot(dialogueAdvanceSound);
        }
    }

    [Header("Quest Settings")]
    public string questToGive;

    private void EndConversation()
    {
        isInConversation = false;

        if (playerManager != null)
        {
            playerManager.isInteracting = false;
        }

        displayDialogue.SetActive(false);

        audioSource.Stop();

        interactionPanel.SetActive(true);

        if (playerCamera != null)
        {
            playerCamera.SetActive(true);
        }

        if (playerVisualDisabled != null)
        {
            playerVisualDisabled.SetActive(true);
        }

        if (cameraPos != null)
        {
            cameraPos.SetActive(false);
        }

        if (playerInputManager != null)
        {
            playerInputManager.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!string.IsNullOrEmpty(questToGive))
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.AcceptQuest(questToGive);
            }
        }

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