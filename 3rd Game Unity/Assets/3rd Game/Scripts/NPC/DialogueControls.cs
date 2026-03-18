using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DialogueControls : MonoBehaviour
{
    [System.Serializable]
    public struct IconSet
    {
        public string schemeName; // "Keyboard&Mouse", "Gamepad", "PS4", etc.
        public Texture interactIcon;  // Changed from Sprite to Texture
        public Texture exitIcon;      // Changed from Sprite to Texture
        public Texture jumpIcon;      // Changed from Sprite to Texture
        public Texture attackIcon;    // Changed from Sprite to Texture
        // Add more as needed
    }

    public IconSet[] iconSets;
    public RawImage interactImage;
    public RawImage exitImage;
    public RawImage jumpImage;
    public RawImage attackImage;

    // Called by PlayerInput's "Controls Changed" event
    public void OnControlsChanged(PlayerInput input)
    {
        string currentScheme = input.currentControlScheme;
        Debug.Log($"Switched to: {currentScheme}");

        // Find matching icon set
        foreach (var set in iconSets)
        {
            if (string.Equals(set.schemeName, currentScheme,
                System.StringComparison.CurrentCultureIgnoreCase))
            {
                ApplyIconSet(set);
                break;
            }
        }
    }

    private void ApplyIconSet(IconSet set)
    {
        // RawImage uses .texture property, not .sprite
        if (interactImage != null) interactImage.texture = set.interactIcon;
        if (exitImage != null) exitImage.texture = set.exitIcon;
        if (jumpImage != null) jumpImage.texture = set.jumpIcon;
        if (attackImage != null) attackImage.texture = set.attackIcon;
    }
}