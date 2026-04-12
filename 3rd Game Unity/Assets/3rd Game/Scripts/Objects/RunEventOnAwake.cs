using UnityEngine;
using UnityEngine.Events;

public class RunEventOnAwake : MonoBehaviour
{
    public UnityEvent onObjectAwakeEvent;

    private void Start()
    {
        onObjectAwakeEvent.Invoke();
    }
}
