using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool startVisible = true;
    public UnityEvent onFadeEndEvent;

    private Color originalColor;
    private Coroutine activeFadeCoroutine;

    void Start()
    {

        if (rawImage == null)
            rawImage = GetComponent<RawImage>();


        originalColor = rawImage.color;


        if (!startVisible)
            SetAlpha(0f);
    }


    public void FadeIn()
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        activeFadeCoroutine = StartCoroutine(FadeAlpha(0f, 1f, fadeDuration));
    }

    public void FadeOut()
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        activeFadeCoroutine = StartCoroutine(FadeAlpha(1f, 0f, fadeDuration));
    }

    public void FadeTo(float targetAlpha)
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        float startAlpha = rawImage.color.a;
        activeFadeCoroutine = StartCoroutine(FadeAlpha(startAlpha, targetAlpha, fadeDuration));
    }

    public void FadeTo(float targetAlpha, float duration)
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        float startAlpha = rawImage.color.a;
        activeFadeCoroutine = StartCoroutine(FadeAlpha(startAlpha, targetAlpha, duration));
    }

    public void SetAlphaImmediate(float alpha)
    {
        if (activeFadeCoroutine != null)
        {
            StopCoroutine(activeFadeCoroutine);
            activeFadeCoroutine = null;
        }

        SetAlpha(alpha);
    }

    public bool IsFading()
    {
        return activeFadeCoroutine != null;
    }


    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetAlpha(currentAlpha);
            yield return null;
        }

        SetAlpha(endAlpha);
        activeFadeCoroutine = null;

        onFadeEndEvent.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = rawImage.color;
        newColor.a = alpha;
        rawImage.color = newColor;
    }
}