using UnityEngine;
using UnityEngine.Events;

public class TestIfCorrect : MonoBehaviour
{
    public float tolerance = 0.1f;

    public RotateFloor[] runes;
    public int[] correctAngles;  
    public AudioClip successSound;
    public AudioClip failureSound;

    public UnityEvent onSuccess;
    public UnityEvent onFailure;

    public AudioSource audioSource;

    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
    }

    public void TestIfCorrectMethod()
    {
        if (runes.Length != correctAngles.Length)
        {
            Debug.LogError("Runes and CorrectAngles arrays must be the same length!");
            return;
        }

        for (int i = 0; i < runes.Length; i++)
        {
            if (runes[i].currentStage != correctAngles[i])
            {
                Debug.LogError($"Rune '{runes[i].name}' failed: Current Stage = {runes[i].currentStage}, Target Stage = {correctAngles[i]}");
                onFailure.Invoke();
                audioSource.PlayOneShot(failureSound);
                return;
            }
        }
        Debug.LogWarning("All runes are correctly positioned!");
        onSuccess.Invoke();
        audioSource.PlayOneShot(successSound);
    }
}