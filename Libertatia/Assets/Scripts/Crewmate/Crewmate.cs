﻿using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum CrewmateState
{
    IDLE,
    BUILDING,
    ATTACKING,
    MOVING
}

[Serializable]
public class Crewmate : MonoBehaviour
{
    // Components
    private NavMeshAgent agent;
    // Tracking
    [SerializeField] private int id = -1;
    [SerializeField] private CrewmateState state = CrewmateState.IDLE;
    [SerializeField] private ObjectData building;
    // Characteristics
    [SerializeField] private string fullName;
    [SerializeField] private string firstName;
    [SerializeField] private string lastName;
    [SerializeField] private int health = 100;
    [SerializeField] private int strength = -1;
    [SerializeField] private int agility = -1;
    [SerializeField] private int stamina = -1;
    // UI
    [SerializeField] private Sprite iconCrewmate;
    [SerializeField] private Sprite iconStatus;
    [SerializeField] private bool isHovered = false;
    // Cache
    [SerializeField] private Sprite iconDefaultBuilding;
    [SerializeField] private Sprite iconStatusIdle;
    [SerializeField] private Sprite iconStatusBuilding;
    [SerializeField] private Sprite iconStatusAttaching;
    [SerializeField] private Sprite iconStatusMoving;
    // Events
    public UnityEvent onAssign;
    public UnityEvent onReassign;
    public UnityEvent onSelect;
    public UnityEvent onDestroy;

    public int ID
    {
        get { return id; }
    }
    public ObjectData Building
    {
        get { return building; }
    }
    public string FullName
    {
        get { return fullName; }
    }
    public string FirstName
    {
        get { return firstName; }
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
        get { return iconCrewmate; }
    }
    public bool IsHovered
    {
        get { return isHovered; }
    }
    public Sprite StatusIcon
    {
        get { return iconStatus; }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        id = gameObject.GetInstanceID();
        building = new ObjectData(-1, iconDefaultBuilding); // Is there a need for a default icon?
        // Update once values are set
        strength = UnityEngine.Random.Range(1, 6);
        agility = UnityEngine.Random.Range(1, 6);
        stamina = UnityEngine.Random.Range(1, 6);
    }
    private void Start()
    {
        agent.Warp(transform.position);
    }
    private void Update()
    {
        HandleSelection();
    }
    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            isHovered = true;
        }
    }
    private void OnMouseExit()
    {
        isHovered = false;
    }
    // Implement
    private void OnDestroy()
    {
        //onDestroy.Invoke();
    }

    // Actions
    public void Set(CrewmateData data)
    {
        // Tracking
        id = data.id;
        building = data.building;
        // Characteristics
        fullName = data.name;
        health = data.health;
        strength = data.strength;
        agility = data.agility;
        stamina = data.stamina;
        // UI
        iconCrewmate = data.icon;
        // Spacial
        transform.position = data.position;
        transform.rotation = data.rotation;
    }
    public void Assign(int buildingID, Sprite buildingIcon, Vector3 destination)
    {
        if(building.id != -1)
        {
            onReassign.Invoke();
        }
        building = new ObjectData(buildingID, buildingIcon); // Assign building
        agent.destination = destination; // Set destination
        onAssign.Invoke(); // Update UI
    }
    public void Unassign()
    {
        building.Reset(iconDefaultBuilding);
    }

    // Util
    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    // UI
    public void SetUI(Sprite crewmateIcon, Sprite emptyBuildingAssignmentIcon) // will likely have state in future, similar to building
    {
        iconCrewmate = crewmateIcon;
        iconDefaultBuilding = emptyBuildingAssignmentIcon;
        building.icon = iconDefaultBuilding;
    }

    // Handlers
    private void HandleSelection()
    {
        if (isHovered && Input.GetMouseButtonDown(0))
        {
            onSelect.Invoke();
        }
    }
}
