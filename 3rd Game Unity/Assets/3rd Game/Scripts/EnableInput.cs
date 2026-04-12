using UnityEngine;

public class EnableInput : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private bool showCursor = true;
    [SerializeField] private CursorLockMode cursorLockMode = CursorLockMode.None;

    [Header("Input Manager Settings")]
    [SerializeField] private string inputManagerTag = "Player";
    [SerializeField] private bool disablePlayerInputOnMenu = true;

    private void Awake()
    {
        // Set cursor state for menu
        Cursor.visible = showCursor;
        Cursor.lockState = cursorLockMode;

        // Disable player input if needed (for main menu scene)
        if (disablePlayerInputOnMenu)
        {
            DisablePlayerInput();
        }

        Debug.Log($"Menu input enabled - Cursor visible: {showCursor}, Lock state: {cursorLockMode}");
    }

    private void DisablePlayerInput()
    {
        // Find and disable InputManager using FindFirstObjectByType
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            inputManager.DisablePlayerControls();
            Debug.Log("InputManager disabled in menu");
        }

        // Also find by tag as backup
        GameObject playerObject = GameObject.FindGameObjectWithTag(inputManagerTag);
        if (playerObject != null)
        {
            InputManager playerInputManager = playerObject.GetComponent<InputManager>();
            if (playerInputManager != null && playerInputManager != inputManager)
            {
                playerInputManager.DisablePlayerControls();
            }
        }
    }

    // Call this when leaving menu to re-enable player input
    public void EnablePlayerInput()
    {
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            inputManager.EnablePlayerControls();
            Debug.Log("InputManager re-enabled");
        }
    }

    private void OnDestroy()
    {
        // Optional: Re-enable input if needed when menu is destroyed
        // (for scenes that might load without a menu)
        if (disablePlayerInputOnMenu)
        {
            EnablePlayerInput();
        }
    }
}