using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// Notes
// Probably just have a local ShipData struct at some point - remove accessors

public enum ShipState
{
    ONBOARDING,
    OFFBOARDING
}
public class Ship : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();
    public int enemyCount;

    public int detectionRange = 25;
    private bool inRange = false;

    public CombatResourcesUI resourceUI;
    [SerializeField] private GameObject battleLootUI;
    [SerializeField] private GameObject leaveButton;

    // Components
    [SerializeField] private OutpostResourcesUI rUI;
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private MeshRenderer[] renderers;
    // Tracking
    [SerializeField] private int id;
    [SerializeField] private int islandID;
    [SerializeField] private int capacity;
    [SerializeField] private List<CrewmateData> crewmates;
    [SerializeField] private ShipState state;
    //[SerializeField] private int level; // I dont think the ship has a level itself?
    // Characteristics
    [SerializeField] private string buildingName = "Ship";
    [SerializeField] private float radius = 10.0f;
    // UI
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isHovered = false;
    // Emissions
    [SerializeField] private Color normalEmission = Color.black;
    [SerializeField] private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);
    // Event
    public UnityEvent<int> onCrewmateAssigned;

    public int ID
    {
        get { return id; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public int IslandID
    {
        get { return islandID; }
    }
    public int Capacity
    {
        get { return capacity; }
    }
    public bool IsHovered
    {
        get { return isHovered; }
    }
    public CrewmateData[] Crewmates
    {
        get { return crewmates.ToArray(); }
    }
    public ShipState State
    {
        get { return state; }
    }

    private void Awake()
    {
        if(renderers == null) { renderers = GetComponentsInChildren<MeshRenderer>(); }
        if (resourceUI == null) { resourceUI = FindObjectOfType<CombatResourcesUI>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rUI == null) { rUI = FindObjectOfType<OutpostResourcesUI>(); }
        if (shipUI == null) { shipUI = FindObjectOfType<ShipUI>(); }

        isHovered = false;

        unitList.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));
        enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }
    void Start()
    {
        ShipData shipData = PlayerDataManager.LoadShipData();
        capacity = shipData.crewCcapacity;

        if (shipData.isInitialized)
        {
            id = shipData.id;
            islandID = shipData.islandID;
            crewmates = shipData.crew.ToList();
            state = shipData.state;
            icon = shipData.icon;
            transform.position = shipData.position;
            transform.rotation = shipData.rotation;
        }
        else
        {
            id = gameObject.GetInstanceID();
            crewmates = new List<CrewmateData>(capacity);
            shipUI.onUnassign.AddListener(UnassignCrewmateCallback);
        }

        // Set UI
        shipUI.Set(capacity);
        for (int i = 0; i < crewmates.Count; i++)
        {
            shipUI.SetCrewmate(i, new ObjectData(crewmates[i].id, crewmates[i].icon));
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(isHovered)
        {
            // move to handlers
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                shipUI.OpenInterface();
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
        isHovered = true;
        foreach (MeshRenderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetColor("_EmissionColor", hoveredEmission);
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
        if (GameManager.Phase == GamePhase.BUILDING)
        {
            PlayerDataManager.SaveShipData(this);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private Vector3 GetDestination()
    {
        // Get random position around unit circle
        Vector2 circlePosition = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 destinationOffset = new Vector3(circlePosition.x * radius, 0, Mathf.Abs(circlePosition.y) * radius); // keeps crewmate in-front of the building, maybe tweak
        return transform.position + destinationOffset;
    }
    public void OnCrewmateDropAssign()
    {
        if (isHovered)
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

                    if (!CrewmateAlreadyBoarded(crewmateData))
                    {
                        if (!OutpostTutorialManager.HasCompletedTutorial && crewmateData.building.id != -1)
                        {
                            continue;
                        }
                        shipUI.SetCrewmate(crewmates.Count, new ObjectData(crewmateData.id, crewmateData.icon));
                        crewmates.Add(crewmateData);
                        mate.Assign(id, icon, GetDestination(), true);
                        onCrewmateAssigned.Invoke(crewmateData.id);
                    }
                }
            }
        }
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
    // Utility
    private bool CrewmateAlreadyBoarded(CrewmateData newCrewmate)
    {
        foreach(CrewmateData crewmate in crewmates)
        {
            if(newCrewmate.id == crewmate.id)
            {
                return true;
            }
        }
        return false;
    }
}