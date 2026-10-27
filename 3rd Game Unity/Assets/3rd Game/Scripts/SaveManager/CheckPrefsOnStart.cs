using UnityEngine;
using UnityEngine.Events;

public class CheckPrefsOnStart : MonoBehaviour
{
    [Tooltip("The key used to mark that save data exists")]
    public string checkKey = "SaveData_Exists";

    [Tooltip("Fired on Start if the save key is found")]
    public UnityEvent onPrefsFound;

    void Start()
    {
        // Check whether the specified key exists
        if (PlayerPrefs.HasKey(checkKey))
        {
            // If it exists, invoke the UnityEvent
            onPrefsFound?.Invoke();
        }
    }
}