using UnityEngine;
using UnityEngine.Events;

public class VapeShopEBox : MonoBehaviour
{
    [SerializeField] private float value;
    [SerializeField] private float finaleValue = 10f;
    [SerializeField] private UnityEvent onFinalValueReached;

    public void AddValue(float amount = 1)
    {
        value += amount;
        if (value >= finaleValue)
        {
            value = finaleValue;
            OnFinalValueReached();
        }
    }

    public void RemoveValue(float amount = 1)
    {
        value -= amount;
        if (value < 0)
        {
            value = 0;
        }
    }

    private void OnFinalValueReached()
    {
        onFinalValueReached.Invoke();
        Debug.Log("Final value reached!");
    }
}
