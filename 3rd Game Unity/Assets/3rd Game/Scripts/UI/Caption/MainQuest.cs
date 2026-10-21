using UnityEngine;
using UnityEngine.Events;

public class MainQuest : MonoBehaviour
{
    [SerializeField] bool mainQuestTriggerable = false;

    public UnityEvent onMainQuestTriggered;

    public void CheckIfTriggerable()
    {
        if (mainQuestTriggerable)
        {
            onMainQuestTriggered.Invoke();
        }
    }

    public void SetMainQuestTriggerable(bool value)
    {
        mainQuestTriggerable = value;
    }
}
