using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();
    public int enemyCount;

    public int detectionRange;
    private bool inRange;

    public CombatResourcesUI resourceUI;
    [SerializeField] private GameObject battleLootUI;
    [SerializeField] private GameObject leaveButton;

    // Components
    [SerializeField] private ResourcesUI rUI;
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private MeshRenderer[] renderers;
    // Tracking
    [SerializeField] private bool isCombat = false;
    [SerializeField] private int id = -1;
    [SerializeField] private int capacity = 12;
    [SerializeField] private List<CrewmateData> crewmates;
    [SerializeField] private int level = 0;
    // Characteristics
    [SerializeField] private string buildingName = "Ship";
    [SerializeField] private float radius = 10.0f;
    // UI
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isHovered = false;
    // Emissions
    [SerializeField] private Color normalEmission = Color.black;
    [SerializeField] private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);

    // Events
    public GameEvent onCrewmateAssignedGE;

    public int ID
    {
        get { return id; }
    }
    public bool IsHovered
    {
        get { return isHovered; }
    }
    public List<CrewmateData> CrewmateData
    { get { return crewmates; } }

    private void Awake()
    {
        if(renderers == null) { renderers = GetComponentsInChildren<MeshRenderer>(); }
        if (resourceUI == null) { resourceUI = FindObjectOfType<CombatResourcesUI>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rUI == null) { rUI = FindObjectOfType<ResourcesUI>(); }
        if (shipUI == null) { shipUI = FindObjectOfType<ShipUI>(); }

        id = gameObject.GetInstanceID();
        level = 0;
        isHovered = false;
    }
    void Start()
    {
        crewmates = new List<CrewmateData>(capacity); // new ObjectData(-1, iconEmptyAsssignment)
        shipUI.onUnassign.AddListener(UnassignCrewmateCallback);
        shipUI.Set(capacity);

        detectionRange = 25;
        inRange = false;

        unitList.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));
        enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }
    // Update is called once per frame
    void Update()
    {
        if(isHovered)
        {
            // move to handlers
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                shipUI.OpenMenu();
            }
            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (cm.IsCrewmateSelected && crewmates.Count < crewmates.Capacity)
                {
                    // Get selected units
                    Crewmate[] selectedCrewmates = cm.GetSelectedCrewmates();
                    // Assign - move to function
                    for (int i = 0; i < selectedCrewmates.Length; i++)
                    {
                        Crewmate mate = selectedCrewmates[i];
                        CrewmateData crewmateData = new CrewmateData(mate);
                        shipUI.SetCrewmate(crewmates.Count, new ObjectData(crewmateData.id, crewmateData.icon));
                        crewmates.Add(crewmateData);
                        mate.Assign(id, icon, GetDestination());
                    }
                }
                else
                {
                    Debug.Log("Ship assignments are full");
                }
            }
        }

        foreach (GameObject unit in unitList)
        {
            if (unit.activeSelf != false)
            {
                //If a unit is within range of the ship, the player can return to the outpost
                if (Vector3.Distance(transform.position, unit.transform.position) <= detectionRange)
                {
                    //Debug.Log("Go home");
                    inRange = true;

                    //sets the leave button to be active if within range
                    leaveButton.SetActive(true);

                }
                else
                {
                    inRange = false;
                    leaveButton.SetActive(false);
                }
            }
        }
    }
    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            isHovered = true;
            foreach(MeshRenderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetColor("_EmissionColor", hoveredEmission);
                }
            }
        }
    }
    private void OnMouseExit()
    {
        if (isHovered)
        {
            // should validate that renderers exists
            foreach (MeshRenderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetColor("_EmissionColor", normalEmission);
                }
            }
        }
        isHovered = false;
    }
    private void OnDestroy()
    {
        if (isCombat)
        {
            GameManager.UpdateCombatCrew(crewmates.ToArray());
            GameManager.UpdateCrewmateData();
        }
        else
        {
            GameManager.UpdateCrewmateData(crewmates.ToArray());
            GameManager.SeparateCrew();
        }
    }

    private Vector3 GetDestination()
    {
        // Get random position around unit circle
        Vector2 circlePosition = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 destinationOffset = new Vector3(circlePosition.x * radius, 0, Mathf.Abs(circlePosition.y) * radius); // keeps crewmate in-front of the building, maybe tweak
        return transform.position + destinationOffset;
    }
    public void OpenBattleLootUI()
    {
        //GameManager.data.resources.wood += 50;
        //GameManager.data.resources.doubloons += 10;
        //GameManager.data.resources.food += 100;

        GameManager.data.resources.wood += resourceUI.woodAmount;
        GameManager.data.resources.doubloons += resourceUI.doubloonAmount;
        GameManager.data.resources.food += resourceUI.foodAmount;

        //Opens the battle loot ui
        battleLootUI.SetActive(true);
        //CeneManager.LoadOutpostFromCombat();
    }

    // Callback
    private void UnassignCrewmateCallback(int crewmateID)
    {
        for (int i = 0; i < crewmates.Count; i++)
        {
            if (crewmates[i].id == crewmateID)
            {
                crewmates.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < crewmates.Count; i++)
        {
            shipUI.SetCrewmate(i, new ObjectData(crewmates[i].id, crewmates[i].icon));
        }
        shipUI.ResetCard(crewmates.Count);
        cm.UnassignCrewmate(crewmateID);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void OnCrewmateDropAssign()
    {
        if (isHovered)
        {
            if (crewmates.Count < capacity)
            {
                // Get selected units
                Crewmate[] selectedCrewmates = cm.GetSelectedCrewmates();
                // Assign - move to function
                for (int i = 0; i < selectedCrewmates.Length; i++)
                {
                    if (crewmates.Count >= capacity)
                    {
                        Debug.Log("Ship is full");
                        return;
                    }
                     
                    Crewmate mate = selectedCrewmates[i];
                    CrewmateData crewmateData = new CrewmateData(mate);
                    shipUI.SetCrewmate(crewmates.Count, new ObjectData(crewmateData.id, crewmateData.icon));
                    crewmates.Add(crewmateData);
                    mate.Assign(id, icon, GetDestination());
                    onCrewmateAssignedGE.Raise(this, mate);
                }
                
                //onCrewmateShipAssigned.Invoke();
            }
            else
            {
                Debug.Log("Ship assignments are full");
            }
        }
    }
}
