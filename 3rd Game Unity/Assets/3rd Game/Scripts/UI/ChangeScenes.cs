using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public void ChangeToScene(string sceneName)
    {
        Invoke(nameof(DelayedSceneChange), 0.5f);
        _targetScene = sceneName;
    }

    private string _targetScene;

    private void DelayedSceneChange()
    {
        SceneManager.LoadScene(_targetScene);
    }

}
