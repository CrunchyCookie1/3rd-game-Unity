using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider lookSensitivitySlider;
    [SerializeField] private Slider pivotSensitivitySlider;
    [SerializeField] private TextMeshProUGUI lookSensitivityValueText;
    [SerializeField] private TextMeshProUGUI pivotSensitivityValueText;
    [SerializeField] private TextMeshProUGUI lookSliderLabelText;
    [SerializeField] private TextMeshProUGUI pivotSliderLabelText;

    [Header("Sensitivity Settings")]
    [SerializeField] private float minSensitivity = 0.5f;
    [SerializeField] private float maxSensitivity = 5f;
    [SerializeField] private float defaultLookSensitivity = 2f;
    [SerializeField] private float defaultPivotSensitivity = 2f;

    [Header("Runtime References")]
    [SerializeField] private CameraManager cameraManager;

    private const string LOOK_SENSITIVITY_PREF_KEY = "CameraLookSpeed";
    private const string PIVOT_SENSITIVITY_PREF_KEY = "CameraPivotSpeed";
    private float currentLookSensitivity;
    private float currentPivotSensitivity;

    private void Awake()
    {
        LoadSensitivities();
        FindCameraManager();
        SetupSliders();
    }

    private void FindCameraManager()
    {
        if (cameraManager == null)
        {
            cameraManager = FindAnyObjectByType<CameraManager>();

            if (cameraManager == null)
            {
                Debug.LogError("SensManager: CameraManager not found in the scene!");
            }
        }
    }

    private void SetupSliders()
    {
        SetupLookSlider();
        SetupPivotSlider();
    }

    private void SetupLookSlider()
    {
        if (lookSensitivitySlider != null)
        {
            lookSensitivitySlider.minValue = minSensitivity;
            lookSensitivitySlider.maxValue = maxSensitivity;
            lookSensitivitySlider.value = currentLookSensitivity;

            lookSensitivitySlider.onValueChanged.AddListener(OnLookSensitivityChanged);

            UpdateLookSensitivityDisplay(currentLookSensitivity);
        }
        else
        {
            Debug.LogWarning("SensManager: Look sensitivity slider not assigned in inspector!");
        }
    }

    private void SetupPivotSlider()
    {
        if (pivotSensitivitySlider != null)
        {
            pivotSensitivitySlider.minValue = minSensitivity;
            pivotSensitivitySlider.maxValue = maxSensitivity;
            pivotSensitivitySlider.value = currentPivotSensitivity;

            pivotSensitivitySlider.onValueChanged.AddListener(OnPivotSensitivityChanged);

            UpdatePivotSensitivityDisplay(currentPivotSensitivity);
        }
        else
        {
            Debug.LogWarning("SensManager: Pivot sensitivity slider not assigned in inspector!");
        }
    }

    private void OnLookSensitivityChanged(float newValue)
    {
        currentLookSensitivity = newValue;
        ApplyLookSensitivity();
        SaveLookSensitivity();
        UpdateLookSensitivityDisplay(newValue);
    }

    private void OnPivotSensitivityChanged(float newValue)
    {
        currentPivotSensitivity = newValue;
        ApplyPivotSensitivity();
        SavePivotSensitivity();
        UpdatePivotSensitivityDisplay(newValue);
    }

    private void ApplyLookSensitivity()
    {
        if (cameraManager != null)
        {
            cameraManager.cameraLookSpeed = currentLookSensitivity;
        }
        else
        {
            Debug.LogWarning("SensManager: Cannot apply look sensitivity - CameraManager is null!");
        }
    }

    private void ApplyPivotSensitivity()
    {
        if (cameraManager != null)
        {
            cameraManager.cameraPivotSpeed = currentPivotSensitivity;
        }
        else
        {
            Debug.LogWarning("SensManager: Cannot apply pivot sensitivity - CameraManager is null!");
        }
    }

    private void ApplyAllSensitivities()
    {
        ApplyLookSensitivity();
        ApplyPivotSensitivity();
    }

    private void SaveLookSensitivity()
    {
        PlayerPrefs.SetFloat(LOOK_SENSITIVITY_PREF_KEY, currentLookSensitivity);
        PlayerPrefs.Save();
    }

    private void SavePivotSensitivity()
    {
        PlayerPrefs.SetFloat(PIVOT_SENSITIVITY_PREF_KEY, currentPivotSensitivity);
        PlayerPrefs.Save();
    }

    private void LoadSensitivities()
    {
        // Load look sensitivity
        if (PlayerPrefs.HasKey(LOOK_SENSITIVITY_PREF_KEY))
        {
            currentLookSensitivity = PlayerPrefs.GetFloat(LOOK_SENSITIVITY_PREF_KEY);
        }
        else
        {
            currentLookSensitivity = defaultLookSensitivity;
        }

        // Load pivot sensitivity
        if (PlayerPrefs.HasKey(PIVOT_SENSITIVITY_PREF_KEY))
        {
            currentPivotSensitivity = PlayerPrefs.GetFloat(PIVOT_SENSITIVITY_PREF_KEY);
        }
        else
        {
            currentPivotSensitivity = defaultPivotSensitivity;
        }
    }

    private void UpdateLookSensitivityDisplay(float value)
    {
        if (lookSensitivityValueText != null)
        {
            lookSensitivityValueText.text = value.ToString("F1");
        }

        if (lookSliderLabelText != null)
        {
            lookSliderLabelText.text = "Look Sensitivity";
        }
    }

    private void UpdatePivotSensitivityDisplay(float value)
    {
        if (pivotSensitivityValueText != null)
        {
            pivotSensitivityValueText.text = value.ToString("F1");
        }

        if (pivotSliderLabelText != null)
        {
            pivotSliderLabelText.text = "Pivot Sensitivity";
        }
    }

    // Public methods for look sensitivity
    public void SetLookSensitivity(float newSensitivity)
    {
        newSensitivity = Mathf.Clamp(newSensitivity, minSensitivity, maxSensitivity);

        if (lookSensitivitySlider != null)
        {
            lookSensitivitySlider.value = newSensitivity;
        }
        else
        {
            currentLookSensitivity = newSensitivity;
            ApplyLookSensitivity();
            SaveLookSensitivity();
        }
    }

    public float GetLookSensitivity()
    {
        return currentLookSensitivity;
    }

    // Public methods for pivot sensitivity
    public void SetPivotSensitivity(float newSensitivity)
    {
        newSensitivity = Mathf.Clamp(newSensitivity, minSensitivity, maxSensitivity);

        if (pivotSensitivitySlider != null)
        {
            pivotSensitivitySlider.value = newSensitivity;
        }
        else
        {
            currentPivotSensitivity = newSensitivity;
            ApplyPivotSensitivity();
            SavePivotSensitivity();
        }
    }

    public float GetPivotSensitivity()
    {
        return currentPivotSensitivity;
    }

    // Reset methods
    public void ResetLookToDefault()
    {
        SetLookSensitivity(defaultLookSensitivity);
    }

    public void ResetPivotToDefault()
    {
        SetPivotSensitivity(defaultPivotSensitivity);
    }

    public void ResetAllToDefault()
    {
        SetLookSensitivity(defaultLookSensitivity);
        SetPivotSensitivity(defaultPivotSensitivity);
    }

    private void Start()
    {
        if (cameraManager != null)
        {
            ApplyAllSensitivities();
        }
    }

    // Registration methods for dynamic UI creation
    public void RegisterLookSlider(Slider newSlider)
    {
        lookSensitivitySlider = newSlider;
        SetupLookSlider();
    }

    public void RegisterPivotSlider(Slider newSlider)
    {
        pivotSensitivitySlider = newSlider;
        SetupPivotSlider();
    }
}