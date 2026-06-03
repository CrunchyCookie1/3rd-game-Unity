using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform canvasRectTransform;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    private float padding = 25f;

    private bool previousMouseState;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private Camera mainCamera;

    private string previousControlScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    public bool isCursorEnabled = false; // New flag

    private void OnEnable()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChanged;

        SetCursorActive(false);
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added) InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    public void SetCursorActive(bool active)
    {
        isCursorEnabled = active;

        if (!isCursorEnabled)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
        }
        else if (playerInput.currentControlScheme == gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
        }
    }

    private void UpdateMotion()
    {
        if (!isCursorEnabled || virtualMouse == null || Gamepad.current == null || playerInput.currentControlScheme != gamepadScheme)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        deltaValue *= cursorSpeed * Time.unscaledDeltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool aButtonPressed = Gamepad.current.aButton.IsPressed();
        if (previousMouseState != aButtonPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode
            == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (!isCursorEnabled)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            previousControlScheme = playerInput.currentControlScheme;
            return;
        }

        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }
}