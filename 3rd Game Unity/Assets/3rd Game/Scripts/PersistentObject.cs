using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PersistentObject : MonoBehaviour
{
    private static Dictionary<string, PersistentObject> instances = new Dictionary<string, PersistentObject>();

    [Header("Persistent Settings")]
    [SerializeField] private string objectID = ""; // Optional: custom ID for more precise control

    [Header("Reset Settings")]
    [SerializeField] private bool resetPositionOnSceneLoad = true;
    [SerializeField] private Vector3 resetPosition = Vector3.zero;
    [SerializeField] private bool resetRotation = true;
    [SerializeField] private bool resetScale = false;

    private void Awake()
    {
        // Use tag as the key, or custom ID if provided, or name as fallback
        string key = GetKey();

        // Check if an instance with this key already exists
        if (instances.ContainsKey(key))
        {
            // If this is the same object, keep it
            if (instances[key] == this)
            {
                return;
            }

            // Otherwise destroy the duplicate
            Debug.Log($"Destroying duplicate persistent object with tag: {gameObject.tag}, name: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // Register this instance
        instances[key] = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log($"Persistent object registered with key: {key}, tag: {gameObject.tag}, name: {gameObject.name}");
    }

    private string GetKey()
    {
        // Priority: custom ID > tag > name + tag combination
        if (!string.IsNullOrEmpty(objectID))
        {
            return objectID;
        }

        if (!string.IsNullOrEmpty(gameObject.tag) && gameObject.tag != "Untagged")
        {
            return gameObject.tag;
        }

        // Fallback to name + tag combination to avoid conflicts
        return $"{gameObject.name}_{gameObject.tag}";
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (resetPositionOnSceneLoad)
        {
            transform.position = resetPosition;
        }

        if (resetRotation)
        {
            transform.rotation = Quaternion.identity;
        }

        if (resetScale)
        {
            transform.localScale = Vector3.one;
        }

        Debug.Log($"Persistent object (tag: {gameObject.tag}) reset to position: {transform.position} in scene: {scene.name}");
    }

    private void OnDestroy()
    {
        string key = GetKey();

        // Remove from dictionary if this instance is being destroyed
        if (instances.ContainsKey(key) && instances[key] == this)
        {
            instances.Remove(key);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log($"Persistent object (tag: {gameObject.tag}) removed from registry");
        }
    }

    // Optional: Method to manually set the object ID
    public void SetObjectID(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        // Remove old registration
        string oldKey = GetKey();
        if (instances.ContainsKey(oldKey) && instances[oldKey] == this)
        {
            instances.Remove(oldKey);
        }

        // Set new ID and register again
        objectID = id;
        string newKey = GetKey();

        if (!instances.ContainsKey(newKey))
        {
            instances[newKey] = this;
        }
        else
        {
            Debug.LogWarning($"Object ID {id} already exists, will not re-register");
        }
    }
}