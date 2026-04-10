using UnityEngine;
using UnityEngine.Events;

public class ZoneEvent : MonoBehaviour
{
    public UnityEvent onTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEvent?.Invoke();
        }
    }
}
