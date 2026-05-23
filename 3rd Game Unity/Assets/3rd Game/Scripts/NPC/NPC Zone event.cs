using UnityEngine;
using UnityEngine.Events;

public class NPCZoneevent : MonoBehaviour
{
    public UnityEvent onTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            onTriggerEvent?.Invoke();
            Debug.Log("NPC Has entered zone!");
        }
    }
}
