using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class EBox : MonoBehaviour
{
    public InputManager inputManager;
    public OpenUI openUI;

    public Slider slider1;
    public Slider slider2;
    public Slider slider3;

    public UnityEvent correctValueEvent;
    public UnityEvent incorrectValueEvent;

    public UnityEvent slider1CorrectEvent;
    public UnityEvent slider2CorrectEvent;
    public UnityEvent slider3CorrectEvent;

    public UnityEvent soundsCorrectEvent;

    public float correctValue1 = 50f;
    public float correctValue2 = 75f;
    public float correctValue3 = 100f;
    public float tolerance = 0.01f;

    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    public GameObject[] objectsToEnableOnCorrect;
    public GameObject[] objectsToDisableOnCorrect;
    public GameObject[] objectsToEnableOnIncorrect;
    public GameObject[] objectsToDisableOnIncorrect;

    private bool slider1WasCorrect = false;
    private bool slider2WasCorrect = false;
    private bool slider3WasCorrect = false;

    private bool lastCorrectState = false;

    void Start()
    {
        // Add listeners to detect when sliders change
        if (slider1 != null) slider1.onValueChanged.AddListener(OnSlider1Changed);
        if (slider2 != null) slider2.onValueChanged.AddListener(OnSlider2Changed);
        if (slider3 != null) slider3.onValueChanged.AddListener(OnSlider3Changed);
    }

    void OnSlider1Changed(float value)
    {
        bool isNowCorrect = Mathf.Abs(value - correctValue1) <= tolerance;

        // Check if this slider just became correct
        if (isNowCorrect && !slider1WasCorrect)
        {
            Debug.Log("Slider 1 is at the correct value: " + value);
            PlaySound(correctSound);
            slider1CorrectEvent?.Invoke();
            correctValueEvent?.Invoke();
        }
        // Check if this slider was correct but now isn't
        else if (!isNowCorrect && slider1WasCorrect)
        {
            Debug.Log("Slider 1 moved away from correct value");
            incorrectValueEvent?.Invoke();
        }

        slider1WasCorrect = isNowCorrect;

        // Check all sliders for the overall state
        TestAllSliders();
    }

    void OnSlider2Changed(float value)
    {
        bool isNowCorrect = Mathf.Abs(value - correctValue2) <= tolerance;

        // Check if this slider just became correct
        if (isNowCorrect && !slider2WasCorrect)
        {
            Debug.Log("Slider 2 is at the correct value: " + value);
            PlaySound(correctSound);
            slider2CorrectEvent?.Invoke();
            correctValueEvent?.Invoke();
        }
        // Check if this slider was correct but now isn't
        else if (!isNowCorrect && slider2WasCorrect)
        {
            Debug.Log("Slider 2 moved away from correct value");
            incorrectValueEvent?.Invoke();
        }

        slider2WasCorrect = isNowCorrect;

        // Check all sliders for the overall state
        TestAllSliders();
    }

    void OnSlider3Changed(float value)
    {
        bool isNowCorrect = Mathf.Abs(value - correctValue3) <= tolerance;

        // Check if this slider just became correct
        if (isNowCorrect && !slider3WasCorrect)
        {
            Debug.Log("Slider 3 is at the correct value: " + value);
            PlaySound(correctSound);
            slider3CorrectEvent?.Invoke();
            correctValueEvent?.Invoke();
        }
        // Check if this slider was correct but now isn't
        else if (!isNowCorrect && slider3WasCorrect)
        {
            Debug.Log("Slider 3 moved away from correct value");
            incorrectValueEvent?.Invoke();
        }

        slider3WasCorrect = isNowCorrect;

        // Check all sliders for the overall state
        TestAllSliders();
    }

    public void TestAllSliders()
    {
        bool allCorrect = AreAllSlidersCorrect();

        // Only trigger if the state has changed
        if (allCorrect != lastCorrectState)
        {
            lastCorrectState = allCorrect;

            if (allCorrect)
            {
                OnAllSlidersCorrect();
            }
            else
            {
                OnSlidersIncorrect();
            }
        }
    }

    public bool AreAllSlidersCorrect()
    {
        return slider1WasCorrect && slider2WasCorrect && slider3WasCorrect;
    }

    private void OnAllSlidersCorrect()
    {
        Debug.Log("ALL sliders are correct! Playing success sound and enabling/disabling objects.");

        EnableObjects(objectsToEnableOnCorrect);
        DisableObjects(objectsToDisableOnCorrect);

        EnableObjects(objectsToEnableOnIncorrect, false);
        DisableObjects(objectsToDisableOnIncorrect, false);

        soundsCorrectEvent?.Invoke();

        // FORCE CLOSE UI AND UNFREEZE THE GAME
        if (openUI != null)
        {
            openUI.ForceCloseAndUnfreeze();
            Debug.Log("UI forced closed and game unfrozen!");
        }
        else
        {
            Debug.LogWarning("OpenUI reference not set in EBox!");
        }

        // Also ensure InputManager is enabled as backup
        if (inputManager != null)
        {
            inputManager.EnablePlayerControls();
            Debug.Log("Player input enabled - Game unfrozen!");
        }
    }

    private void OnSlidersIncorrect()
    {
        Debug.Log("Sliders are NOT all correct. Playing fail sound and enabling/disabling objects.");

        // Play incorrect sound
        PlaySound(incorrectSound);

        // Invoke the UnityEvent for incorrect values
        incorrectValueEvent?.Invoke();

        // Enable objects for incorrect state
        EnableObjects(objectsToEnableOnIncorrect);
        DisableObjects(objectsToDisableOnIncorrect);

        // Clean up correct state objects
        EnableObjects(objectsToEnableOnCorrect, false);
        DisableObjects(objectsToDisableOnCorrect, false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else if (clip == null)
        {
            Debug.LogWarning("Sound clip is missing!");
        }
        else if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is missing! Add an AudioSource component to this GameObject.");
        }
    }

    private void EnableObjects(GameObject[] objects, bool enable = true)
    {
        if (objects == null) return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(enable);
            }
        }
    }

    private void DisableObjects(GameObject[] objects, bool disable = true)
    {
        if (objects == null) return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(!disable);
            }
        }
    }

    public void ForceCheckSliders()
    {
        TestAllSliders();
    }

    public string GetSlidersStatus()
    {
        return $"Slider1: {slider1.value}/{correctValue1}, " +
               $"Slider2: {slider2.value}/{correctValue2}, " +
               $"Slider3: {slider3.value}/{correctValue3}";
    }

    public void ResetSliderStates()
    {
        slider1WasCorrect = false;
        slider2WasCorrect = false;
        slider3WasCorrect = false;
        lastCorrectState = false;
    }
}