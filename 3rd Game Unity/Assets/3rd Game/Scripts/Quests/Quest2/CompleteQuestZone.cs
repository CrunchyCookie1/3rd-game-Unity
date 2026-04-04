using UnityEngine;
using QuestSystem;

public class QuestCompleteZone : MonoBehaviour
{
    public string questID;
    public QuestManager2 questManager2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(questID))
            {
                Debug.LogWarning("No quest ID assigned to QuestCompleteZone!");
                return;
            }

            if (QuestProgression.Instance == null)
            {
                Debug.LogError("QuestProgression.Instance not found!");
                return;
            }

            QuestProgression.Instance.CompleteQuest(questID);

            if (QuestManager2.Instance != null)
            {
                questManager2.CompleteQuestText();
            }
            questManager2.CompleteQuestText();

            Debug.Log($"Quest {questID} completed!");

            gameObject.SetActive(false);
        }
    }
}