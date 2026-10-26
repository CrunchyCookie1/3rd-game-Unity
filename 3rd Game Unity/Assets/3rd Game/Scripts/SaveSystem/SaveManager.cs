using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BoolFlag
{
    public string key;
    public bool value;
    public UnityEvent onLoadedTrue;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private List<BoolFlag> flags = new List<BoolFlag>();

    private readonly Dictionary<string, BoolFlag> flagLookup = new Dictionary<string, BoolFlag>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildLookup();
        LoadSaveData();
        FireLoadedEvents();
    }

    private void BuildLookup()
    {
        flagLookup.Clear();
        foreach (var flag in flags)
        {
            if (string.IsNullOrEmpty(flag.key)) continue;
            if (flagLookup.ContainsKey(flag.key))
            {
                Debug.LogWarning($"Duplicate save key: {flag.key}");
                continue;
            }
            flagLookup[flag.key] = flag;
        }
    }

    public bool GetFlag(string key)
    {
        return flagLookup.TryGetValue(key, out var flag) && flag.value;
    }

    public void SaveFlag(string key, bool value)
    {
        if (!flagLookup.TryGetValue(key, out var flag))
        {
            Debug.LogWarning($"Save key not registered: {key}");
            return;
        }

        flag.value = value;
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSaveData()
    {
        foreach (var flag in flags)
        {
            if (string.IsNullOrEmpty(flag.key)) continue;
            flag.value = PlayerPrefs.GetInt(flag.key, flag.value ? 1 : 0) == 1;
        }
    }

    private void FireLoadedEvents()
    {
        foreach (var flag in flags)
        {
            if (flag.value)
                flag.onLoadedTrue?.Invoke();
        }
    }

    [ContextMenu("Clear All Save Data")]
    private void ClearAll()
    {
        foreach (var flag in flags)
        {
            if (string.IsNullOrEmpty(flag.key)) continue;
            PlayerPrefs.DeleteKey(flag.key);
            flag.value = false;
        }
        PlayerPrefs.Save();
    }
}