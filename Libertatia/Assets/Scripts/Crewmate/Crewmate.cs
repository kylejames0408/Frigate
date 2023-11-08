using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public struct AssignedBuildingData
{
    public int id;
    public Sprite icon;

    public AssignedBuildingData(int id, Sprite icon)
    {
        this.id = id;
        this.icon = icon;
    }

    public void Reset()
    {
        id = -1;
        icon = null;
    }
}

[Serializable]
public class Crewmate : MonoBehaviour
{
    // Components
    private NavMeshAgent agent;
    // Tracking
    [SerializeField] private int id = -1;
    [SerializeField] private AssignedBuildingData building;
    // Characteristics
    [SerializeField] private string crewmateName;
    [SerializeField] private int health = 100;
    [SerializeField] private int strength = -1;
    [SerializeField] private int agility = -1;
    [SerializeField] private int stamina = -1;
    // UI
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isHovered = false;
    // Cache
    [SerializeField] private Sprite iconEmptyAsssignment;
    // Events
    public UnityEvent onAssign;
    public UnityEvent onReassign;
    public UnityEvent onSelect;
    public UnityEvent onDestroy;

    public int ID
    {
        get { return id; }
    }
    public AssignedBuildingData Building
    {
        get { return building; }
    }
    public string Name
    {
        get { return crewmateName; }
    }
    public int Health
    {
        get { return health; }
    }
    public int Strength
    {
        get { return strength; }
    }
    public int Agility
    {
        get { return agility; }
    }
    public int Stamina
    {
        get { return stamina; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public bool IsHovered
    {
        get { return isHovered; }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        id = gameObject.GetInstanceID();
        building = new AssignedBuildingData(-1, iconEmptyAsssignment); // Is there a need for a default icon?
        // Update once values are set
        strength = UnityEngine.Random.Range(1, 6);
        agility = UnityEngine.Random.Range(1, 6);
        stamina = UnityEngine.Random.Range(1, 6);
    }
    private void Update()
    {
        HandleSelection();
    }
    private void OnMouseEnter()
    {
        isHovered = true;
    }
    private void OnMouseExit()
    {
        isHovered = false;
    }
    private void OnDestroy()
    {
        //onDestroy.Invoke();
    }

    // Actions
    public void Init(CrewmateData data)
    {
        // Tracking
        id = data.id;
        building = data.building;
        // Characteristics
        crewmateName = data.name;
        health = data.health;
        strength = data.strength;
        agility = data.agility;
        stamina = data.stamina;
        // UI
        icon = data.icon;

        transform.position = data.position;
        transform.rotation = data.rotation;
    }
    public void SetUI(Sprite crewmateIcon, Sprite emptyBuildingAssignmentIcon) // will likely have state in future, similar to building
    {
        icon = crewmateIcon;
        iconEmptyAsssignment = emptyBuildingAssignmentIcon;
        building.icon = iconEmptyAsssignment;
    }
    public void Assign(int buildingID, Sprite buildingIcon, Vector3 destination)
    {
        if(building.id != -1)
        {
            onReassign.Invoke();
        }
        building = new AssignedBuildingData(buildingID, buildingIcon); // Assign building
        agent.destination = destination; // Set destination
        onAssign.Invoke(); // Update UI
    }
    public void Free()
    {
        building.Reset();
    }

    // Handlers
    private void HandleSelection()
    {
        if (isHovered && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            onSelect.Invoke();
        }
    }
}
