using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPlateActivated;
    public UnityEvent onPlateDeactivated;

    public UnityEvent timerActivated;
    public UnityEvent timerDeactivated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            onPlateActivated.Invoke();
        }

        if (other.CompareTag("Box"))
        {
            Timer();
            Debug.LogWarning("Box on pressure plate. Timer activated!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            onPlateDeactivated.Invoke();
        }

        if (other.CompareTag("Box"))
        {
            StopTimer();
            Debug.LogWarning("Box off pressure plate. Timer deactivated!");
        }
    }

    private void Timer()
    {
        timerActivated.Invoke();
    }

    private void StopTimer()
    {
        timerDeactivated.Invoke();
    }
}
