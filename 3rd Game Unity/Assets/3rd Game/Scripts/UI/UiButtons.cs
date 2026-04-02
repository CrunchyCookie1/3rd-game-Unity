using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiButtons : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Button button;
    [SerializeField] private ButtonActionType actionType;

    [Header("Custom Action (Optional)")]
    [SerializeField] private UnityEvent onButtonPressed;

    [Header("References")]
    [SerializeField] private OpenMenu openMenu;

    private enum ButtonActionType
    {
        ResumeGame,
        QuitGame,
        Custom
    }

    private void Start()
    {
        // Get button component if not assigned
        if (button == null)
            button = GetComponent<Button>();

        // Add listener based on action type
        if (button != null)
        {
            switch (actionType)
            {
                case ButtonActionType.ResumeGame:
                    button.onClick.AddListener(ResumeGame);
                    break;
                case ButtonActionType.QuitGame:
                    button.onClick.AddListener(QuitGame);
                    break;
                case ButtonActionType.Custom:
                    button.onClick.AddListener(() => onButtonPressed?.Invoke());
                    break;
            }
        }
    }

    private void ResumeGame()
    {
        if (openMenu != null)
        {
            openMenu.CloseMenuFromButton();
        }
        else
        {
            Debug.LogWarning("GameMenuManager not found! Can't resume game.");
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // Clean up listeners to prevent memory leaks
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}