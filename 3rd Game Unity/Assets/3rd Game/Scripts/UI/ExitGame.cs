using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    [SerializeField] private float quitDelay = 0.5f;

    public void QuitApplication()
    {
        Invoke(nameof(DelayedSceneChange), quitDelay);
    }

    private void DelayedSceneChange()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }
}
