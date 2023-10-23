using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public List<GameObject> crewMembers;
    public List<GameObject> enemies;

    public List<Terrain> zones;

    public GameObject shipWaypoint;

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

    public void Retreat()
    {
        for(int i = 0; i < crewMembers.Count; i++)
        {
            CrewMember crewMember = crewMembers[i].GetComponent<CrewMember>();

            crewMember.charAgent.SetDestination(shipWaypoint.transform.position);
        }
    }
}
