using UnityEngine;

public class RotateFloor : MonoBehaviour
{
    [Range(0, 3)] public int startStage = 0; // 0=90°, 1=180°, 2=270°, 3=0°

    public Transform pipe;

    public int currentStage = 0;
    private float[] cornerRotations = { 90f, 180f, 270f, 0f };

    void Start()
    {
        currentStage = startStage;
        ApplyCurrentRotation();
    }

    public void RotateRuneFloor()
    {
            currentStage = (currentStage + 1) % cornerRotations.Length;
            ApplyCurrentRotation();
            Debug.LogWarning($"Rune Rotated to stage {currentStage}: {cornerRotations[currentStage]}°");
    }

    void ApplyCurrentRotation()
    {
            float targetAngle = cornerRotations[currentStage];
            pipe.localRotation = Quaternion.Euler(-90, 0, targetAngle);
    }

    public void ResetRunes()
    {
        currentStage = startStage;
        ApplyCurrentRotation();
    }
}
