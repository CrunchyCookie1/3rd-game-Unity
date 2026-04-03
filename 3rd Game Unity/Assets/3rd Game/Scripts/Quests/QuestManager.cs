using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest UI Elements")]
        public GameObject activeQuestContainer;
        public GameObject completedQuestContainer;
        public GameObject questPanelPrefab; // Optional: use a prefab instead of individual panels

        [Header("Quest List")]
        public Quest[] availableQuests;

        [Header("Save Settings")]
        public string saveFileName = "questsave.dat";
        public bool autoSave = true;
        public bool showCompletedQuests = true;

        private List<ActiveQuest> activeQuests = new List<ActiveQuest>();
        private List<CompletedQuest> completedQuests = new List<CompletedQuest>();

        [System.Serializable]
        public class Quest
        {
            public string questID;
            public string questTitle;
            public string completedTitle;
            public string questDescription;
            public string completedDescription;
            public QuestType questType;
            public int requiredAmount;
            public string targetItem;
            public string targetNPC;

            [Header("UI Elements (Optional)")]
            public GameObject questPanelPrefab;
            public TextMeshProUGUI titleText;
            public TextMeshProUGUI descriptionText;

            public enum QuestType
            {
                Simple,
                Gather,
                Kill,
                Interaction
            }
        }

        [System.Serializable]
        public class ActiveQuest
        {
            public string questID;
            public int currentAmount;
            public bool isCompleted;
            public GameObject questPanel;
            public TextMeshProUGUI titleText;
            public TextMeshProUGUI descriptionText;

            public ActiveQuest(Quest original)
            {
                questID = original.questID;
                currentAmount = 0;
                isCompleted = false;
            }
        }

        [System.Serializable]
        public class CompletedQuest
        {
            public string questID;
            public string completionDate;

            public CompletedQuest(string id)
            {
                questID = id;
                completionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        [System.Serializable]
        private class SaveData
        {
            public List<string> completedQuestIDs = new List<string>();
            public List<SavedActiveQuest> activeQuests = new List<SavedActiveQuest>();
        }

        [System.Serializable]
        private class SavedActiveQuest
        {
            public string questID;
            public int currentAmount;
            public bool isCompleted;
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
                return;
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
            {
                Debug.Log($"Quest {questID} is already {(IsQuestActive(questID) ? "active" : "completed")}");
                return false;
            }

            Quest questData = GetQuestByID(questID);
            if (questData == null)
            {
                Debug.LogError($"Quest with ID {questID} not found!");
                return false;
            }

            ActiveQuest newQuest = new ActiveQuest(questData);

            // Create UI for the quest
            CreateQuestUI(newQuest, questData);

            activeQuests.Add(newQuest);
            UpdateSingleQuestUI(newQuest, questData);

            if (autoSave) SaveQuests();
            Debug.Log($"Accepted quest: {questData.questTitle}");
            return true;
        }

        private void CreateQuestUI(ActiveQuest quest, Quest questData)
        {
            // Determine which container to use
            Transform targetContainer = activeQuestContainer != null ? activeQuestContainer.transform : null;

            // Use the quest's specific prefab if available, otherwise use the general prefab
            GameObject prefabToUse = questData.questPanelPrefab != null ? questData.questPanelPrefab : questPanelPrefab;

            if (prefabToUse != null && targetContainer != null)
            {
                quest.questPanel = Instantiate(prefabToUse, targetContainer);
                quest.questPanel.SetActive(true);

                // Find text components
                TextMeshProUGUI[] textComponents = quest.questPanel.GetComponentsInChildren<TextMeshProUGUI>();
                if (textComponents.Length >= 2)
                {
                    quest.titleText = textComponents[0];
                    quest.descriptionText = textComponents[1];
                }
                else if (textComponents.Length == 1)
                {
                    quest.titleText = textComponents[0];
                }
            }
            else if (questData.titleText != null && questData.descriptionText != null)
            {
                // Use direct references if provided
                quest.titleText = questData.titleText;
                quest.descriptionText = questData.descriptionText;
                if (questData.questPanelPrefab != null)
                    quest.questPanel = questData.questPanelPrefab;
            }
        }

        private Quest GetQuestByID(string questID)
        {
            return availableQuests.FirstOrDefault(q => q.questID == questID);
        }

        public void UpdateQuestProgress(string questID, int amount)
        {
            ActiveQuest quest = GetActiveQuest(questID);
            if (quest == null || quest.isCompleted) return;

            Quest questData = GetQuestByID(questID);
            if (questData == null) return;

            quest.currentAmount += amount;

            if (quest.currentAmount >= questData.requiredAmount)
            {
                CompleteQuest(questID);
            }
            else
            {
                UpdateSingleQuestUI(quest, questData);
            }

            if (autoSave) SaveQuests();
        }

        public void SetQuestProgress(string questID, int amount)
        {
            ActiveQuest quest = GetActiveQuest(questID);
            if (quest == null || quest.isCompleted) return;

            Quest questData = GetQuestByID(questID);
            if (questData == null) return;

            quest.currentAmount = Mathf.Min(amount, questData.requiredAmount);

            if (quest.currentAmount >= questData.requiredAmount)
            {
                CompleteQuest(questID);
            }
            else
            {
                UpdateSingleQuestUI(quest, questData);
            }

            if (autoSave) SaveQuests();
        }

        public void CompleteInteraction(string questID, string npcName)
        {
            ActiveQuest quest = GetActiveQuest(questID);
            if (quest == null || quest.isCompleted) return;

            Quest questData = GetQuestByID(questID);
            if (questData != null && questData.questType == Quest.QuestType.Interaction && questData.targetNPC == npcName)
            {
                CompleteQuest(questID);
                if (autoSave) SaveQuests();
            }
        }

        public void ForceCompleteQuest(string questID)
        {
            ActiveQuest quest = GetActiveQuest(questID);
            if (quest != null && !quest.isCompleted)
            {
                CompleteQuest(questID);
                if (autoSave) SaveQuests();
            }
        }

        private void CompleteQuest(string questID)
        {
            ActiveQuest quest = GetActiveQuest(questID);
            if (quest == null) return;

            Quest questData = GetQuestByID(questID);
            if (questData == null) return;

            quest.isCompleted = true;
            quest.currentAmount = questData.requiredAmount;

            UpdateSingleQuestUI(quest, questData);

            // Move to completed quests list
            activeQuests.Remove(quest);

            // Only add to completed if not already there
            if (!IsQuestCompleted(questID))
            {
                completedQuests.Add(new CompletedQuest(questID));
            }

            // Optionally move UI to completed container
            if (showCompletedQuests && completedQuestContainer != null && quest.questPanel != null)
            {
                quest.questPanel.transform.SetParent(completedQuestContainer.transform);
            }
            else if (quest.questPanel != null)
            {
                // Keep it in active container but mark as completed visually
                UpdateSingleQuestUI(quest, questData);
            }

            if (autoSave) SaveQuests();
            Debug.Log($"Quest completed: {questData.questTitle}");

            // Optional: Trigger any completion events
            OnQuestCompleted(questID);
        }

        private void OnQuestCompleted(string questID)
        {
            // You can add additional logic here, like giving rewards
            Debug.Log($"Quest {questID} completed! Add rewards here.");
        }

        public bool IsQuestActive(string questID)
        {
            return GetActiveQuest(questID) != null;
        }

        public bool IsQuestCompleted(string questID)
        {
            return completedQuests.Any(q => q.questID == questID);
        }

        public int GetQuestProgress(string questID)
        {
            ActiveQuest quest = GetActiveQuest(questID);
            return quest != null ? quest.currentAmount : 0;
        }

        private ActiveQuest GetActiveQuest(string questID)
        {
            return activeQuests.FirstOrDefault(q => q.questID == questID);
        }

        private void UpdateAllQuestUI()
        {
            foreach (ActiveQuest quest in activeQuests)
            {
                Quest questData = GetQuestByID(quest.questID);
                if (questData != null)
                {
                    UpdateSingleQuestUI(quest, questData);
                }
            }
        }

        private void UpdateSingleQuestUI(ActiveQuest quest, Quest questData)
        {
            if (quest == null || questData == null) return;

            if (quest.titleText != null)
            {
                quest.titleText.text = quest.isCompleted && !string.IsNullOrEmpty(questData.completedTitle)
                    ? questData.completedTitle
                    : questData.questTitle;

                // Optional: add strikethrough for completed quests
                if (quest.isCompleted && showCompletedQuests)
                {
                    quest.titleText.fontStyle = FontStyles.Strikethrough;
                }
            }

            if (quest.descriptionText != null)
            {
                string description = quest.isCompleted && !string.IsNullOrEmpty(questData.completedDescription)
                    ? questData.completedDescription
                    : questData.questDescription;

                if (!quest.isCompleted && (questData.questType == Quest.QuestType.Gather || questData.questType == Quest.QuestType.Kill))
                {
                    description += $"\n<color=yellow>Progress: {quest.currentAmount}/{questData.requiredAmount}</color>";
                }
                else if (quest.isCompleted)
                {
                    description = $"<color=green> {description}</color>";
                }

                quest.descriptionText.text = description;
            }

            if (quest.questPanel != null && !quest.questPanel.activeSelf)
                quest.questPanel.SetActive(true);
        }

        public void SaveQuests()
        {
            try
            {
                SaveData saveData = new SaveData();

                // Save completed quest IDs
                saveData.completedQuestIDs = completedQuests.Select(q => q.questID).ToList();

                // Save active quests
                foreach (ActiveQuest quest in activeQuests)
                {
                    SavedActiveQuest savedQuest = new SavedActiveQuest
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

                Debug.Log($"Quests saved to {filePath}. Active: {activeQuests.Count}, Completed: {completedQuests.Count}");
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
                if (!File.Exists(filePath))
                {
                    Debug.Log("No save file found, starting fresh.");
                    return;
                }

                string json = File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData != null)
                {
                    // Clear existing data
                    ClearAllQuestUI();
                    activeQuests.Clear();
                    completedQuests.Clear();

                    // Load completed quests
                    foreach (string questID in saveData.completedQuestIDs)
                    {
                        completedQuests.Add(new CompletedQuest(questID));
                    }

                    // Load active quests
                    foreach (SavedActiveQuest savedQuest in saveData.activeQuests)
                    {
                        Quest originalQuest = GetQuestByID(savedQuest.questID);
                        if (originalQuest != null)
                        {
                            ActiveQuest loadedQuest = new ActiveQuest(originalQuest)
                            {
                                currentAmount = savedQuest.currentAmount,
                                isCompleted = savedQuest.isCompleted
                            };

                            CreateQuestUI(loadedQuest, originalQuest);

                            if (loadedQuest.isCompleted)
                            {
                                // Move to completed if it was completed
                                completedQuests.Add(new CompletedQuest(loadedQuest.questID));
                                if (showCompletedQuests && completedQuestContainer != null && loadedQuest.questPanel != null)
                                {
                                    loadedQuest.questPanel.transform.SetParent(completedQuestContainer.transform);
                                }
                            }
                            else
                            {
                                activeQuests.Add(loadedQuest);
                            }

                            UpdateSingleQuestUI(loadedQuest, originalQuest);
                        }
                    }

                    Debug.Log($"Quests loaded. Active: {activeQuests.Count}, Completed: {completedQuests.Count}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load quests: {e.Message}");
            }
        }

        private void ClearAllQuestUI()
        {
            // Clear active quest UI
            foreach (ActiveQuest quest in activeQuests)
            {
                if (quest.questPanel != null)
                    Destroy(quest.questPanel);
            }

            // Clear any remaining UI in containers
            if (activeQuestContainer != null)
            {
                foreach (Transform child in activeQuestContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (completedQuestContainer != null)
            {
                foreach (Transform child in completedQuestContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void ResetAllQuests()
        {
            ClearAllQuestUI();
            activeQuests.Clear();
            completedQuests.Clear();

            if (autoSave) SaveQuests();
            Debug.Log("All quests reset");
        }

        public List<string> GetCompletedQuestIDs()
        {
            return completedQuests.Select(q => q.questID).ToList();
        }

        public List<string> GetActiveQuestIDs()
        {
            return activeQuests.Select(q => q.questID).ToList();
        }

        private void OnApplicationQuit()
        {
            if (autoSave)
            {
                SaveQuests();
            }
        }

        private void OnDestroy()
        {
            if (autoSave && Instance == this)
            {
                SaveQuests();
            }
        }
    }
}