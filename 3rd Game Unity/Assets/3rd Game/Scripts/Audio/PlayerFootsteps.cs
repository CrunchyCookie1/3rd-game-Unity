using System.Collections;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip footstepSFX;
    public float walkInterval = 0.5f;
    public float sprintInterval = 0.3f;

    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;
    private PlayerManager playerManager;

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

        StartCoroutine(PlayFootSteps());
    }

    IEnumerator PlayFootSteps()
    {
        while (true)
        {
            // Check if player is moving and grounded
            if (inputManager.moveAmount > 0.1f && playerLocomotion.isGrounded && !playerManager.isInteracting)
            {
                // Determine interval based on movement type
                float interval = playerLocomotion.isSprinting ? sprintInterval : walkInterval;

                // Play footstep sound
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(footstepSFX, transform.position, 0.5f);
                }

                yield return new WaitForSeconds(interval);
            }
            else
            {
                // Wait a short time before checking again to save performance
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}