using UnityEngine;
using UnityEngine.AI;

public class GhostMove : MonoBehaviour
{
    public NavMeshAgent agent; // Reference to the NavMeshAgent component
    private  Transform target; // Target for the ghost to move towards
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Camera.main.transform; // Set the target to the main camera's transform
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component if not assigned
        }
        if (target != null)
        {
            agent.SetDestination(target.position); // Set the destination to the target's position
        }
    }

    // Update is called once per frame
    void Update()
    {
        target = Camera.main.transform; // Continuously update the target to the main camera's transform
        if (target != null)
        {
            agent.SetDestination(target.position); // Continuously update the destination to the target's position
        }
        
    }
}
