using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;

    public void ToggleObjects()
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}