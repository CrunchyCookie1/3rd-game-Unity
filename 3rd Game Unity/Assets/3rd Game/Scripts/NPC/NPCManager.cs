using UnityEngine;
using UnityEngine.InputSystem;

public class NPCManager : MonoBehaviour
{
    PlayerControls playerControls;

    public GameObject interactionZone; // Reference to the zone object
    public GameObject cameraPos; // This should be a camera GameObject, not just a position
    public GameObject playerVisualDisabled;

    private bool isPlayerInZone = false;
    private bool isInConversation = false;

    // References to player components
    private GameObject player;
    private GameObject playerCamera;
    private InputManager playerInputManager;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;
    private PlayerManager playerManager; // Add PlayerManager reference
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        playerControls = new PlayerControls();

        // Ensure the NPC camera is disabled at start
        if (cameraPos != null)
        {
            cameraPos.SetActive(false);
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
            Debug.Log("Already in conversation with NPC");
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

        // Set player as interacting to stop movement (uses your existing system)
        if (playerManager != null)
        {
            playerManager.isInteracting = true;
        }

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
            // Note: PlayerLocomotion doesn't have horizontal/verticalMovement variables
            // It uses inputManager values directly
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
        Cursor.visible = true;

        Debug.Log("Started conversation with NPC");
    }

    private void EndConversation()
    {
        isInConversation = false;

        // Reset player interacting state
        if (playerManager != null)
        {
            playerManager.isInteracting = false;
        }

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