using QuestSystem;
using UnityEngine;

public class GiveQuestZone : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questToGive;
    public QuestManager2 questManager2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(questToGive))
            {
                Debug.LogWarning("No quest ID assigned to GiveQuestZone!");
                return;
            }

            if (QuestProgression.Instance == null)
            {
                Debug.LogError("QuestProgression.Instance not found!");
                return;
            }

            if (QuestProgression.Instance.HasQuest(questToGive))
            {
                Debug.Log($"Player already has quest: {questToGive}");
                gameObject.SetActive(false);
                return;
            }

            if (QuestProgression.Instance.IsQuestCompleted(questToGive))
            {
                Debug.Log($"Quest already completed: {questToGive}");
                gameObject.SetActive(false);
                return;
            }

            // Check if QuestManager2 is assigned
            if (questManager2 != null)
            {
                // Set the quest ID and assign the quest
                questManager2.questID = questToGive;
                questManager2.AssignQuest();
                QuestProgression.Instance.GiveQuest(questToGive);
                Debug.Log($"Quest given: {questToGive}");
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("QuestManager2 is not assigned in the inspector!");
            }
        }
    }
}