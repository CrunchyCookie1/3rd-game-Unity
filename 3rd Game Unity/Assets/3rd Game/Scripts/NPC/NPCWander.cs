using UnityEngine;
using UnityEngine.AI;

public class NPCWander : NPCComponent
{
    public Area area;


    enum EState
    {
        Wandering,
        Waiting
    }

    [SerializeField] float maxWaitTime = 3f;
    [SerializeField] float maxWaitTimeRandom = 5f;

    [Space(15f)] float maxWanderTime = 5f;

    float currentMaxWaitTime = 3f;

    [Header("Debugging")]
    [SerializeField] EState state = EState.Wandering;
    [SerializeField] private float waitTime = 0f;
    [SerializeField] private float wanderTime = 0f;

    private void Start()
    {
        if (Random.Range(0f, 100.0f) > 50f)
        {
            ChangeState(EState.Wandering);
        }
        else
        {
            ChangeState(EState.Waiting);
        }
    }

    private void Update()
    {
        if (state == EState.Waiting)
        {
            waitTime -= Time.deltaTime;

            if (waitTime < 0f)
            {
                ChangeState(EState.Wandering);
            }
        }
        else if (state == EState.Wandering)
        {
            wanderTime -= Time.deltaTime;

            if (HasArrived() || wanderTime < 0f)
            {
                ChangeState(EState.Waiting);
            }
        }
    }

    void ChangeState(EState newState)
    {
        state = newState;

        if (state == EState.Wandering)
        {
            npc.agent.isStopped = false;

            SetRandomDestination();

            wanderTime = maxWanderTime;
        }
        else if (state == EState.Waiting)
        {
            waitTime = maxWaitTime + Random.Range(0f, maxWaitTimeRandom);

            npc.agent.isStopped = true;
        }
    }

    bool HasArrived()
    {
        return npc.agent.remainingDistance <= npc.agent.stoppingDistance;
    }

    void SetRandomDestination()
    {
        npc.agent.SetDestination(area.GetRandomPoint());
    }
}
