using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderAtMax : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private UnityEvent onSliderAtMax;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider component not found on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (slider != null && slider.value >= slider.maxValue)
        {
            onSliderAtMax.Invoke();
            audioSource.PlayOneShot(clip);
            Debug.Log("Slider is at maximum value!");
        }
    }
}
