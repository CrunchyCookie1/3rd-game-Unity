using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;


    public Vector2 movementInput;
    private float moveAmount;
    public float verticlalInput;
    public float horizontalInput;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
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
        //HandleJumpingInput();
        //HandleActionInput();
    }

    private void HandleMovementInput()
    {
        verticlalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticlalInput));
        animatorManager.UpdatedAnimatorValues(0, moveAmount);
    }
}
