using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zone : MonoBehaviour
{
    public List<GameObject> crewMembers;
    public List<GameObject> enemies;

    public List<GameObject> unitsInZone;

    public string zoneName;

    float onMeshThreshold = 3;


    // Start is called before the first frame update
    void Start()
    {
      
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();

        zoneName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if(crewMembers.Count == 0)
        {
            crewMembers = GameObject.FindGameObjectsWithTag("PlayerCharacter").ToList<GameObject>();
        }

        AddToUnitsInZoneList(crewMembers);
        AddToUnitsInZoneList(enemies);

        RemoveUnitsInZoneList(crewMembers);
        RemoveUnitsInZoneList(enemies);

    }

    /// <summary>
    /// Checks if a nav mesh agent is on a nav mesh surface/zone.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public bool IsAgentOnNavMesh(GameObject agent)
    {
        Vector3 agentPosition = agent.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.GetAreaFromName("Walkable")))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return agentPosition.y >= hit.position.y;
            }
        }

        return false;
    }

    /// <summary>
    /// Adds agents to units in zone list
    /// </summary>
    /// <param name="agents"></param>
    public void AddToUnitsInZoneList(List<GameObject> agents)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            //If an agent is on the zone
            if (IsAgentOnNavMesh(agents[i]))
            {
                //Skips units that are already in the units in zone list
                if (unitsInZone.Contains(agents[i]))
                {
                    continue;
                }
                //Adds units into the units in zone lsit
                else
                {
                    unitsInZone.Add(agents[i]);
                }
            }
        }
    }

    public void RemoveUnitsInZoneList(List<GameObject> agents)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            //If an agent is on the zone
            if (!IsAgentOnNavMesh(agents[i]))
            {
                //Skips units that are already in the units in zone list
                //if (unitsInZone.Contains(agents[i]))
                //{
                    unitsInZone.Remove(agents[i]);
                //}

            }
        }
    }
}
