using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class AiController : MonoBehaviour
{
    private GameObject destination;
    private NavMeshAgent agent;

    private void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Player");

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agent.SetDestination(destination.transform.position);
    }
}
