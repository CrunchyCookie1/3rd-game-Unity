using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GiveQuestZone2 : MonoBehaviour
{
    public UnityEvent onQuestGiven;

    public string questName;
    [TextArea(3, 10)]
    public string description;
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questNameText.text = questName;
            descriptionText.text = description;
            onQuestGiven.Invoke();
            Debug.Log("Quest given: " + questName);
            gameObject.SetActive(false);
        }
    }
}
