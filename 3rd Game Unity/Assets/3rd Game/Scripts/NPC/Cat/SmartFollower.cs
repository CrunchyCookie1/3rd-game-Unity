using UnityEngine;
using UnityEngine.AI;

public class SmartFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float followRange = 15f;      // Start following when within this range
    [SerializeField] private float loseInterestRange = 25f; // Stop following when beyond this range
    [SerializeField] private float updatePathInterval = 0.2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private NavMeshAgent agent;
    private float nextUpdateTime;
    private bool isFollowing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Start following when player enters follow range
        if (distanceToPlayer <= followRange)
        {
            isFollowing = true;
        }
        // Stop following when player gets too far away
        else if (distanceToPlayer >= loseInterestRange)
        {
            isFollowing = false;
            agent.ResetPath();
        }

        // Update destination if following
        if (isFollowing && Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updatePathInterval;
            agent.SetDestination(player.position);
        }
    }

    // Optional: Visualize ranges in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);
    }
}