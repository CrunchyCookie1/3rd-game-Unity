using UnityEngine;
using System.Collections.Generic;

public class OutfitsManager : MonoBehaviour
{
    [Header("Outfit Settings")]
    [SerializeField] private string[] allOutfitNames; // Names of all possible outfits
    [SerializeField] private bool[] unlockedOutfits; // Which outfits are unlocked

    private const string SAVE_KEY = "UnlockedOutfits";

    private void Start()
    {
        LoadUnlockedOutfits();
    }

    // Check if a specific outfit is unlocked
    public bool IsOutfitUnlocked(int outfitIndex)
    {
        if (outfitIndex >= 0 && outfitIndex < unlockedOutfits.Length)
            return unlockedOutfits[outfitIndex];
        return false;
    }

    // Get array of all unlocked outfits
    public bool[] GetUnlockedOutfits()
    {
        return unlockedOutfits;
    }

    // Unlock a specific outfit
    public void UnlockOutfit(int outfitIndex)
    {
        if (outfitIndex >= 0 && outfitIndex < unlockedOutfits.Length)
        {
            unlockedOutfits[outfitIndex] = true;
            SaveUnlockedOutfits();
        }
    }

    // Unlock multiple outfits
    public void UnlockOutfits(int[] outfitIndices)
    {
        foreach (int index in outfitIndices)
        {
            if (index >= 0 && index < unlockedOutfits.Length)
                unlockedOutfits[index] = true;
        }
        SaveUnlockedOutfits();
    }

    // Get count of unlocked outfits
    public int GetUnlockedCount()
    {
        int count = 0;
        foreach (bool unlocked in unlockedOutfits)
        {
            if (unlocked) count++;
        }
        return count;
    }

    // Save progress
    private void SaveUnlockedOutfits()
    {
        string saveData = "";
        for (int i = 0; i < unlockedOutfits.Length; i++)
        {
            saveData += (unlockedOutfits[i] ? "1" : "0");
        }
        PlayerPrefs.SetString(SAVE_KEY, saveData);
        PlayerPrefs.Save();
    }

    // Load progress
    private void LoadUnlockedOutfits()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string saveData = PlayerPrefs.GetString(SAVE_KEY);
            for (int i = 0; i < unlockedOutfits.Length && i < saveData.Length; i++)
            {
                unlockedOutfits[i] = (saveData[i] == '1');
            }
        }
        else
        {
            // First time playing - set default unlocks (e.g., first outfit unlocked)
            if (unlockedOutfits.Length > 0)
                unlockedOutfits[0] = true;
        }
    }

    // Reset all unlocks (for testing)
    public void ResetAllUnlocks()
    {
        for (int i = 0; i < unlockedOutfits.Length; i++)
        {
            unlockedOutfits[i] = false;
        }
        SaveUnlockedOutfits();
    }
}