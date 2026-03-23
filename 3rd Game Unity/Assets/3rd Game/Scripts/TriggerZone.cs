using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerZone : MonoBehaviour
{
    ScoreManager scoreManager;
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    InputManager inputManager;

    private bool playerInZone = false;

    public bool callFunctionTrigger = false;
    public string callFunction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            inputManager = other.GetComponent<InputManager>();

            if (inputManager != null)
            {
                playerControls.PlayerActions.Interact.performed += OnInteractPerformed2;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            if (inputManager != null)
            {
                playerControls.PlayerActions.Interact.performed -= OnInteractPerformed2;
                inputManager = null;
            }
        }
    }

    public void OnInteractPerformed2(InputAction.CallbackContext context)
    {
        if (callFunctionTrigger)
        {
            SendMessage(callFunction, SendMessageOptions.DontRequireReceiver);
        }
    }
}