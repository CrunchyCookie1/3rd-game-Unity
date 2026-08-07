using UnityEngine;
using UnityEngine.Events;

public class LoadEvent : MonoBehaviour
{
    public UnityEvent onLoad;

    private void Update()
    {
        onLoad?.Invoke();
    }
}
