using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float maxTime = 600f; // 10 minutes in seconds
    [SerializeField] private bool startOnAwake = true;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image progressBar;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onTimerComplete;

    private float currentTime = 0f;
    private bool isRunning = false;
    private bool isComplete = false;

    void Start()
    {
        if (startOnAwake)
        {
            StartTimer();
        }
        UpdateUI();
    }

    void Update()
    {
        if (isRunning && !isComplete)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= maxTime)
            {
                currentTime = maxTime;
                isComplete = true;
                isRunning = false;
                OnTimerComplete();
            }

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Update timer text
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Update progress bar
        if (progressBar != null)
        {
            progressBar.fillAmount = currentTime / maxTime;
        }
    }

    public void StartTimer()
    {
        if (!isComplete)
        {
            isRunning = true;
        }
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (!isComplete)
        {
            isRunning = true;
        }
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        isRunning = false;
        isComplete = false;
        UpdateUI();
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
        if (currentTime > maxTime)
        {
            currentTime = maxTime;
        }
        UpdateUI();
    }

    public void SubtractTime(float seconds)
    {
        currentTime -= seconds;
        if (currentTime < 0)
        {
            currentTime = 0;
        }
        UpdateUI();
    }

    private void OnTimerComplete()
    {
        Debug.Log("Timer Complete! 10 minutes reached.");
        onTimerComplete?.Invoke();
    }

    // Public getters
    public float GetCurrentTime() => currentTime;
    public float GetRemainingTime() => maxTime - currentTime;
    public float GetProgress() => currentTime / maxTime;
    public bool IsRunning() => isRunning;
    public bool IsComplete() => isComplete;
}
