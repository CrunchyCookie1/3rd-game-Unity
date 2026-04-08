using System.Collections;
using QuestSystem;
using UnityEngine;

public class GiveQuestZone : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questToGive;
    public QuestManager2 questManager2;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip questSound;
    public float soundVolume = 1f;

    [Header("GameObject Settings")]
    public GameObject objectToEnable;
    public bool disableZoneAfterQuest = true;

    [Header("Delay Settings")]
    public float disableDelay = 2f; // Delay before disabling the zone
    private bool hasTriggered = false; // Prevents multiple triggers

    private void OnTriggerEnter(Collider other)
    {
        // Prevent multiple triggers
        if (hasTriggered) return;

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
                if (disableZoneAfterQuest) gameObject.SetActive(false);
                return;
            }

            if (QuestProgression.Instance.IsQuestCompleted(questToGive))
            {
                Debug.Log($"Quest already completed: {questToGive}");
                if (disableZoneAfterQuest) gameObject.SetActive(false);
                return;
            }

            // Mark as triggered to prevent multiple executions
            hasTriggered = true;

            // Check if QuestManager2 is assigned
            if (questManager2 != null)
            {
                // Set the quest ID and assign the quest
                questManager2.questID = questToGive;
                questManager2.AssignQuest();
                QuestProgression.Instance.GiveQuest(questToGive);
                Debug.Log($"Quest given: {questToGive}");

                // Play the quest sound (immediately)
                PlayQuestSound();

                // Enable the GameObject (immediately)
                EnableGameObject();

                // Start coroutine to disable zone after delay
                if (disableZoneAfterQuest)
                {
                    StartCoroutine(DisableZoneAfterDelay());
                }
            }
            else
            {
                Debug.LogError("QuestManager2 is not assigned in the inspector!");
            }
        }
    }

    private void PlayQuestSound()
    {
        // Debug what's happening
        Debug.Log($"PlayQuestSound called - audioSource: {(audioSource != null ? audioSource.name : "NULL")}, questSound: {(questSound != null ? questSound.name : "NULL")}");

        if (audioSource != null && questSound != null)
        {
            audioSource.PlayOneShot(questSound, soundVolume);
            Debug.Log($"Playing quest sound: {questSound.name} at volume {soundVolume}");
        }
        else if (questSound != null)
        {
            Debug.Log("AudioSource is null, attempting to create one...");
            // If no AudioSource assigned, try to add one
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("Created new AudioSource component");
            }
            audioSource.PlayOneShot(questSound, soundVolume);
            Debug.Log($"Playing quest sound (Auto-created AudioSource): {questSound.name}");
        }
        else if (audioSource != null && questSound == null)
        {
            Debug.LogError("Quest Sound is not assigned in the Inspector!");
        }
        else
        {
            Debug.LogError("Both AudioSource and Quest Sound are not assigned in the Inspector!");
        }
    }

    private void EnableGameObject()
    {
        // Debug what's happening
        Debug.Log($"EnableGameObject called - objectToEnable: {(objectToEnable != null ? objectToEnable.name : "NULL")}");

        if (objectToEnable != null)
        {
            Debug.Log($"Attempting to enable: {objectToEnable.name}, Current active state: {objectToEnable.activeSelf}");
            objectToEnable.SetActive(true);
            Debug.Log($"Enabled GameObject: {objectToEnable.name}, New active state: {objectToEnable.activeSelf}");
        }
        else
        {
            Debug.LogError("No GameObject assigned to enable! Please assign objectToEnable in the Inspector.");
        }
    }

    private IEnumerator DisableZoneAfterDelay()
    {
        // Wait for specified delay
        yield return new WaitForSeconds(disableDelay);

        // Disable the zone
        gameObject.SetActive(false);
        Debug.Log($"Zone disabled after {disableDelay} seconds");
    }

    // Optional: Visual feedback in the Inspector
    private void OnValidate()
    {
        if (questSound != null && audioSource == null)
        {
            Debug.LogWarning($"AudioSource is not assigned for quest sound: {questSound.name}. The script will try to add one automatically.");
        }

        if (objectToEnable == null)
        {
            Debug.LogWarning("objectToEnable is not assigned in the Inspector!");
        }
    }
}