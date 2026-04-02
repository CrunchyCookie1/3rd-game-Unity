using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuPanel; // Assign your menu UI panel in the inspector
    [SerializeField] private InputManager inputManager;

    private bool isMenuOpen = false;
    private float originalTimeScale = 1f;

    private void Start()
    {
        // Ensure menu starts closed
        if (gameMenuPanel != null)
            gameMenuPanel.SetActive(false);

        // Find InputManager if not assigned
        if (inputManager == null)
            inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        // Check for GameMenu input
        if (inputManager != null && inputManager.gameMenuInput)
        {
            ToggleMenu();
            // Reset the input flag to prevent multiple toggles
            inputManager.gameMenuInput = false;
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            OpenMenu2();
        }
        else
        {
            CloseMenu();
        }
    }

    private void OpenMenu2()
    {
        // Freeze the game
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // Show menu
        if (gameMenuPanel != null)
            gameMenuPanel.SetActive(true);

        // Unlock and hide cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Optionally disable player input components
        DisablePlayerInput();
    }

    private void CloseMenu()
    {
        // Unfreeze the game
        Time.timeScale = originalTimeScale;

        // Hide menu
        if (gameMenuPanel != null)
            gameMenuPanel.SetActive(false);

        // Lock and hide cursor back for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Re-enable player input
        EnablePlayerInput();
    }

    private void DisablePlayerInput()
    {
        // Disable scripts that control player movement/actions
        if (inputManager != null)
            inputManager.enabled = false;

        PlayerLocomotion playerLocomotion = GetComponent<PlayerLocomotion>();
        if (playerLocomotion != null)
            playerLocomotion.enabled = false;

        AnimatorManager animatorManager = GetComponent<AnimatorManager>();
        if (animatorManager != null)
            animatorManager.enabled = false;
    }

    private void EnablePlayerInput()
    {
        // Re-enable scripts
        if (inputManager != null)
            inputManager.enabled = true;

        PlayerLocomotion playerLocomotion = GetComponent<PlayerLocomotion>();
        if (playerLocomotion != null)
            playerLocomotion.enabled = true;

        AnimatorManager animatorManager = GetComponent<AnimatorManager>();
        if (animatorManager != null)
            animatorManager.enabled = true;
    }

    // Public method to close menu from UI buttons (like "Resume" button)
    public void CloseMenuFromButton()
    {
        if (isMenuOpen)
        {
            isMenuOpen = false;
            CloseMenu();
        }
    }
}