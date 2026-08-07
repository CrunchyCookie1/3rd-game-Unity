using UnityEngine;

public class DeleteData : MonoBehaviour
{
    public void deleteData()
    {
        PlayerPrefs.DeleteAll();
    }
}
