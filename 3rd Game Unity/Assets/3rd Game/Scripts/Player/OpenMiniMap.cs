using UnityEngine;
using UnityEngine.Events;

public class OpenMiniMap : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject miniMap; // Make sure to assign this in the Inspector

    [SerializeField] UnityEvent onMiniMapOpened;
    [SerializeField] UnityEvent onMiniMapClosed;

    private bool miniMapActive = false;

    private void Start()
    {
        if (inputManager == null)
            inputManager = GetComponent<InputManager>();

        if (miniMap != null)
            miniMap.SetActive(false);
    }

    private void Update()
    {
        if (inputManager != null && inputManager.miniMapInput)
        {
            ToggleMiniMap();
            inputManager.miniMapInput = false; // Reset input flag
        }
    }

    private void ToggleMiniMap()
    {
        if (miniMap == null)
        {
            Debug.LogError("MiniMap GameObject not assigned!");
            return;
        }

        if (miniMapActive)
            CloseMinimap();
        else
            OpenMinimap();
    }

    private void CloseMinimap()
    {
        if (miniMap != null && miniMapActive)
        {
            miniMapActive = false;
            miniMap.SetActive(false);
            onMiniMapClosed.Invoke();
            Debug.Log("MiniMap closed.");
        }
    }

    private void OpenMinimap()
    {
        if (miniMap != null && !miniMapActive)
        {
            miniMapActive = true;
            miniMap.SetActive(true);
            onMiniMapOpened.Invoke();
            Debug.Log("MiniMap opened.");
        }
    }

    public void ForceCloseMiniMap()
    {
        if (miniMap != null && miniMapActive)
        {
            miniMapActive = false;
            miniMap.SetActive(false);
            onMiniMapClosed.Invoke();
            Debug.Log("MiniMap forcibly closed.");
        }
    }
}