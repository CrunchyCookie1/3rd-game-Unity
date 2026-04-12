using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UiButtons : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Button button;
    [SerializeField] private ButtonActionType actionType;

    [Header("Scene Loading Settings")]
    [SerializeField] private string sceneName = "NextScene";
    [SerializeField] private LoadSceneMode sceneLoadMode = LoadSceneMode.Single;

    [Header("Custom Action (Optional)")]
    [SerializeField] private UnityEvent onButtonPressed;

    [Header("References")]
    [SerializeField] private OpenMenu openMenu;

    private enum ButtonActionType
    {
        ResumeGame,
        QuitGame,
        LoadScene,
        LoadSceneAdditive,
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
                case ButtonActionType.LoadScene:
                    button.onClick.AddListener(LoadScene);
                    break;
                case ButtonActionType.LoadSceneAdditive:
                    button.onClick.AddListener(LoadSceneAdditive);
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

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set! Please assign a scene name in the inspector.");
        }
    }

    private void LoadSceneAdditive()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogError("Scene name is not set! Please assign a scene name in the inspector.");
        }
    }

    // Optional: Async loading with loading screen support
    public void LoadSceneAsync(string sceneToLoad, bool useLoadingScreen = false)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (useLoadingScreen)
            {
                // You can implement loading screen logic here
                // Example: SceneManager.LoadSceneAsync("LoadingScene");
                Debug.Log("Loading screen would appear here");
            }

            SceneManager.LoadSceneAsync(sceneToLoad);
        }
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