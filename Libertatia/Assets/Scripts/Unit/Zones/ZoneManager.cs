using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ZoneManager : MonoBehaviour
{
    public List<GameObject> crewMembers;
    public List<GameObject> enemies;
    public List<GameObject> enemyHouses;

    public List<Terrain> zones;

    public GameObject shipWaypoint;

    public ResourcesUI resourceUI;

    public GameObject combatUI;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();
        enemyHouses = GameObject.FindGameObjectsWithTag("EnemyHouse").ToList<GameObject>();
        combatUI = GameObject.FindGameObjectWithTag("CombatUI");
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
                resourceUI.UpdateCrewAmountUI(crewMembers.Count - 1);
            }
        }

        //checks the number of objects in a zone when clicking on it
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.tag == "Zone")
                {
                    Zone zone = hit.transform.gameObject.GetComponent<Zone>();

                    Debug.Log("Crew Members: " + zone.crewMembersInZone.Count + " Enemies: " + zone.enemiesInZone.Count + " Houses: " + zone.housesInZone.Count);
                }
            }
        }

        //Obtain loot from zones after beating the enemies, if there are resource deposits
        foreach(Terrain terrain in zones)
        {
            Zone zone = terrain.GetComponent<Zone>();

            //if there is a resource depot & the zone has at least 1 crew member & the zone has no enemies left
            if(zone.housesInZone.Count >= 1 && zone.crewMembersInZone.Count >= 1 && zone.enemiesInZone.Count <= 0)
            {
                //if the loot has not been collected yet
                if(zone.zoneLootCollected == false)
                {

                    CombatResourcesUI combatResources = combatUI.GetComponent<CombatResourcesUI>();

                    foreach (GameObject enemyHouse in zone.housesInZone)
                    {
                        EnemyHouse house = enemyHouse.GetComponent<EnemyHouse>();

                        //obtain wood from the houses
                        combatResources.woodAmount += house.lootValue;
                        combatResources.UpdateWoodUI(combatResources.woodAmount);

                        house.CreatePopUpText();
                    }
       

                    zone.zoneLootCollected = true;
                }
            }
        }
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
