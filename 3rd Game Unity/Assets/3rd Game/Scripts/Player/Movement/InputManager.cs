using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;
    OpenMiniMap openMiniMap;


    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticlalInput;
    public float horizontalInput;

    public bool sprintInput;
    public bool jumpInput;

    public bool interactInput;
    public bool exitInput;
    public bool gameMenuInput;

    public bool miniMapInput;   

    public bool miniMapUnlocked = false; // Flag to track if the mini-map is unlocked

    private bool cameraControlsEnabled = true;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        openMiniMap = GetComponent<OpenMiniMap>();
    }

    public void EnablePlayerControls()
    {
        this.enabled = true;
        Debug.Log("InputManager enabled");
    }

    public void DisablePlayerControls()
    {
        this.enabled = false;

        // Reset input values when disabled
        movementInput = Vector2.zero;
        cameraInput = Vector2.zero;
        openMiniMap.ForceCloseMiniMap(); // Ensure the mini-map is closed when controls are disabled
        miniMapInput = false;
        sprintInput = false;
        jumpInput = false;
        interactInput = false;
        exitInput = false;
        gameMenuInput = false;
        verticlalInput = 0;
        horizontalInput = 0;
        moveAmount = 0;

        Debug.Log("InputManager disabled");
    }

    public void DisableCameraControls()
    {
        cameraControlsEnabled = false;

        // Reset camera input values
        cameraInput = Vector2.zero;
        cameraInputX = 0;
        cameraInputY = 0;

        Debug.Log("Camera controls disabled");
    }

    public void EnableCameraControls()
    {
        cameraControlsEnabled = true;

        Debug.Log("Camera controls enabled");
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            playerControls.PlayerActions.Interact.performed += i => interactInput = true;
            playerControls.PlayerActions.Interact.canceled += i => interactInput = false;

            playerControls.PlayerActions.Exit.performed += i => exitInput = true;
            playerControls.PlayerActions.Exit.canceled += i => exitInput = false;

            playerControls.PlayerActions.GameMenu.performed += i => gameMenuInput = true;
            playerControls.PlayerActions.GameMenu.canceled += i => gameMenuInput = false;

            if (miniMapUnlocked == true)
            {
                playerControls.PlayerActions.MiniMap.performed += i => miniMapInput = true;
                playerControls.PlayerActions.MiniMap.canceled += i => miniMapInput = false;
            }
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintInput();
        HandleJumpingInput();
        //HandleActionInput();
    }

    private void HandleMovementInput()
    {
        verticlalInput = movementInput.y;
        horizontalInput = movementInput.x;

        if (cameraControlsEnabled)
        {
            cameraInputY = cameraInput.y;
            cameraInputX = cameraInput.x;
        }
        else
        {
            // Ensure camera values are zero when disabled
            cameraInputY = 0;
            cameraInputX = 0;
        }

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticlalInput));
        animatorManager.UpdatedAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandleSprintInput()
    {
        if (sprintInput && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerLocomotion.HandleJumping();
        }
    }

    public void UnlockMiniMap()
    {
        miniMapUnlocked = true;
        playerControls.PlayerActions.MiniMap.performed += i => miniMapInput = true;
        playerControls.PlayerActions.MiniMap.canceled += i => miniMapInput = false;
        Debug.Log("Mini-map unlocked and input enabled.");
    }
}