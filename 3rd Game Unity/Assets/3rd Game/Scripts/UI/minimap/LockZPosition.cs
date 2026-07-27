using UnityEngine;

public class LockZPosition : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 position = transform.position;
        position.z = 0f; // Lock the Z position to 0
        transform.position = position;
    }
}
