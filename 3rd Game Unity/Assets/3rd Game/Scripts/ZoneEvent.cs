using UnityEngine;
using UnityEngine.Events;

public class ZoneEvent : MonoBehaviour
{
    public UnityEvent onTriggerEvent;
    public Transform tpLocation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEvent?.Invoke();
            if (tpLocation != null)
            {
                other.transform.position = tpLocation.position;
            }
        }
    }
}
