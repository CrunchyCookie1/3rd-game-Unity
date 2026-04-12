using UnityEngine;
using System.Collections.Generic;

public class TriggerSaveManager : MonoBehaviour
{
    private static TriggerSaveManager instance;
    private HashSet<string> triggeredObjects = new HashSet<string>();

    private const string SAVE_KEY = "TriggeredObjectsSave";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTriggeredObjects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void MarkAsTriggered(string objectID)
    {
        if (instance != null && !instance.triggeredObjects.Contains(objectID))
        {
            instance.triggeredObjects.Add(objectID);
            instance.SaveTriggeredObjects();
        }
    }

    public static bool IsTriggered(string objectID)
    {
        return instance != null && instance.triggeredObjects.Contains(objectID);
    }

    private void SaveTriggeredObjects()
    {
        string saveData = string.Join(",", triggeredObjects);
        PlayerPrefs.SetString(SAVE_KEY, saveData);
        PlayerPrefs.Save();
    }

    private void LoadTriggeredObjects()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string saveData = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(saveData))
            {
                string[] ids = saveData.Split(',');
                foreach (string id in ids)
                {
                    triggeredObjects.Add(id);
                }
            }
        }
    }

    public static void ClearAllTriggers()
    {
        if (instance != null)
        {
            instance.triggeredObjects.Clear();
            PlayerPrefs.DeleteKey(SAVE_KEY);
        }
    }
}