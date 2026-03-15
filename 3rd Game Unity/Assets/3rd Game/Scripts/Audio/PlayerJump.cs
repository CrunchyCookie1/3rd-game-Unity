using System.Collections;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip jumpSFX;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float jumpVolume = 0.5f;

    [Tooltip("How far to check for ground")]
    public float groundCheckDistance = 0.5f;

    [Tooltip("How much to offset the raycast from the player's feet")]
    public float raycastOffset = 0.2f;

    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;
    private PlayerManager playerManager;

    private LayerMask groundLayer;
    private bool jumpInputPressed = false;

    [Header("Debug (Read Only)")]
    public bool isGrounded;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerManager = GetComponent<PlayerManager>();

        if (inputManager == null || playerLocomotion == null)
        {
            Debug.LogError("Required components not found!");
            return;
        }

        // Set up ground layer - add "Ground" layer if you have one
        groundLayer = LayerMask.GetMask("Default", "Ground", "Terrain");
    }

    void Update()
    {
        // Check current grounded state (always update this regardless of interaction)
        isGrounded = CheckGrounded();

        // Handle jump input detection (always detect input, but only play sound when appropriate)
        DetectJumpInput();
    }

    void DetectJumpInput()
    {
        // Detect when jump is pressed (rising edge detection)
        if (inputManager.jumpInput && !jumpInputPressed)
        {
            jumpInputPressed = true;

            // Check if we should play the jump sound
            // Play sound if we're grounded AND (not interacting OR we want sound during interaction)
            if (isGrounded)
            {
                // Only skip sound if we're interacting AND we don't want sound during interactions
                // You can decide based on your game's needs
                if (playerManager != null && playerManager.isInteracting)
                {
                    // Option 1: Don't play sound during interactions
                    // Debug.Log("Jump during interaction - no sound");

                    // Option 2: Still play sound during interactions (uncomment the line below)
                     PlayJumpSound();
                }
                else
                {
                    // Not interacting, play sound normally
                    PlayJumpSound();
                }
            }
        }

        // Reset when jump is released
        if (!inputManager.jumpInput)
        {
            jumpInputPressed = false;
        }
    }

    bool CheckGrounded()
    {
        // Cast a ray from slightly above the player's feet
        Vector3 origin = transform.position + Vector3.up * raycastOffset;

        // Do the raycast
        RaycastHit hit;
        bool hitGround = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // Draw debug ray
        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, hitGround ? Color.green : Color.red);

        return hitGround;
    }

    void PlayJumpSound()
    {
        if (AudioManager.instance != null && jumpSFX != null)
        {
            AudioManager.instance.PlaySFX(jumpSFX, transform.position, jumpVolume);
            Debug.Log("Jump sound played at: " + Time.time);
        }
    }
}