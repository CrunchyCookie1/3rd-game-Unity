using UnityEngine;
using UnityEngine.AI;

public class AdvancedPathFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public int stopAtWaypointIndex = 2;
    public bool stopAtWaypoint = true;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private Vector3[] corners;
    private bool hasReachedEnd = false;
    private bool isWaitingAtStopPoint = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        agent.angularSpeed = 120f;
        agent.acceleration = 8f;

        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    void Update()
    {
        if (hasReachedEnd || isWaitingAtStopPoint) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextWaypoint();
        }

        if (agent.velocity.magnitude > 0.1f && !hasReachedEnd)
        {
            Vector3 direction = agent.steeringTarget - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void DontStopAtWaypoint()
    {
        stopAtWaypoint = false;
        if (isWaitingAtStopPoint)
        {
            ContinueFromStopPoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        if (currentWaypointIndex == stopAtWaypointIndex && stopAtWaypoint == true)
        {
            StopAtWaypoint();
            return;
        }

        if (currentWaypointIndex >= waypoints.Length)
        {
            StopMoving();
            return;
        }

        agent.destination = waypoints[currentWaypointIndex].position;

        Debug.Log($"Moving to waypoint {currentWaypointIndex + 1} of {waypoints.Length}");

        if (agent.hasPath)
        {
            corners = agent.path.corners;
            Debug.Log($"Path has {corners.Length} corners");
        }

        currentWaypointIndex++;
    }

    void StopAtWaypoint()
    {
        isWaitingAtStopPoint = true;
        hasReachedEnd = false;
        agent.isStopped = true;
        agent.ResetPath();

        Debug.Log($"Reached stop waypoint {stopAtWaypointIndex + 1}! NPC has stopped.");

        OnReachedStopPoint();
    }

    public void ContinueFromStopPoint()
    {
        if (isWaitingAtStopPoint)
        {
            isWaitingAtStopPoint = false;
            agent.isStopped = false;

            currentWaypointIndex++;
            MoveToNextWaypoint();

            Debug.Log("NPC continuing from stop point...");
        }
    }

    public void ResumeToEnd()
    {
        if (isWaitingAtStopPoint)
        {
            isWaitingAtStopPoint = false;
            agent.isStopped = false;

            MoveToNextWaypoint();
        }
    }

    public void StopMoving()
    {
        hasReachedEnd = true;
        isWaitingAtStopPoint = false;
        agent.isStopped = true;
        agent.ResetPath();

        Debug.Log("Reached final waypoint! NPC has stopped.");
    }

    public void ContinueMoving()
    {
        if (hasReachedEnd)
        {
            hasReachedEnd = false;
            agent.isStopped = false;
            if (currentWaypointIndex < waypoints.Length)
            {
                MoveToNextWaypoint();
            }
        }
    }

    public void RestartPath()
    {
        currentWaypointIndex = 0;
        hasReachedEnd = false;
        isWaitingAtStopPoint = false;
        agent.isStopped = false;

        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    void OnReachedStopPoint()
    {
        Debug.Log("Custom behavior at stop point!");
    }

    public bool HasReachedEnd()
    {
        return hasReachedEnd;
    }

    public bool IsAtStopPoint()
    {
        return isWaitingAtStopPoint;
    }

    void OnDrawGizmos()
    {
        if (corners != null && corners.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }

        if (waypoints != null)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    if (i == stopAtWaypointIndex)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);
                        Gizmos.color = new Color(1, 0, 0, 0.3f);
                        Gizmos.DrawSphere(waypoints[i].position, 0.5f);
                    }
                    else
                    {
                        Gizmos.color = (i < currentWaypointIndex) ? Color.gray : Color.white;
                        Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
                    }

                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        if (i < stopAtWaypointIndex)
                        {
                            Gizmos.color = Color.yellow;
                        }
                        else
                        {
                            Gizmos.color = Color.gray;
                        }
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                }
            }
        }
    }
}