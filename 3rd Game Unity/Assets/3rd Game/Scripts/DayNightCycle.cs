using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light light;
    public float rotationSpeed;

    private void Update()
    {
        light.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}
