using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour
{
    NavMeshAgent agent;
    bool isResource;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
    }
    public WaitUntil WaitForNavMesh()
    {
        return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
    }



}
