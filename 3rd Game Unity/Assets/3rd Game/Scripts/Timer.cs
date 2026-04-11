using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public UnityEvent onEndTimerEvent;
    public UnityEvent onStartTimerEvent;
    public float duration = 5f; // Duration of the timer in seconds
    private float initialDuration; // To store the initial duration for resetting
    public int endValue = 0; // Value to set when the timer ends

    public bool startOnAwake = true; // Whether to start the timer automatically


    private void Start()
    {
        initialDuration = duration;
    }
    public void StartTimer()
    {
        startOnAwake = true;
        duration = initialDuration;
        Debug.Log("Timer started with duration: " + duration + " seconds.");
        onStartTimerEvent.Invoke();
    }

    public void ResetTimer()
    {
        startOnAwake = true;
        duration = initialDuration;
        Debug.Log("Timer reset.");
    }

    public void ForceStopTimer()
    {
        startOnAwake = false;
        duration = 0f;
        Debug.Log("Timer force stopped.");
        onEndTimerEvent.Invoke();
    }

    private void Update()
    {
        if (duration > 0f && startOnAwake == true)
        {
            duration -= Time.deltaTime;
            if (duration <= 0f)
            {
                duration = 0f;
                onEndTimerEvent.Invoke();
                startOnAwake = false;
                Debug.Log("Timer ended. Setting value to: " + endValue);
            }
        }
    }
}
