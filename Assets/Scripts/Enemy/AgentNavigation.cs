using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentNavigation : MonoBehaviour
{
    [SerializeField]
    private Enemy enem;

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (enem != null)
        {
            targetPosition = enem.target; // Initialize targetPosition from enem
            GetComponent<NavMeshAgent>().destination = targetPosition;
        }
        else
        {
            Debug.LogError("Enemy (enem) is not assigned in the Inspector!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enem != null)
        {
            targetPosition = enem.target;
            GetComponent<NavMeshAgent>().destination = targetPosition;
        }
    }
}
