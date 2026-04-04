using UnityEngine;

public class QuestProgression : MonoBehaviour
{
    public static QuestProgression Instance { get; private set; }

    [System.Serializable]
    public class QuestProgress
    {
        public string questID;
        public bool hasQuest = false;
        public bool isCompleted = false;
    }

    public QuestProgress[] QuestsProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanTakeQuest(string questID)
    {
        foreach (var quest in QuestsProgress)
        {
            if (quest.questID == questID)
            {
                return !quest.hasQuest && !quest.isCompleted;
            }
        }
        return true;
    }

    public bool IsQuestCompleted(string questID)
    {
        foreach (var quest in QuestsProgress)
        {
            if (quest.questID == questID)
            {
                return quest.isCompleted;
            }
        }
        return false;
    }

    public bool HasQuest(string questID)
    {
        foreach (var quest in QuestsProgress)
        {
            if (quest.questID == questID)
            {
                return quest.hasQuest;
            }
        }
        return false;
    }

    public void GiveQuest(string questID)
    {
        foreach (var quest in QuestsProgress)
        {
            if (quest.questID == questID && !quest.hasQuest && !quest.isCompleted)
            {
                quest.hasQuest = true;
                Debug.Log($"Quest {questID} given to player");
                return;
            }
        }

        var newQuest = new QuestProgress
        {
            questID = questID,
            hasQuest = true,
            isCompleted = false
        };

        var newList = new QuestProgress[QuestsProgress.Length + 1];
        for (int i = 0; i < QuestsProgress.Length; i++)
        {
            newList[i] = QuestsProgress[i];
        }
        newList[QuestsProgress.Length] = newQuest;
        QuestsProgress = newList;
    }

    public void CompleteQuest(string questID)
    {
        foreach (var quest in QuestsProgress)
        {
            if (quest.questID == questID)
            {
                quest.isCompleted = true;
                Debug.Log($"Quest {questID} marked as completed");
                return;
            }
        }
    }
}