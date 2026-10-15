using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject endPoint;
    public GameObject startPoint;
    public Transform objectToMove;

    public bool pingPong = false;
    public bool startOnAwake = false;
    public float speed = 1.0f;

    private bool startMoving = false;
    private bool movingTowardEnd = true;
    private bool hasReachedTarget = false;

    private void Start()
    {
        if (startOnAwake)
        {
            StartMoving();
        }
    }

    private void Update()
    {
        if (!startMoving) return;

        Transform currentTarget;

        if (pingPong)
        {
            currentTarget = movingTowardEnd ? endPoint.transform : startPoint.transform;
        }
        else
        {
            currentTarget = endPoint.transform;
        }

        objectToMove.position = Vector3.MoveTowards(
            objectToMove.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(objectToMove.position, currentTarget.position) < 0.01f)
        {
            if (pingPong)
            {
                movingTowardEnd = !movingTowardEnd;
                hasReachedTarget = true;
            }
            else
            {
                startMoving = false;
                hasReachedTarget = true;
            }
        }
    }

    public void StartMoving()
    {
        startMoving = true;
        hasReachedTarget = false;
        movingTowardEnd = true;
    }

    public void StopMoving()
    {
        startMoving = false;
    }
    public void ResetToStart()
    {
        if (startPoint != null)
        {
            objectToMove.position = startPoint.transform.position;
            movingTowardEnd = true;
            hasReachedTarget = false;
            startMoving = false;
        }
    }

    public bool IsMovementComplete()
    {
        return hasReachedTarget && !pingPong;
    }
}