using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class RiddlePuzzle : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] riddleQuestion;
    [TextArea(3, 10)]
    public string[] correctAnswers;

    public int currentRiddleIndex = 0;
    public TextMeshPro riddleText;
    public TMP_InputField answerInputField;

    public UnityEvent onCorrectAnswer;
    public UnityEvent onIncorrectAnswer;

    private void Start()
    {
        RandomizeRiddle();
    }

    public void SubmitAnswer()
    {
        // Ensure the input field is updated before reading its value
        answerInputField.ForceLabelUpdate();
        string playerInput = answerInputField.text;
        TestIfCorrectAnswer(playerInput);
    }

    public void TestIfCorrectAnswer(string playerAnswer)
    {
        if (currentRiddleIndex < correctAnswers.Length)
        {
            if (playerAnswer.Trim().Equals(correctAnswers[currentRiddleIndex].Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Correct answer!");
                onCorrectAnswer?.Invoke();
                // Do not move to the next riddle
            }
            else
            {
                Debug.Log("Incorrect answer. Try again.");
                onIncorrectAnswer?.Invoke();
                RandomizeRiddle();
            }
        }
    }

    private void ShowCurrentRiddle()
    {
        if (currentRiddleIndex < riddleQuestion.Length)
        {
            riddleText.text = riddleQuestion[currentRiddleIndex];
            answerInputField.text = "";
        }
    }

    public void RandomizeRiddle()
    {
        if (riddleQuestion.Length > 0)
        {
            currentRiddleIndex = Random.Range(0, riddleQuestion.Length);
            ShowCurrentRiddle();
        }
    }
}
