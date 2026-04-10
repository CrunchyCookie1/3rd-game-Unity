using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SceneName";

    public void ChangeToScene()
    {
        Invoke(nameof(DelayedSceneChange), 0.5f);
        _targetScene = sceneToLoad;
        Debug.Log("Scene Changed");
    }

    private string _targetScene;

    private void DelayedSceneChange()
    {
        SceneManager.LoadScene(_targetScene);
    }
}