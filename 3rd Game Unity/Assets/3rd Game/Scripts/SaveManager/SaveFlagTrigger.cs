using UnityEngine;

public class SaveFlagTrigger : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private bool value = true;

    // Wire this to any plain UnityEvent in the Inspector
    public void Save()
    {
        SaveManager.Instance.SaveFlag(key, value);
    }

    // Optional convenience for wiring two separate events
    public void SaveTrue() => SaveManager.Instance.SaveFlag(key, true);
    public void SaveFalse() => SaveManager.Instance.SaveFlag(key, false);
}