using UnityEngine;
using UnityEngine.Events;

public class PressurePlateManager : MonoBehaviour
{
    public int totalPlates;
    public int activatedPlates;

    public UnityEvent onPuzzleCompleted;
    public UnityEvent onTimerReached;

    public float timer = 0f;
    public float timerGoal = 5f; // Goal for the timer
    private bool timerActive = false;
    private float tickingInterval;

    public GameObject boxToReset;
    public Transform boxResetPoint;
    public AudioSource audioSource;
    public AudioClip tickingSFX;


    private void Start()
    {
        activatedPlates = 0;
        tickingInterval = 0f;
        boxToReset.transform.position = boxResetPoint.position;
        Debug.LogWarning("Box reset to original position.");
    }
    public void ActivatePlate()
    {
        activatedPlates++;
        Debug.LogWarning($"Pressure Plate Pressed current active plates {activatedPlates}");
        CheckPuzzleCompletion();
    }

    public void DeactivatePlate()
    {
        activatedPlates--;
        Debug.LogWarning($"Pressure Plate Released current active plates {activatedPlates}");
        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        if (activatedPlates >= totalPlates)
        {
            Debug.LogWarning("Puzzle Completed!");
            onPuzzleCompleted.Invoke();
        }
    }
    public void StartTimer()
    {
        timer = 0f; // Reset the timer
        timerActive = true;
    }

    public void StopTimer()
    {
        timer = 0f; // Reset the timer
        timerActive = false;
    }

    private void Update()
    {
        if (timerActive == true)
        {
            timer += Time.deltaTime;
            tickingInterval += Time.deltaTime;
            if (tickingInterval >= 1f) // Play ticking sound every second
            {
                audioSource.PlayOneShot(tickingSFX);
                tickingInterval = 0f; // Reset the interval
            }
            if (timer >= timerGoal)
            {
                Debug.LogWarning("Timer goal reached!");
                timerActive = false; // Stop the timer
                onTimerReached.Invoke();
                // Reset the box position
                if (boxToReset != null && boxResetPoint != null)
                {
                    boxToReset.transform.position = boxResetPoint.position;
                    Debug.LogWarning("Box reset to original position.");
                }
            }
        }
    }
}
