using UnityEngine;
using UnityEngine.Events;

public class BoxSlotPuzzle : MonoBehaviour
{
    public UnityEvent onTriggerEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PuzzleBox"))
        {
            Debug.Log("Box placed in slot!");
            onTriggerEvent?.Invoke();
        }
    }
}
