using UnityEngine;
using TMPro;
using QuestSystem;

public class QuestManager2 : MonoBehaviour
{
    public static QuestManager2 Instance { get; private set; }

    public string questID;

    public string questTitle;
    public TextMeshProUGUI questTitleText;
    [TextArea(3, 10)]
    public string questDescription;
    public TextMeshProUGUI questDescriptionText;

    public string questTitleComplete;
    [TextArea(3, 10)]
    public string questDescriptionComplete;

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

    private void Start()
    {
        LoadSave();
    }

    public void AssignQuest()
    {
        // Update UI
        questTitleText.text = questTitle;
        questDescriptionText.text = questDescription;

        // Save to QuestProgression
        if (QuestProgression.Instance != null)
        {
            QuestProgression.Instance.GiveQuest(questID);
        }

        // Save locally
        SaveQuest();

        Debug.Log("Quest assigned: " + questTitle);
    }

    public void CompleteQuestText()
    {
        // Update UI
        questTitleText.text = questTitleComplete;
        questDescriptionText.text = questDescriptionComplete;

        // Save to QuestProgression
        if (QuestProgression.Instance != null)
        {
            QuestProgression.Instance.CompleteQuest(questID);
        }

        // Save locally
        SaveQuest();

        Debug.Log("Quest completed: " + questTitleComplete);
    }

    public void SaveQuest()
    {
        // Check if quest is completed by looking at QuestProgression
        bool isCompleted = false;
        if (QuestProgression.Instance != null)
        {
            isCompleted = QuestProgression.Instance.IsQuestCompleted(questID);
        }

        PlayerPrefs.SetString("QuestID_" + questID, questID);
        PlayerPrefs.SetString("QuestTitle_" + questID, questTitle);
        PlayerPrefs.SetString("QuestDescription_" + questID, questDescription);
        PlayerPrefs.SetString("QuestTitleComplete_" + questID, questTitleComplete);
        PlayerPrefs.SetString("QuestDescriptionComplete_" + questID, questDescriptionComplete);
        PlayerPrefs.SetInt("QuestIsCompleted_" + questID, isCompleted ? 1 : 0);
        PlayerPrefs.SetInt("QuestHasQuest_" + questID, QuestProgression.Instance.HasQuest(questID) ? 1 : 0);

        Debug.Log("Quest saved: " + questTitle);
    }

    public void LoadSave()
    {
        if (PlayerPrefs.HasKey("QuestID_" + questID))
        {
            // Load saved data
            questID = PlayerPrefs.GetString("QuestID_" + questID);
            questTitle = PlayerPrefs.GetString("QuestTitle_" + questID);
            questDescription = PlayerPrefs.GetString("QuestDescription_" + questID);
            questTitleComplete = PlayerPrefs.GetString("QuestTitleComplete_" + questID);
            questDescriptionComplete = PlayerPrefs.GetString("QuestDescriptionComplete_" + questID);

            bool isCompleted = PlayerPrefs.GetInt("QuestIsCompleted_" + questID, 0) == 1;
            bool hasQuest = PlayerPrefs.GetInt("QuestHasQuest_" + questID, 0) == 1;

            // Update QuestProgression to match saved data
            if (QuestProgression.Instance != null)
            {
                // Find or create quest entry
                bool found = false;
                for (int i = 0; i < QuestProgression.Instance.QuestsProgress.Length; i++)
                {
                    if (QuestProgression.Instance.QuestsProgress[i].questID == questID)
                    {
                        QuestProgression.Instance.QuestsProgress[i].hasQuest = hasQuest;
                        QuestProgression.Instance.QuestsProgress[i].isCompleted = isCompleted;
                        found = true;
                        break;
                    }
                }

                if (!found && hasQuest)
                {
                    // Add new quest entry if it doesn't exist
                    var newList = new QuestProgression.QuestProgress[QuestProgression.Instance.QuestsProgress.Length + 1];
                    for (int i = 0; i < QuestProgression.Instance.QuestsProgress.Length; i++)
                    {
                        newList[i] = QuestProgression.Instance.QuestsProgress[i];
                    }
                    newList[QuestProgression.Instance.QuestsProgress.Length] = new QuestProgression.QuestProgress
                    {
                        questID = questID,
                        hasQuest = hasQuest,
                        isCompleted = isCompleted
                    };
                    QuestProgression.Instance.QuestsProgress = newList;
                }
            }

            // Load the appropriate text onto the UI
            if (isCompleted)
            {
                questTitleText.text = questTitleComplete;
                questDescriptionText.text = questDescriptionComplete;
                Debug.Log("Loaded completed quest: " + questTitleComplete);
            }
            else if (hasQuest)
            {
                questTitleText.text = questTitle;
                questDescriptionText.text = questDescription;
                Debug.Log("Loaded active quest: " + questTitle);
            }
            else
            {
                // Quest was reset
                questTitleText.text = "";
                questDescriptionText.text = "";
                Debug.Log("Quest was reset, UI cleared");
            }
        }
        else
        {
            Debug.Log("No saved quest found for ID: " + questID);
            // Clear the UI text if no quest is saved
            if (questTitleText != null)
                questTitleText.text = "";
            if (questDescriptionText != null)
                questDescriptionText.text = "";
        }
    }

    public void ResetSaveData()
    {
        if (PlayerPrefs.HasKey("QuestID_" + questID))
        {
            // Delete PlayerPrefs
            PlayerPrefs.DeleteKey("QuestID_" + questID);
            PlayerPrefs.DeleteKey("QuestTitle_" + questID);
            PlayerPrefs.DeleteKey("QuestDescription_" + questID);
            PlayerPrefs.DeleteKey("QuestTitleComplete_" + questID);
            PlayerPrefs.DeleteKey("QuestDescriptionComplete_" + questID);
            PlayerPrefs.DeleteKey("QuestIsCompleted_" + questID);
            PlayerPrefs.DeleteKey("QuestHasQuest_" + questID);

            // Reset in QuestProgression
            if (QuestProgression.Instance != null)
            {
                for (int i = 0; i < QuestProgression.Instance.QuestsProgress.Length; i++)
                {
                    if (QuestProgression.Instance.QuestsProgress[i].questID == questID)
                    {
                        QuestProgression.Instance.QuestsProgress[i].hasQuest = false;
                        QuestProgression.Instance.QuestsProgress[i].isCompleted = false;
                        break;
                    }
                }
            }

            // Clear the UI text
            if (questTitleText != null)
                questTitleText.text = "";
            if (questDescriptionText != null)
                questDescriptionText.text = "";

            Debug.Log("Quest save data reset for ID: " + questID);
        }
        else
        {
            Debug.LogWarning("No saved quest found to reset for ID: " + questID);
        }
    }
}