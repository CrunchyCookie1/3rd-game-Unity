using UnityEngine;

public class TpObjectFacing : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [SerializeField] private float lineLength = 2f;
    [SerializeField] private Color forwardColor = Color.blue;
    [SerializeField] private Color rightColor = Color.red;
    [SerializeField] private Color upColor = Color.green;
    [SerializeField] private bool alwaysDrawGizmos = true;

    private void OnDrawGizmos()
    {
        if (!alwaysDrawGizmos) return;
        DrawDirectionGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (alwaysDrawGizmos) return;
        DrawDirectionGizmos();
    }

    private void DrawDirectionGizmos()
    {
        // Draw forward direction (blue)
        Gizmos.color = forwardColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * lineLength);

        // Draw a small sphere at the end of forward line
        Gizmos.DrawWireSphere(transform.position + transform.forward * lineLength, 0.1f);

        // Draw right direction (red)
        Gizmos.color = rightColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * lineLength);

        // Draw up direction (green)
        Gizmos.color = upColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * lineLength);

        // Draw a sphere at the object's position
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        // Draw a label showing which direction is forward
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + transform.forward * (lineLength + 0.2f), "Forward");
        UnityEditor.Handles.Label(transform.position + transform.right * (lineLength + 0.2f), "Right");
        UnityEditor.Handles.Label(transform.position + transform.up * (lineLength + 0.2f), "Up");
#endif
    }
}