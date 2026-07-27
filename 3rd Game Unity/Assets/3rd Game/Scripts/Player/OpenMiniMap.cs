using UnityEngine;

public class OpenMiniMap : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject miniMap; // Make sure to assign this in the Inspector

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

        miniMapActive = !miniMapActive;
        miniMap.SetActive(miniMapActive);

        Debug.Log($"MiniMap toggled: {(miniMapActive ? "ON" : "OFF")}");
    }

    public void ForceCloseMiniMap()
    {
        if (miniMap != null && miniMapActive)
        {
            miniMapActive = false;
            miniMap.SetActive(false);
            Debug.Log("MiniMap forcibly closed.");
        }
    }
}