using UnityEngine;
using UnityEngine.Events;

public class ObjectCounter : MonoBehaviour
{
    public int targetCount = 5;
    public int currentCount = 0;

    public UnityEvent onTargetReached;
    public UnityEvent onCountGoUp;

    public void IncrementCount()
    {
        currentCount++;
        onCountGoUp.Invoke();
        if (currentCount >= targetCount)
        {
            onTargetReached.Invoke();
        }
    }
}
