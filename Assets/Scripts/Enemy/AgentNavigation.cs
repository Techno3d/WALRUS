using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavigation : MonoBehaviour
{
    [SerializeField]
    private Vector3 targetPosition = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        //destination
        GetComponent<NavMeshAgent>().destination = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //change target
        targetPosition += Vector3.forward;
        GetComponent<NavMeshAgent>().destination = targetPosition;
    }
}