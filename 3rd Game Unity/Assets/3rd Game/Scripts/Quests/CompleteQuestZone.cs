using UnityEngine;
using QuestSystem;

public class QuestCompleteZone : MonoBehaviour
{
    public string questID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.ForceCompleteQuest(questID);
                Debug.Log($"Quest {questID} completed!");
            }
        }
    }
}