using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;

namespace QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest UI Elements")]
        public GameObject questContainer;

        [Header("Quest List")]
        public Quest[] availableQuests;

        [Header("Save Settings")]
        public string saveFileName = "questsave.dat";
        public bool autoSave = true;

        private List<Quest> activeQuests = new List<Quest>();
        private List<string> completedQuestIDs = new List<string>();

        [System.Serializable]
        public class Quest
        {
            public string questID;
            public string questTitle;
            public string questDescription;
            public QuestType questType;
            public int requiredAmount;
            public string targetItem;
            public string targetNPC;

            [Header("UI Elements")]
            public GameObject questPanel;
            public TextMeshProUGUI titleText;
            public TextMeshProUGUI descriptionText;

            [HideInInspector]
            public int currentAmount;
            [HideInInspector]
            public bool isCompleted;

            public enum QuestType
            {
                Simple,
                Gather,
                Kill,
                Interaction
            }
        }

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
            LoadQuests();
            UpdateAllQuestUI();
        }

        public bool AcceptQuest(string questID)
        {
            if (IsQuestActive(questID) || IsQuestCompleted(questID))
                return false;

            Quest newQuest = GetQuestByID(questID);
            if (newQuest != null)
            {
                GameObject newPanel = null;
                TextMeshProUGUI newTitle = null;
                TextMeshProUGUI newDescription = null;

                if (newQuest.questPanel != null)
                {
                    newPanel = Instantiate(newQuest.questPanel, questContainer != null ? questContainer.transform : null);
                    newTitle = newPanel.GetComponentInChildren<TextMeshProUGUI>();

                    TextMeshProUGUI[] allTexts = newPanel.GetComponentsInChildren<TextMeshProUGUI>();
                    if (allTexts.Length >= 2)
                    {
                        newTitle = allTexts[0];
                        newDescription = allTexts[1];
                    }
                }

                Quest activeQuest = new Quest
                {
                    questID = newQuest.questID,
                    questTitle = newQuest.questTitle,
                    questDescription = newQuest.questDescription,
                    questType = newQuest.questType,
                    requiredAmount = newQuest.requiredAmount,
                    targetItem = newQuest.targetItem,
                    targetNPC = newQuest.targetNPC,
                    currentAmount = 0,
                    isCompleted = false,
                    questPanel = newPanel,
                    titleText = newTitle,
                    descriptionText = newDescription
                };

                activeQuests.Add(activeQuest);
                UpdateSingleQuestUI(activeQuest);

                if (autoSave) SaveQuests();
                return true;
            }
            return false;
        }

        private Quest GetQuestByID(string questID)
        {
            foreach (Quest quest in availableQuests)
            {
                if (quest.questID == questID)
                    return quest;
            }
            return null;
        }

        public void UpdateQuestProgress(string questID, int amount)
        {
            Quest quest = GetActiveQuest(questID);
            if (quest == null) return;

            quest.currentAmount += amount;
            if (quest.currentAmount >= quest.requiredAmount)
            {
                CompleteQuest(questID);
            }

            UpdateSingleQuestUI(quest);
            if (autoSave) SaveQuests();
        }

        public void CompleteInteraction(string questID, string npcName)
        {
            Quest quest = GetActiveQuest(questID);
            if (quest != null && quest.questType == Quest.QuestType.Interaction && quest.targetNPC == npcName)
            {
                if (!quest.isCompleted)
                {
                    CompleteQuest(questID);
                    if (autoSave) SaveQuests();
                }
            }
        }

        private void CompleteQuest(string questID)
        {
            Quest quest = GetActiveQuest(questID);
            if (quest != null)
            {
                quest.isCompleted = true;
                activeQuests.Remove(quest);
                completedQuestIDs.Add(quest.questID);

                if (quest.questPanel != null)
                {
                    Destroy(quest.questPanel);
                }

                if (autoSave) SaveQuests();
            }
        }

        public bool IsQuestActive(string questID)
        {
            return GetActiveQuest(questID) != null;
        }

        public bool IsQuestCompleted(string questID)
        {
            return completedQuestIDs.Contains(questID);
        }

        private Quest GetActiveQuest(string questID)
        {
            return activeQuests.Find(q => q.questID == questID);
        }

        private void UpdateAllQuestUI()
        {
            foreach (Quest quest in activeQuests)
            {
                UpdateSingleQuestUI(quest);
            }
        }

        private void UpdateSingleQuestUI(Quest quest)
        {
            if (quest == null) return;

            if (quest.titleText != null)
                quest.titleText.text = quest.questTitle;

            if (quest.descriptionText != null)
            {
                string description = quest.questDescription;
                if (quest.questType == Quest.QuestType.Gather || quest.questType == Quest.QuestType.Kill)
                {
                    description += $"\nProgress: {quest.currentAmount}/{quest.requiredAmount}";
                }
                quest.descriptionText.text = description;
            }

            if (quest.questPanel != null && !quest.questPanel.activeSelf)
                quest.questPanel.SetActive(true);
        }

        [System.Serializable]
        private class SaveData
        {
            public string[] completedQuestIDs;
            public List<SavedQuest> activeQuests;
        }

        [System.Serializable]
        private class SavedQuest
        {
            public string questID;
            public int currentAmount;
            public bool isCompleted;
        }

        public void SaveQuests()
        {
            try
            {
                SaveData saveData = new SaveData();
                saveData.completedQuestIDs = completedQuestIDs.ToArray();
                saveData.activeQuests = new List<SavedQuest>();

                foreach (Quest quest in activeQuests)
                {
                    SavedQuest savedQuest = new SavedQuest
                    {
                        questID = quest.questID,
                        currentAmount = quest.currentAmount,
                        isCompleted = quest.isCompleted
                    };
                    saveData.activeQuests.Add(savedQuest);
                }

                string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
                string json = JsonUtility.ToJson(saveData);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save quests: {e.Message}");
            }
        }

        public void LoadQuests()
        {
            try
            {
                string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                    if (saveData != null)
                    {
                        completedQuestIDs = new List<string>(saveData.completedQuestIDs);
                        activeQuests.Clear();

                        foreach (SavedQuest savedQuest in saveData.activeQuests)
                        {
                            Quest originalQuest = GetQuestByID(savedQuest.questID);
                            if (originalQuest != null)
                            {
                                GameObject newPanel = null;
                                TextMeshProUGUI newTitle = null;
                                TextMeshProUGUI newDescription = null;

                                if (originalQuest.questPanel != null)
                                {
                                    newPanel = Instantiate(originalQuest.questPanel, questContainer != null ? questContainer.transform : null);
                                    TextMeshProUGUI[] allTexts = newPanel.GetComponentsInChildren<TextMeshProUGUI>();
                                    if (allTexts.Length >= 2)
                                    {
                                        newTitle = allTexts[0];
                                        newDescription = allTexts[1];
                                    }
                                }

                                Quest loadedQuest = new Quest
                                {
                                    questID = originalQuest.questID,
                                    questTitle = originalQuest.questTitle,
                                    questDescription = originalQuest.questDescription,
                                    questType = originalQuest.questType,
                                    requiredAmount = originalQuest.requiredAmount,
                                    targetItem = originalQuest.targetItem,
                                    targetNPC = originalQuest.targetNPC,
                                    currentAmount = savedQuest.currentAmount,
                                    isCompleted = savedQuest.isCompleted,
                                    questPanel = newPanel,
                                    titleText = newTitle,
                                    descriptionText = newDescription
                                };
                                activeQuests.Add(loadedQuest);
                                UpdateSingleQuestUI(loadedQuest);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load quests: {e.Message}");
            }
        }

        public void ResetAllQuests()
        {
            foreach (Quest quest in activeQuests)
            {
                if (quest.questPanel != null)
                    Destroy(quest.questPanel);
            }

            activeQuests.Clear();
            completedQuestIDs.Clear();

            if (autoSave) SaveQuests();
        }

        private void OnDestroy()
        {
            if (autoSave)
            {
                SaveQuests();
            }
        }
    }
}