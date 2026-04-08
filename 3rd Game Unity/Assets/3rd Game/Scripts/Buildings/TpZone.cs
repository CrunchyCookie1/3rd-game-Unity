using UnityEngine;

public class TpZone : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && teleportDestination != null)
        {
            Vector3 offset = other.transform.position - transform.position;

            Vector3 newPosition = teleportDestination.position + (teleportDestination.forward * offset.z) +
                                 (teleportDestination.right * offset.x) + (teleportDestination.up * offset.y);

            other.transform.position = newPosition;
            other.transform.rotation = teleportDestination.rotation;

            // Find and reset the camera manager
            CameraManager cameraManager = FindFirstObjectByType<CameraManager>();
            if (cameraManager != null)
            {
                // Reset the camera angles to face the teleport destination's forward direction
                cameraManager.lookAngle = teleportDestination.eulerAngles.y;
                cameraManager.pivotAngle = 20; // Set to your default vertical angle (adjust as needed)

                // Immediately apply the camera rotation
                cameraManager.HandleAllCameraMovement();
            }
        }
    }
}