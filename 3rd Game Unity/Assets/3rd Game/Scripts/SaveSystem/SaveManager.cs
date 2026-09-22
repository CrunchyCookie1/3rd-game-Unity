using UnityEngine;
using UnityEngine.Events;


public class SaveManager : MonoBehaviour
{
    public UnityEvent onSave;
    private void Awake()
    {
        bool miniMapUnlocked = PlayerPrefs.GetInt("MiniMapUnlocked", 0) == 1;
        InputManager inputManager = Object.FindFirstObjectByType<InputManager>();
    }
}
