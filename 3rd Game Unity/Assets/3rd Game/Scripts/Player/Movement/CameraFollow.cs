using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = false;

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateToTarget = true;
    [SerializeField] private float rotationSpeed = 5f;

    private bool isEnabled = true;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start()
    {
        if (target == null)
        {
            // Try to find the player if not set
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: No target assigned and no player found!");
                enabled = false;
                return;
            }
        }

        // Set initial position
        UpdateCameraPosition(true);
    }

    private void LateUpdate()
    {
        if (!isEnabled || target == null) return;

        UpdateCameraPosition(false);
    }

    private void UpdateCameraPosition(bool instant)
    {
        // Calculate desired position based on target
        Vector3 desiredPosition = target.position + offset;

        // Apply axis restrictions
        targetPosition = transform.position;

        if (followX)
            targetPosition.x = desiredPosition.x;

        if (followY)
            targetPosition.y = desiredPosition.y;

        if (followZ)
            targetPosition.z = desiredPosition.z;

        // Move camera
        if (instant)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        // Handle rotation
        if (rotateToTarget && target != null)
        {
            targetRotation = Quaternion.LookRotation(target.position - transform.position);

            if (instant)
            {
                transform.rotation = targetRotation;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Public methods to control the camera follow behavior
    public void EnableCameraFollow()
    {
        isEnabled = true;
        Debug.Log("Camera follow enabled");
    }

    public void DisableCameraFollow()
    {
        isEnabled = false;
        Debug.Log("Camera follow disabled");
    }

    public void ToggleCameraFollow()
    {
        isEnabled = !isEnabled;
        Debug.Log($"Camera follow {(isEnabled ? "enabled" : "disabled")}");
    }

    // Methods to control which axes are followed
    public void SetFollowX(bool follow)
    {
        followX = follow;
    }

    public void SetFollowY(bool follow)
    {
        followY = follow;
    }

    public void SetFollowZ(bool follow)
    {
        followZ = follow;
    }

    public void SetFollowAxes(bool x, bool y, bool z)
    {
        followX = x;
        followY = y;
        followZ = z;
    }

    // Method to change target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Method to change offset
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    // Method to instantly snap camera to target
    public void SnapToTarget()
    {
        if (target != null)
        {
            UpdateCameraPosition(true);
        }
    }

    // Property to check if camera follow is enabled
    public bool IsEnabled => isEnabled;
}