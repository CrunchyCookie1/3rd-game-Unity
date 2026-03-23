using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int questsCompleted = 0;


    public void Awake()
    {
        loadValues();
    }
    public void incrementQuestCompleted()
    {
        questsCompleted++;
        saveValues();
    }
    public void decrementQuestCompleted()
    {
        if (questsCompleted > 0)
        {
            questsCompleted--;
        }
    }

    public void saveValues()
    {
        PlayerPrefs.SetInt("QuestsCompleted", questsCompleted);
    }

    public void loadValues()
    {
        if (PlayerPrefs.HasKey("QuestsCompleted"))
        {
            questsCompleted = PlayerPrefs.GetInt("QuestsCompleted");
        }
    }
}
