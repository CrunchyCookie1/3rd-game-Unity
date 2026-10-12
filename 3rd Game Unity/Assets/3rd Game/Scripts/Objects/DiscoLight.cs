using UnityEngine;

public class DiscoLight : MonoBehaviour
{
    public float cycleSpeed = 1.0f;
    private Light _light;

    private void Start()
    {
        _light = GetComponent<Light>();
    }

    private void Update()
    {
        float hue = Mathf.Repeat(Time.time * (1.0f / cycleSpeed), 1.0f);
        _light.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
