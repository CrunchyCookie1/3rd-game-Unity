using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EBoxSlider : MonoBehaviour
{
    public Slider slider;
    public float targetValue = 50f;
    public float tolerance = 0.01f;

    void Update()
    {
        if (Mathf.Abs(slider.value - targetValue) <= tolerance)
        {
            Debug.Log("Slider is at target value!");
        }
    }
}