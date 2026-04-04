using UnityEngine;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    public string sliderName;

    public void SetVolume(float sliderValue)
    {
        audioMixer.SetFloat(sliderName, Mathf.Log10(sliderValue) * 20);
    }
}
