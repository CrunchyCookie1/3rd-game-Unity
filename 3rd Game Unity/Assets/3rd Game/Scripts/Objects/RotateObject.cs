using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("=-= Rotation Settings =-=")]
    public bool rotateOnX = false;
    public bool rotateOnY = false;
    public bool rotateOnZ = false;

    [Header("=-= Bobbing =-=")]
    public float startY;
    public float speed = 1f;
    public float height = 0.1f;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        if (rotateOnX)
            transform.Rotate(90 * Time.deltaTime, 0, 0);
        if (rotateOnY)
            transform.Rotate(0, 90 * Time.deltaTime, 0);
        if (rotateOnZ)
            transform.Rotate(0, 0, 90 * Time.deltaTime);

        float newY = startY + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
