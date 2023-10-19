using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zone : MonoBehaviour
{


    public List<GameObject> unitsInZone;

    public string zoneName;

    float onMeshThreshold = 3;


    // Start is called before the first frame update
    void Start()
    {
        zoneName = gameObject.name;

        TerrainCollider tCollider = gameObject.GetComponent<TerrainCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "PlayerCharacter" || collision.gameObject.tag == "Enemy")
    //    {
    //        Debug.Log(collision.gameObject.name + " Enter");

    //        if(unitsInZone.Contains(collision.gameObject) == false)
    //        {
    //            unitsInZone.Add(collision.gameObject);
    //        }

    //    }
    //}

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "PlayerCharacter" || collider.gameObject.tag == "Enemy")
        {
            Debug.Log(collider.gameObject.name + " Enter");

            if (unitsInZone.Contains(collider.gameObject) == false)
            {
                unitsInZone.Add(collider.gameObject);
            }

        }
    }

    //public void OnCollisionExit(Collision other)
    //{
    //    if (other.gameObject.tag == "PlayerCharacter" || other.gameObject.tag == "Enemy")
    //    {
    //        Debug.Log(other.gameObject.name + " Exit");

    //        if (unitsInZone.Contains(other.gameObject))
    //        {
    //            unitsInZone.Remove(other.gameObject);
    //        }

    //    }
    //}

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerCharacter" || other.gameObject.tag == "Enemy")
        {
            Debug.Log(other.gameObject.name + " Exit");

            if (unitsInZone.Contains(other.gameObject))
            {
                unitsInZone.Remove(other.gameObject);
            }

        }
    }

























    ///// <summary>
    ///// Checks if a nav mesh agent is on a nav mesh surface/zone.
    ///// </summary>
    ///// <param name="agent"></param>
    ///// <returns></returns>
    //public bool IsAgentOnNavMesh(GameObject agent)
    //{
    //    Vector3 agentPosition = agent.transform.position;
    //    NavMeshHit hit;

    //    // Check for nearest point on navmesh to agent, within onMeshThreshold
    //    if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
    //    {

    //        // Check if the positions are vertically aligned
    //        if (Mathf.Approximately(agentPosition.x, hit.position.x)
    //            && Mathf.Approximately(agentPosition.z, hit.position.z))
    //        {
    //            // Lastly, check if object is below navmesh
    //            return agentPosition.y >= hit.position.y;
    //        }
    //    }

    //    return false;
    //}

    ///// <summary>
    ///// Adds agents to units in zone list
    ///// </summary>
    ///// <param name="agents"></param>
    //public void AddToUnitsInZoneList(List<GameObject> agents)
    //{
    //    for (int i = 0; i < agents.Count; i++)
    //    {
    //        //If an agent is on the zone
    //        if (IsAgentOnNavMesh(agents[i]))
    //        {
    //            //Skips units that are already in the units in zone list
    //            if (unitsInZone.Contains(agents[i]))
    //            {
    //                continue;
    //            }
    //            //Adds units into the units in zone lsit
    //            else
    //            {
    //                unitsInZone.Add(agents[i]);
    //            }
    //        }
    //    }
    //}

    //public void RemoveUnitsInZoneList(List<GameObject> agents)
    //{
    //    for (int i = 0; i < agents.Count; i++)
    //    {
    //        //If an agent is on the zone
    //        if (!IsAgentOnNavMesh(agents[i]))
    //        {
    //            //Skips units that are already in the units in zone list
    //            //if (unitsInZone.Contains(agents[i]))
    //            //{
    //                unitsInZone.Remove(agents[i]);
    //            //}

    //        }
    //    }
    //}



}
