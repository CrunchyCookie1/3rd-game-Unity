using UnityEngine;
using UnityEngine.Events;

public class ZoneEvent : MonoBehaviour
{
    public UnityEvent onTriggerEvent;
    public Transform tpLocation;

    public bool TpOnTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEvent?.Invoke();
            if (tpLocation != null && TpOnTrigger == true)
            {
                other.transform.position = tpLocation.position;
            }
        }
    }

    public void TpToLocation()
    {
        if (tpLocation != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = tpLocation.position;
            }
        }
    }
}
