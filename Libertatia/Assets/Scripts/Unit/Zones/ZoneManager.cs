using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ZoneManager : MonoBehaviour
{
    public List<GameObject> crewMembers;
    public List<GameObject> enemies;

    public List<Terrain> zones;

    public GameObject shipWaypoint;

    public CombatUI combatUI;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();

        zones = Terrain.activeTerrains.ToList();


    }

    // Update is called once per frame
    void Update()
    {
        if (crewMembers.Count == 0)
        {
            crewMembers = GameObject.FindGameObjectsWithTag("PlayerCharacter").ToList<GameObject>();
        }

        foreach(GameObject cm in crewMembers)
        {
            CrewMember crewMember = cm.GetComponent<CrewMember>();

            //Decreases crewmate number by 1 if they died
            if(crewMember.currentHealth <= 0)
            {
                combatUI.UpdateCrewAmountUI(crewMembers.Count - 1);
            }
        }




        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.tag == "Zone")
                {
                    Zone zone = hit.transform.gameObject.GetComponent<Zone>();

                    Debug.Log(zone.crewMembersInZone.Count + zone.enemiesInZone.Count);
                }
            }
        }

            //for (int i = 0; i < zones.Count; i++)
            //{
            //    Zone terrainChunk = zones[i].GetComponent<Zone>();

            //    for(int j = 0; j < crewMembers.Count; j++)
            //    {
            //        terrainChunk.AddToUnitsInZoneList(crewMembers);
            //    }

            //    for(int k = 0; k < enemies.Count; k++)
            //    {
            //        terrainChunk.AddToUnitsInZoneList(enemies);
            //    }

            //}

    }


    /// <summary>
    /// Makes the crew members retreat back to the ship's waypoint game object
    /// </summary>
    public void Retreat()
    {
        for(int i = 0; i < crewMembers.Count; i++)
        {
            CrewMember crewMember = crewMembers[i].GetComponent<CrewMember>();

            crewMember.charAgent.SetDestination(shipWaypoint.transform.position);
        }
    }
}
