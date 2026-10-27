using UnityEngine;

public class FirstRunSave : MonoBehaviour
{
    [Tooltip("Marker key written on first run. Must match checkKey in CheckPrefsOnStart.")]
    public string saveKey = "SaveData_Exists";

    [Tooltip("Value stored for the key (only the key's existence matters for HasKey).")]
    public int saveValue = 1;

    void Awake()
    {
        // Only write if the key has never been saved before.
        if (!PlayerPrefs.HasKey(saveKey))
        {
            PlayerPrefs.SetInt(saveKey, saveValue);
            PlayerPrefs.Save(); // flush to disk immediately
            Debug.Log($"FirstRunSave: wrote marker key '{saveKey}'.");
        }
    }
}