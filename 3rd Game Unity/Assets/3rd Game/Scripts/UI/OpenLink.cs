using UnityEngine;

public class OpenLink : MonoBehaviour
{
    [SerializeField] string url = "https://unity.com";
    public void OpenMyUrl()
    {
        Application.OpenURL(url);
    }
}
