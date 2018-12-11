using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunController : MonoBehaviour {

    private Transform target;
    private NavMeshAgent navAgent;

    public void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        navAgent.SetDestination(target.position);
    }
}
