using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeClothes : MonoBehaviour
{
    public UnityEvent outfitsEvent;
    public Button[] changeClothesButtons;

    [Header("References")]
    [SerializeField] private OutfitsManager outfitsManager;
    [SerializeField] private InputManager inputManager; // Drag InputManager reference here

    private void OnEnable()
    {
        // Show cursor for UI interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Disable player input when clothes menu is open
        if (inputManager != null)
        {
            inputManager.DisablePlayerControls();
        }

        // Load outfits
        RefreshOutfitButtons();
    }

    private void OnDisable()
    {
        // Hide cursor when closing clothes menu
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Re-enable player input when clothes menu closes
        if (inputManager != null)
        {
            inputManager.EnablePlayerControls();
        }
    }

    private void Start()
    {
        foreach (Button button in changeClothesButtons)
        {
            button.interactable = false;
        }

        LoadUnlockedOutfits();
    }

    // Load which outfits the player has unlocked
    private void LoadUnlockedOutfits()
    {
        if (outfitsManager == null)
        {
            Debug.LogError("OutfitsManager not assigned in ChangeClothes!");
            return;
        }

        // Enable buttons for unlocked outfits
        for (int i = 0; i < changeClothesButtons.Length && i < outfitsManager.GetUnlockedOutfits().Length; i++)
        {
            if (outfitsManager.IsOutfitUnlocked(i))
            {
                changeClothesButtons[i].interactable = true;
            }
        }
    }

    // Call this when unlocking new outfits
    public void RefreshOutfitButtons()
    {
        // Reset all buttons
        foreach (Button button in changeClothesButtons)
        {
            button.interactable = false;
        }

        // Re-enable unlocked ones
        LoadUnlockedOutfits();

        // Invoke event if needed
        outfitsEvent?.Invoke();
    }

    // Your original save method (kept for compatibility)
    public void saveOutfits()
    {
        outfitsEvent.Invoke();
        RefreshOutfitButtons();
    }

    // Example: Unlock outfit when clicking something
    public void UnlockOutfitByIndex(int outfitIndex)
    {
        if (outfitsManager != null)
        {
            outfitsManager.UnlockOutfit(outfitIndex);
            RefreshOutfitButtons();
        }
    }
}