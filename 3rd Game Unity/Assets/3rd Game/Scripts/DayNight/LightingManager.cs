using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //References
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    //Variables
    [SerializeField, Range(0, 24)] private float timeOfDay;
    public bool nightTime = false;

    private List<Light> allLights = new List<Light>();

    [Header("Exclusion Settings")]
    [SerializeField] private Light[] excludedLights;
    [SerializeField] private string[] excludedTags; // Array of tags to exclude
    [SerializeField] private bool excludeByTag = false; // Toggle for tag exclusion

    [Header("Time Settings")]
    [SerializeField] private float realMinutesPerGameHour = 1f;
    [SerializeField] private float timeMultiplier = 1f / 60f;


    private void Start()
    {
        RefreshLightList();
    }

    private void RefreshLightList()
    {
        Light[] foundLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        allLights.Clear();

        foreach (Light light in foundLights)
        {
            if (!IsExcluded(light))
            {
                allLights.Add(light);
            }
        }

        Debug.Log($"Found {foundLights.Length} total lights, {allLights.Count} controllable lights");

        // Optional: Log excluded tags for debugging
        if (excludeByTag && excludedTags.Length > 0)
        {
            Debug.Log($"Excluding lights with tags: {string.Join(", ", excludedTags)}");
        }
    }

    private bool IsExcluded(Light light)
    {
        // Check direct exclusion list
        foreach (Light excluded in excludedLights)
        {
            if (excluded == light)
                return true;
        }

        // Check tag exclusion if enabled
        if (excludeByTag && excludedTags != null)
        {
            foreach (string tag in excludedTags)
            {
                if (!string.IsNullOrEmpty(tag) && light.CompareTag(tag))
                    return true;
            }
        }

        return false;
    }

    public void TurnOnAllLights()
    {
        foreach (Light light in allLights)
        {
            if (light != null) // Safety check for destroyed lights
                light.enabled = true;
        }
        Debug.Log($"Turned on {allLights.Count} lights");
    }

    public void TurnOffAllLights()
    {
        foreach (Light light in allLights)
        {
            if (light != null) // Safety check for destroyed lights
                light.enabled = false;
        }
        Debug.Log($"Turned off {allLights.Count} lights");
    }

    // Public method to manually refresh the light list (useful for runtime changes)
    public void RefreshExcludedLights()
    {
        RefreshLightList();
        Debug.Log("Light list refreshed with current exclusion settings");
    }

    // Public method to add a tag to exclude at runtime
    public void AddExcludedTag(string tag)
    {
        if (excludedTags == null)
        {
            excludedTags = new string[] { tag };
        }
        else
        {
            List<string> tags = new List<string>(excludedTags);
            if (!tags.Contains(tag))
            {
                tags.Add(tag);
                excludedTags = tags.ToArray();
                RefreshLightList();
            }
        }
    }

    // Public method to remove an excluded tag
    public void RemoveExcludedTag(string tag)
    {
        if (excludedTags != null)
        {
            List<string> tags = new List<string>(excludedTags);
            if (tags.Remove(tag))
            {
                excludedTags = tags.ToArray();
                RefreshLightList();
            }
        }
    }

    private void Update()
    {
        if (preset == null)
            return;

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * timeMultiplier;
            timeOfDay %= 24; //Clamp between 0 - 24
            UpdateLighting(timeOfDay / 24f);
        }
        else
        {
            UpdateLighting(timeOfDay / 24f);
        }

        if (timeOfDay >= 0 && timeOfDay <= 6 || timeOfDay >= 18 && timeOfDay <= 24)
        {
            TurnOnAllLights();
        }
        else
        {
            TurnOffAllLights();
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void OnValidate()
    {
        if (directionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}