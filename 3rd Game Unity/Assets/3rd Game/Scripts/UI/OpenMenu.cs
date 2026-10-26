using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

public class OpenMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject tutorialTip;
    public UnityEvent onMenuOpen;
    public UnityEvent onMenuClose;

    private bool isMenuOpen = false;
    private float originalTimeScale = 1f;

    public GameObject mainCanvas;
    public Canvas[] allCanvases;

    private void Start()
    {
        allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        if (gameMenuPanel != null)
            gameMenuPanel.SetActive(false);

        if (inputManager == null)
            inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (inputManager != null && inputManager.gameMenuInput)
        {
            ToggleMenu();
            inputManager.gameMenuInput = false;
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
            OpenMenu2();
        else
            CloseMenu();
    }

    private void OpenMenu2()
    {
        originalTimeScale = Time.timeScale;
        onMenuOpen?.Invoke();
        Time.timeScale = 0f;

        tutorialTip.SetActive(false);
        if (gameMenuPanel != null)
            gameMenuPanel.SetActive(true);

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject != mainCanvas)
                canvas.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisablePlayerInput();
    }

    private void CloseMenu()
    {
        Time.timeScale = originalTimeScale;
        onMenuClose?.Invoke();

        if (gameMenuPanel != null)
            gameMenuPanel.SetActive(false);

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject != mainCanvas)
                canvas.gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EnablePlayerInput();
    }

    private void DisablePlayerInput()
    {
        if (inputManager != null)
        {
            inputManager.DisableCameraControls();
            inputManager.BlockActionInputs();
        }

        PlayerLocomotion playerLocomotion = GetComponent<PlayerLocomotion>();
        if (playerLocomotion != null)
            playerLocomotion.enabled = false;

        AnimatorManager animatorManager = GetComponent<AnimatorManager>();
        if (animatorManager != null)
            animatorManager.enabled = false;

        CameraManager cameraManager = GetComponentInChildren<CameraManager>(true);
        if (cameraManager != null)
            cameraManager.enabled = false;
    }

    private void EnablePlayerInput()
    {
        if (inputManager != null)
        {
            inputManager.EnableCameraControls();
            inputManager.UnblockActionInputs();
        }

        PlayerLocomotion playerLocomotion = GetComponent<PlayerLocomotion>();
        if (playerLocomotion != null)
            playerLocomotion.enabled = true;

        AnimatorManager animatorManager = GetComponent<AnimatorManager>();
        if (animatorManager != null)
            animatorManager.enabled = true;

        CameraManager cameraManager = GetComponentInChildren<CameraManager>(true);
        if (cameraManager != null)
            cameraManager.enabled = true;
    }

    public void CloseMenuFromButton()
    {
        if (isMenuOpen)
        {
            isMenuOpen = false;
            CloseMenu();
        }
    }
}