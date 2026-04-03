using QuestSystem;
using UnityEngine;

public class GiveQuestZone : MonoBehaviour
{

    [Header("Quest Settings")]
    public string questToGive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(questToGive))
            {
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.AcceptQuest(questToGive);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
