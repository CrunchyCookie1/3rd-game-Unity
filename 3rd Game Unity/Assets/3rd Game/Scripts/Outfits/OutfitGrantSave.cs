using UnityEngine;

public class OutfitGrantSave : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string uniqueID;
    [SerializeField] private GameObject objectToDisable;

    private void Start()
    {
        // Generate ID if empty
        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = gameObject.scene.name + "_" + gameObject.name;

        // Check if already triggered
        if (TriggerSaveManager.IsTriggered(uniqueID))
        {
            if (objectToDisable != null)
                objectToDisable.SetActive(false);
            else
                gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerSaveManager.MarkAsTriggered(uniqueID);

            if (objectToDisable != null)
                objectToDisable.SetActive(false);
            else
                gameObject.SetActive(false);
        }
    }
}