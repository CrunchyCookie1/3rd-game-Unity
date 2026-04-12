using UnityEngine;

public class TpFunction : MonoBehaviour
{
    public Transform target;

    public void TeleportToTarget()
    {
        if (target != null)
        {
            transform.position = target.position;
            Debug.Log($"Teleported to {target.position}");
        }
        else
        {
            Debug.LogWarning("Target transform is not assigned.");
        }
    }
}
