using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverPopUp : MonoBehaviour
{
    [SerializeField] private float popScale = 1.1f;
    [SerializeField] private float animationDuration = 0.15f;

    private Vector3 originalScale;
    private float currentTime = 0f;
    private bool isScalingUp = false;
    private bool isScalingDown = false;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isScalingUp)
        {
            currentTime += Time.deltaTime / animationDuration;
            float progress = Mathf.Clamp01(currentTime);
            // Using SmoothStep for a nice easing effect
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * popScale, easedProgress);

            if (progress >= 1f)
            {
                isScalingUp = false;
                currentTime = 0f;
            }
        }
        else if (isScalingDown)
        {
            currentTime += Time.deltaTime / animationDuration;
            float progress = Mathf.Clamp01(currentTime);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, easedProgress);

            if (progress >= 1f)
            {
                isScalingDown = false;
                currentTime = 0f;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isScalingUp = true;
        isScalingDown = false;
        currentTime = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isScalingDown = true;
        isScalingUp = false;
        currentTime = 0f;
    }
}
