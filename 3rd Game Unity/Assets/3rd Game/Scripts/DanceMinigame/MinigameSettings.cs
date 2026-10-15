using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class MinigameSettings : MonoBehaviour
{
    [SerializeField] float miniGameTime = 60f;
    [SerializeField] float scoreValue = 0f;
    [SerializeField] float playerHighScore = 0f;
    [SerializeField] float highScore = 50f;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI timerText;
    public bool startTimer = false;
    [SerializeField] bool noTimerMode = false;

    [Header("Timer Settings")]
    [SerializeField] GameObject basicInteract;

    [Header("No Timer Settings")]
    [SerializeField] float noTimerModeClock = 0f;
    public float lives = 3f;
    [SerializeField] Slider livesSlider;

    [SerializeField] GameObject noTimerInteract;
    [SerializeField] GameObject livesObject;
    [SerializeField] GameObject timerTextObject;
    [SerializeField] GameObject beatHighScoreObject;

    [SerializeField] UnityEvent onNoTimeModeEnd;

    [Header("End Of Game Settings")]
    [SerializeField] UnityEvent onMinigameEnd;
    [SerializeField] UnityEvent onHighScoreBeaten;
    [SerializeField] UnityEvent onHighScoreNotBeaten;
    private bool hasGameEnded = false;

    private void Update()
    {
        if (hasGameEnded == false && startTimer == true)
        {
            miniGameTime -= Time.deltaTime;
            timerText.text = Mathf.Round(miniGameTime).ToString();
            GameEvents();
        }

        if (hasGameEnded == false && noTimerMode == true)
        {
            noTimerModeClock += Time.deltaTime;
            GameEvents2();
        }
    }

    private void Start()
    {
        LoadHighScoreSave();
        scoreText.text = scoreValue.ToString();
        highScoreText.text = playerHighScore.ToString();
        timerText.text = Mathf.Round(miniGameTime).ToString();
    }

    public void StartTimer()
    {
        startTimer = true;
    }

    public void EndTimer()
    {
        startTimer = false;
    }

    private void GameEvents()
    {
        //Minigame Difficulty Increase
        if (miniGameTime < 60f && miniGameTime > 59.50f)
        {
            MoveInputObjects.dm_Speed = 200f;
            InputSpawner.SetIntervalRange(1f, 2f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (miniGameTime < 50f && miniGameTime > 49.50f)
        {
            MoveInputObjects.dm_Speed = 300f;
            InputSpawner.SetIntervalRange(0.5f, 1.5f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (miniGameTime < 30f && miniGameTime > 29.50f)
        {
            MoveInputObjects.dm_Speed = 400f;
            InputSpawner.SetIntervalRange(0.5f, 1f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (miniGameTime < 10f && miniGameTime > 9.50f)
        {
            MoveInputObjects.dm_Speed = 500f;
            InputSpawner.SetIntervalRange(0.5f, 0.8f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }

        //Timer Ended
        if (miniGameTime <= 0f)
        {
            //Destroy all input objects on the screen
            GameObject[] activeNotes = GameObject.FindGameObjectsWithTag("dm_Input");
            foreach (GameObject note in activeNotes)
            {
                Destroy(note);
            }

            //If High score was beaten
            if (scoreValue >= highScore)
            {
                onHighScoreBeaten.Invoke();
                beatHighScoreObject.SetActive(false);
                Debug.LogError("High Score Beaten!");
            }
            //If failed to beat high score
            if (scoreValue <= highScore)
            {
                onHighScoreNotBeaten.Invoke();
                Debug.LogError("Failed to beat high score!");
            }

            //End of minigame
            Debug.LogError("Minigame Ended");
            if (scoreValue >= playerHighScore)
            {
                playerHighScore = scoreValue;
            }
            highScoreText.text = playerHighScore.ToString();
            SaveHighScore();
            basicInteract.SetActive(true);
            onMinigameEnd.Invoke();
            hasGameEnded = true;
        }
    }

    private void GameEvents2()
    {
        if (noTimerModeClock > 10f && noTimerModeClock < 10.50f)
        {
            MoveInputObjects.dm_Speed = 200f;
            InputSpawner.SetIntervalRange(1f, 2f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (noTimerModeClock > 30f && noTimerModeClock < 30.50f)
        {
            MoveInputObjects.dm_Speed = 300f;
            InputSpawner.SetIntervalRange(1f, 1.5f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (noTimerModeClock > 60f && noTimerModeClock < 60.50f)
        {
            MoveInputObjects.dm_Speed = 400f;
            InputSpawner.SetIntervalRange(0.5f, 1f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (noTimerModeClock > 90f && noTimerModeClock < 90.50f)
        {
            MoveInputObjects.dm_Speed = 500f;
            InputSpawner.SetIntervalRange(0.5f, 1f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (noTimerModeClock > 120f && noTimerModeClock < 120.50f)
        {
            MoveInputObjects.dm_Speed = 550f;
            InputSpawner.SetIntervalRange(0.3f, 0.8f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }
        if (noTimerModeClock > 150f && noTimerModeClock < 150.50f)
        {
            MoveInputObjects.dm_Speed = 600f;
            InputSpawner.SetIntervalRange(0.3f, 0.5f);
            Debug.LogError($"Speed Increased {MoveInputObjects.dm_Speed} & Spawn Interval Increased {InputSpawner.spawnInterval}");
        }

        if (lives <= 0f)
        {
            noTimerInteract.SetActive(true);
            onNoTimeModeEnd.Invoke();
            EndMinigame();
        }
    }

    public void EndMinigame()
    {
        //Destroy all input objects on the screen
        GameObject[] activeNotes = GameObject.FindGameObjectsWithTag("dm_Input");
        foreach (GameObject note in activeNotes)
        {
            Destroy(note);
        }
        //End of minigame
        Debug.LogError("Minigame Ended");
        if (scoreValue >= playerHighScore)
        {
            playerHighScore = scoreValue;
        }
        highScoreText.text = playerHighScore.ToString();
        SaveHighScore();
        onMinigameEnd.Invoke();
        hasGameEnded = true;
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetFloat("PlayerHighScore", playerHighScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScoreSave()
    {
        if (PlayerPrefs.HasKey("PlayerHighScore"))
        {
            playerHighScore = PlayerPrefs.GetFloat("PlayerHighScore");
        }
        else
        {
            playerHighScore = 0f;
        }
    }

    public void IncrementScoreValue()
    {
        scoreValue += 1f;
        scoreText.text = scoreValue.ToString();
    }

    public void DecrementScoreValue(int amount)
    {
        scoreValue -= amount;
        if (scoreValue < 0f) scoreValue = 0f;
        scoreText.text = scoreValue.ToString();

        if (noTimerMode == true)
        {
            lives -= amount;
            if (lives < 0f) lives = 0f;
            livesSlider.value = lives;
        }
    }

    public void Reset()
    {
        noTimerModeClock = 0f;
        lives = 3f;
        livesSlider.value = lives;
        miniGameTime = 60f;
        scoreValue = 0f;
        scoreText.text = scoreValue.ToString();
        timerText.text = Mathf.Round(miniGameTime).ToString();
        MoveInputObjects.dm_Speed = 100f;
        InputSpawner.spawnInterval = 3f;
        hasGameEnded = false;
    }

    public void NoTimerActivate()
    {
        noTimerMode = true;
        MoveInputObjects.dm_Speed = 200f;
        InputSpawner.SetIntervalRange(1f, 3f);
        livesObject.SetActive(true);
        beatHighScoreObject.SetActive(false);
        timerTextObject.SetActive(false);
    }

    public void NoTimerDeactivate()
    {
        noTimerMode = false;
        livesObject.SetActive(false);
        timerTextObject.SetActive(true);
    }
}
