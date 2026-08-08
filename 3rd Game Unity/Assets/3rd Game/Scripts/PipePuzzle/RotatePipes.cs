using UnityEngine;

public class RotatePipes : MonoBehaviour
{
    public bool cornerPipe = false;
    public bool straightPipe = false;

    [Header("Initial Rotation Stage")]
    [Range(0, 3)] public int startStage = 0; // 0=90°, 1=180°, 2=270°, 3=0°

    public Transform pipe;

    public int currentStage = 0;
    private float[] cornerRotations = { 90f, 180f, 270f, 0f };
    private float[] straightRotations = { 90f, 0f };

    void Start()
    {
        currentStage = startStage;
        ApplyCurrentRotation();
    }

    public void RotatePipe()
    {
        if (cornerPipe)
        {
            currentStage = (currentStage + 1) % cornerRotations.Length;
            ApplyCurrentRotation();
            Debug.Log($"Corner Pipe Rotated to stage {currentStage}: {cornerRotations[currentStage]}°");
        }
        else if (straightPipe)
        {
            currentStage = (currentStage + 1) % straightRotations.Length;
            ApplyCurrentRotation();
            Debug.Log($"Straight Pipe Rotated to stage {currentStage}: {straightRotations[currentStage]}°");
        }
        else
        {
            Debug.Log("Both Straight Pipe and Corner Pipe are set to false");
        }
    }

    void ApplyCurrentRotation()
    {
        if (cornerPipe)
        {
            float targetAngle = cornerRotations[currentStage];
            pipe.localRotation = Quaternion.Euler(targetAngle, 0, 90);
        }
        else if (straightPipe)
        {
            float targetAngle = straightRotations[currentStage];
            pipe.localRotation = Quaternion.Euler(targetAngle, 0, 0);
        }
    }

    public void ResetPipe()
    {
        currentStage = startStage;
        ApplyCurrentRotation();
    }
}