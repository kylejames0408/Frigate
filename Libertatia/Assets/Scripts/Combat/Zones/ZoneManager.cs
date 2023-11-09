using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ZoneManager : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;

    public List<GameObject> crewMembers;
    public List<GameObject> enemies;
    public List<GameObject> enemyHouses;

    public List<Terrain> zones;

    public GameObject shipWaypoint;

    //public ResourcesUI resourceUI;

    public GameObject combatUI;

    [SerializeField]
    public GameEvent finishedCombat;
    private bool finishedCombatBool;

    private void Awake()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();
        enemyHouses = GameObject.FindGameObjectsWithTag("EnemyHouse").ToList<GameObject>();
        combatUI = GameObject.FindGameObjectWithTag("CombatUI");
        zones = Terrain.activeTerrains.ToList();
        for(int i = 0; i < zones.Count; i++)
        {
            if (zones[i].tag != "Zone")
            {
                zones.RemoveAt(i);
                i--;
            }
        }
        finishedCombatBool = false;
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
                //resourceUI.UpdateCrewAmountUI(crewMembers.Count - 1);
                CombatResourcesUI combatResourcesUI = combatUI.GetComponent<CombatResourcesUI>();
                combatResourcesUI.UpdateCrewAmountUI(crewMembers.Count - 1);
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

            if (zone.housesInZone.Count == 0 && zone.crewMembersInZone.Count >= 1 && zone.enemiesInZone.Count <= 0)
            {
                zone.zoneLootCollected = true;
            }

            //If there is at least one crew member and enemy in a zone
            if (zone.crewMembersInZone.Count >= 1 && zone.enemiesInZone.Count >= 1)
            {
                foreach (GameObject e in zone.enemiesInZone)
                {
                    Enemy enemy = e.GetComponent<Enemy>();

                    //Makes the enemies move by giving them a speed value
                    enemy.charAgent.speed = 3.5f;

                    foreach (GameObject cm in zone.crewMembersInZone)
                    {
                        CrewMember crewMember = cm.GetComponent<CrewMember>();

                        if (crewMember.isActiveAndEnabled)
                        {
                            //Enemies will go after crew members in the zone
                            enemy.charAgent.SetDestination(crewMember.transform.position);
                        }

                        //if(enemy.isActiveAndEnabled)
                        //{
                        //    crewMember.charAgent.SetDestination(enemy.transform.position);
                        //}
                    }
                }
            }
            //If there is no longer a crew member in the zone
            else if (zone.crewMembersInZone.Count <= 0 && zone.enemiesInZone.Count >= 1)
            {
                foreach (GameObject e in zone.enemiesInZone)
                {
                    Enemy enemy = e.GetComponent<Enemy>();

                    //Make the enemies stop chasing by setting their speed value to zero
                    enemy.charAgent.speed = 0;
                }
            }
        }

        for(int i = 0; i < zones.Count; i++)
        {
            Zone zone = zones[i].GetComponent<Zone>();
            if (!zones[i].GetComponent<Zone>().zoneLootCollected)
            {
                Debug.Log("We broke out of update");
                return;
            }
        }
        
        if (!finishedCombatBool)
        {
            Debug.Log("You triggered the finished combat state!");
            finishedCombatBool = true;
            finishedCombat.Raise(this, 0);
        }
    }

    /// <summary>
    /// Makes the crew members retreat back to the ship's waypoint game object
    /// </summary>
    public void Retreat()
    {
        for (int i = 0; i < crewMembers.Count; i++)
        {
            CrewMember crewMember = crewMembers[i].GetComponent<CrewMember>();

            crewMember.charAgent.SetDestination(shipWaypoint.transform.position);
        }

        //Makes marker disappear when retreating to ship
        GameObject marker = GameObject.FindGameObjectWithTag("Marker");
        if(GameObject.FindGameObjectWithTag("Marker"))
        {
            marker.SetActive(false);
        }
    }

    public void OnCrewmateDropAssign()
    {
        Crewmate[] selectedCrewmates = cm.GetSelectedCrewmates();
        Crewmate crewmateDropped = selectedCrewmates[0];

        UnitMovement unitToMove = crewmateDropped.GetComponent<UnitMovement>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, unitToMove.mask, QueryTriggerInteraction.Ignore))
        {
            //Moves units to the center of zone when it is clicked on
            if (hit.collider.tag == "Zone")
            {
                Zone zone = hit.transform.gameObject.GetComponent<Zone>();

                NavMeshAgent myAgent = crewmateDropped.GetComponent<NavMeshAgent>();
                myAgent.SetDestination(zone.zoneCenter);
            }
        }
    }
}
