using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class ZoneManager : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private OutpostManagementUI omui;
    [SerializeField] private CombatManagementUI cmui;

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

    [SerializeField] private GameObject battleLootUI;

    //Zone information ui
    [SerializeField] private GameObject zoneInfoUI;
    [SerializeField] private TextMeshProUGUI crewmateCountUI;
    [SerializeField] private TextMeshProUGUI enemyCountUI;
    [SerializeField] private TextMeshProUGUI resourceCountUI;
    [SerializeField] private GameObject houseImage;
    [SerializeField] private GameObject farmImage;

    //current zone that is displaying information
    private Zone selectedZone;

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
        //Initializes crew members
        if (crewMembers.Count == 0)
        {
            crewMembers = GameObject.FindGameObjectsWithTag("PlayerCharacter").ToList<GameObject>();

            //start by having each crew member's target position point to themselves
            foreach (GameObject cm in crewMembers)
            {
                CrewMember crewMember = cm.GetComponent<CrewMember>();

                crewMember.targetPos = crewMember.transform.position;
            }
        }

        //for each crew member
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
        PrintZoneInfo();

        //Obtain loot from zones after beating the enemies, if there are resource deposits
        foreach(Terrain terrain in zones)
        {
            Zone zone = terrain.GetComponent<Zone>();

            //if a zone has been clicked on and the zone info ui is open
            if(selectedZone != null && zoneInfoUI.activeSelf)
            {
                //continously updates the information
                crewmateCountUI.text = selectedZone.crewMembersInZone.Count.ToString();
                enemyCountUI.text = selectedZone.enemiesInZone.Count.ToString();

                if(selectedZone.housesInZone.Count > 0)
                {
                    EnemyHouse enemyHouse = selectedZone.housesInZone[0].GetComponent<EnemyHouse>();

                    if(enemyHouse.resourceType == "wood")
                    {
                        resourceCountUI.text = enemyHouse.lootValue + " wood";
                    }

                    if (enemyHouse.resourceType == "food")
                    {
                        resourceCountUI.text = enemyHouse.lootValue + " food";
                    }

                }
                else
                {
                    HideZoneBuildingImages();

                    resourceCountUI.text = "No Resources";
                }
          
            }

            //if there is a resource depot & the zone has at least 1 crew member & the zone has no enemies left
            if(zone.housesInZone.Count >= 1 && zone.crewMembersInZone.Count >= 1 && zone.enemiesInZone.Count <= 0)
            {
                //if the loot has not been collected yet
                ClearZoneLoot(zone);
            }

            //captures the zone if there is at least one crew member in it with no other enemies
            if (zone.housesInZone.Count == 0 && zone.crewMembersInZone.Count >= 1 && zone.enemiesInZone.Count <= 0)
            {
                zone.zoneLootCollected = true;
                zone.centerObject.SetActive(true);
            }

            //If there is at least one crew member and enemy in a zone
            if (zone.crewMembersInZone.Count >= 1 && zone.enemiesInZone.Count >= 1)
            {
                foreach (GameObject e in zone.enemiesInZone)
                {
                    Enemy enemy = e.GetComponent<Enemy>();

                    //Makes the enemies move by giving them a speed value
                    //enemy.charAgent.speed = 3.5f;

                    foreach (GameObject cm in zone.crewMembersInZone)
                    {
                        CrewMember crewMember = cm.GetComponent<CrewMember>();

                        if (crewMember.isActiveAndEnabled)
                        {
                            //Enemies will go after crew members in the zone
                            //enemy.charAgent.SetDestination(crewMember.transform.position);

                            enemy.charAgent.SetDestination(enemy.GetClosestUnit(zone.crewMembersInZone));
                            enemy.targetPos = enemy.GetClosestUnit(zone.crewMembersInZone);

                            enemy.characterState = Character.State.Moving;

                            if (Vector3.Distance(enemy.transform.position, crewMember.transform.position) < enemy.attackRange)
                            {
                                enemy.characterState = Character.State.Attacking;
                            }
                        }

                        if (enemy.isActiveAndEnabled)
                        {
                            //Crew members will go after enemies in the zone
                            //crewMember.charAgent.SetDestination(enemy.transform.position);

                            crewMember.charAgent.SetDestination(crewMember.GetClosestUnit(zone.enemiesInZone));
                            crewMember.targetPos = crewMember.GetClosestUnit(zone.enemiesInZone);

                            crewMember.lineRenderer.SetPosition(1, crewMember.GetClosestUnit(zone.enemiesInZone));

                            //updates state enums
                            crewMember.characterState = Character.State.Moving;
                            Crewmate crewmate = crewMember.GetComponent<Crewmate>();
                            crewmate.State = CrewmateState.MOVING;

                            //updates state on cards
                            cmui.UpdateCard(crewmate.ID, crewmate.StateIcon);
                        }
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

        //If all the enemies are dead, automatically collect every loot and open the battle loot ui
        CompleteIsland();

        for (int i = 0; i < zones.Count; i++)
        {
            Zone zone = zones[i].GetComponent<Zone>();
            if (!zones[i].GetComponent<Zone>().zoneLootCollected)
            {
                //Debug.Log("We broke out of update");
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

    //METHODS

    /// <summary>
    /// Clears every zone's loot if every enemy is dead and opens the battle loot ui
    /// </summary>
    public void CompleteIsland()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>().Count <= 0)
        {
            foreach (Terrain terrain in zones)
            {
                Zone zone = terrain.GetComponent<Zone>();

                ClearZoneLoot(zone);
            }

            battleLootUI.SetActive(true);
        }
    }

    /// <summary>
    /// Obtains loot from a zone
    /// </summary>
    /// <param name="zone"></param>
    public void ClearZoneLoot(Zone zone)
    {
        if (zone.zoneLootCollected == false)
        {
            CombatResourcesUI combatResources = combatUI.GetComponent<CombatResourcesUI>();

            foreach (GameObject enemyHouse in zone.housesInZone)
            {
                EnemyHouse house = enemyHouse.GetComponent<EnemyHouse>();

                //obtain wood from the houses
                switch (house.resourceType)
                {
                    case "wood":
                        combatResources.woodAmount += house.lootValue;
                        combatResources.UpdateWoodUI(combatResources.woodAmount);
                        break;
                    case "food":
                        combatResources.foodAmount += house.lootValue;
                        combatResources.UpdateFoodAmountUI(combatResources.foodAmount);
                        break;
                    default:
                        break;
                }
                house.CreatePopUpText();
            }

            zone.zoneLootCollected = true;
            zone.centerObject.SetActive(true);
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

            crewMember.targetPos = shipWaypoint.transform.position;
            crewMember.charAgent.SetDestination(shipWaypoint.transform.position);

            crewMember.lineRenderer.SetPosition(1, shipWaypoint.transform.position);

            crewMember.characterState = Character.State.Moving;

            Crewmate crewmate = crewMember.GetComponent<Crewmate>();
            crewmate.State = CrewmateState.MOVING;

            cmui.UpdateCard(crewmate.ID, crewmate.StateIcon);
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

                for(int i = 0; i < selectedCrewmates.Length; i++)
                {
                    NavMeshAgent myAgent = selectedCrewmates[i].GetComponent<NavMeshAgent>();

                    //makes crewmates move to a random position within a sphere around the center of the zone
                    myAgent.SetDestination(zone.zoneCenter + (Vector3)UnityEngine.Random.insideUnitSphere * 7f);

                    CrewMember crewMember = selectedCrewmates[i].GetComponent<CrewMember>();
                    crewMember.targetPos = zone.zoneCenter + (Vector3)UnityEngine.Random.insideUnitSphere * 7f;
                    crewMember.characterState = Character.State.Moving;

                    Crewmate crewmate = crewMember.GetComponent<Crewmate>();
                    crewmate.State = CrewmateState.MOVING;

                    cmui.UpdateCard(crewmate.ID, crewmate.StateIcon);

                    ShowLineRenderer(zone.zoneCenter + (Vector3)UnityEngine.Random.insideUnitSphere * 7f, crewMember);
                }   
            }
        }
    }

    /// <summary>
    /// Prints zone info to console when left clicking on a zone
    /// </summary>
    public void PrintZoneInfo()
    {
        //left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //if clicking on a zone
                if (hit.transform.gameObject.tag == "Zone")
                {
                    Zone zone = hit.transform.gameObject.GetComponent<Zone>();

                    //selects the current zone
                    selectedZone = zone;

                    Debug.Log("Crew Members: " + zone.crewMembersInZone.Count + " Enemies: " + zone.enemiesInZone.Count + " Houses: " + zone.housesInZone.Count);

                    //opens the zone info ui when clicking on a zone
                    UpdateZoneTextInfo(zone);
                }
                else
                {
                    //deactivates the zone info ui when not clicking on a zone
                    zoneInfoUI.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Updates the zone info text
    /// </summary>
    /// <param name="zone"></param>
    public void UpdateZoneTextInfo(Zone zone)
    {
        zoneInfoUI.SetActive(true);

        crewmateCountUI.text = zone.crewMembersInZone.Count.ToString();
        enemyCountUI.text = zone.enemiesInZone.Count.ToString();

        //resourceCountUI.text = zone.housesInZone.Count.ToString();

        if(zone.housesInZone.Count > 0)
        {
            //Assuming there is only one resource in each zone
            EnemyHouse resourceBuilding = zone.housesInZone[0].GetComponent<EnemyHouse>();

            //Shows the image of the resource building in the zone
            if(resourceBuilding.resourceType == "wood")
            {
                ShowZoneBuildingImage(houseImage);

                resourceCountUI.text = resourceBuilding.lootValue + " wood";

            }
            else if (resourceBuilding.resourceType == "food")
            {
                ShowZoneBuildingImage(farmImage);

                resourceCountUI.text = resourceBuilding.lootValue + " food";
            }
        }
    }

    private void ShowZoneBuildingImage(GameObject buildingImage)
    {
        HideZoneBuildingImages();

        buildingImage.SetActive(true);
    }

    private void HideZoneBuildingImages()
    {
        houseImage.SetActive(false);
        farmImage.SetActive(false);
    }

    private void ShowLineRenderer(Vector3 targetPos, CrewMember crewMember)
    {
        crewMember.lineRenderer.enabled = true;

        crewMember.lineRenderer.SetPosition(0, crewMember.transform.position);
        crewMember.lineRenderer.SetPosition(1, targetPos);

    }
}
