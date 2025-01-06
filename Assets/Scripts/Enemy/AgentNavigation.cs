using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavigation : MonoBehaviour
{
    [SerializeField]
    private Vector3 targetPosition = GetComponent<Enemy>().target;
    // Start is called before the first frame update
    void Start()
    {
        //destination
        GetComponent<NavMeshAgent>().destination = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = Enemy.target;
        GetComponent<NavMeshAgent>().destination = targetPosition;
    }
}