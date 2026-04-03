using UnityEngine;

public class ResetData : MonoBehaviour
{
    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player data reset.");
    }
}
