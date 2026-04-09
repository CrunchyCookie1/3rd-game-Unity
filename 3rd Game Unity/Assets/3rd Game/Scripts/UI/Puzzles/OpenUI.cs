using UnityEngine;

public class OpenUI : MonoBehaviour
{
    [SerializeField] private GameObject uiElement; // Assign your UI element in the inspector
    [SerializeField] private InputManager inputManager; // Assign your InputManager reference
    [SerializeField] private MonoBehaviour[] scriptsToDisable; // Add scripts you want to disable (movement, camera, etc.)

    private bool isUIOpen = false;
    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisibility;

    // For mouse look scripts (if you have a separate camera/mouse look script)
    [SerializeField] private MonoBehaviour mouseLookScript;

    // Public method to open the UI (call this from events/other scripts)
    public void OpenUI2()
    {
        if (!isUIOpen)
        {
            OpenUIElement();
        }
    }

    // Public method to close the UI (call this from events/other scripts)
    public void CloseUI()
    {
        if (isUIOpen)
        {
            CloseUIElement();
        }
    }

    // Public method to toggle the UI (call this from events/other scripts if needed)
    public void ToggleUI()
    {
        if (isUIOpen)
        {
            CloseUIElement();
        }
        else
        {
            OpenUIElement();
        }
    }

    private void OpenUIElement()
    {
        if (uiElement == null)
        {
            Debug.LogError("UI Element is not assigned!");
            return;
        }

        isUIOpen = true;

        // Enable the UI element
        uiElement.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;

        // Disable player controls and movement scripts
        if (scriptsToDisable != null)
        {
            foreach (MonoBehaviour script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        // Disable mouse look/camera control if assigned
        if (mouseLookScript != null)
            mouseLookScript.enabled = false;

        // Save current cursor state
        previousCursorLockMode = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;

        // Unlock and show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable the InputManager to block all inputs
        if (inputManager != null)
            inputManager.enabled = false;
    }

    private void CloseUIElement()
    {
        if (uiElement == null)
            return;

        isUIOpen = false;

        // Disable the UI element
        uiElement.SetActive(false);

        // Unpause the game
        Time.timeScale = 1f;

        // Re-enable player controls and movement scripts
        if (scriptsToDisable != null)
        {
            foreach (MonoBehaviour script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = true;
            }
        }

        // Re-enable mouse look/camera control if assigned
        if (mouseLookScript != null)
            mouseLookScript.enabled = true;

        // Restore cursor state
        Cursor.lockState = previousCursorLockMode;
        Cursor.visible = previousCursorVisibility;

        // Re-enable the InputManager
        if (inputManager != null)
            inputManager.enabled = true;
    }

    private void OnDestroy()
    {
        // Ensure game unpauses if this object is destroyed while UI is open
        if (isUIOpen)
        {
            Time.timeScale = 1f;

            // Re-enable cursor if needed
            if (inputManager != null)
                inputManager.enabled = true;

            Cursor.lockState = previousCursorLockMode;
            Cursor.visible = previousCursorVisibility;
        }
    }
}