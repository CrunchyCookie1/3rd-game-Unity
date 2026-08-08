using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;
    public bool autoStart = false;

    private List<Vector2Int> currentPath;
    private int currentTargetIndex = 0;
    private bool isMoving = false;
    private PipeGridManager gridManager;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float journeyLength;
    private float startTime;

    void Start()
    {
        // Using FindFirstObjectByType for better performance when only one instance exists
        gridManager = FindFirstObjectByType<PipeGridManager>();
        if (autoStart)
        {
            StartJourney();
        }
    }

    public void StartJourney()
    {
        if (gridManager == null) return;

        Vector2Int currentGridPos = gridManager.WorldToGrid(transform.position);
        gridManager.CheckAndMoveObject(gameObject, currentGridPos);
    }

    public void StartMoving(List<Vector2Int> path, PipeGridManager manager)
    {
        currentPath = path;
        currentTargetIndex = 0;
        gridManager = manager;
        isMoving = true;

        if (currentPath.Count > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        while (currentTargetIndex < currentPath.Count)
        {
            Vector2Int targetGridPos = currentPath[currentTargetIndex];
            targetPosition = gridManager.GridToWorld(targetGridPos);
            targetPosition.y = transform.position.y; // Keep Y consistent

            startPosition = transform.position;
            journeyLength = Vector3.Distance(startPosition, targetPosition);
            startTime = Time.time;

            // Calculate rotation
            Vector3 direction = (targetPosition - startPosition).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // Lock rotation to Z and Y axes
                targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);

                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }

            // Move to target
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                float fractionOfJourney = (Time.time - startTime) / (journeyLength / moveSpeed);
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
                yield return null;
            }

            transform.position = targetPosition;
            currentTargetIndex++;
            yield return null;
        }

        // Reached the end!
        OnReachedEnd();
        isMoving = false;
    }

    void OnReachedEnd()
    {
        Debug.Log("Object reached the end of the pipe!");
        // Add your completion logic here
    }

    public void OnPathNotFound()
    {
        isMoving = false;
        Debug.Log("Path not found! Object stopped at position: " + transform.position);
        // Add your path not found logic here
    }

    // Optional: Check if object can move before starting
    public bool CanMove()
    {
        if (gridManager == null) return false;
        Vector2Int currentPos = gridManager.WorldToGrid(transform.position);
        List<Vector2Int> path;
        return gridManager.FindPathToEnd(currentPos, out path);
    }
}