using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BuildingState
{
    PLACING,
    WAITING_FOR_ASSIGNMENT,
    BUILDING,
    COMPLETE
}

// Move some individualstic data to scriptable objects, no need for prefabs atm
// Building objects are put together upon interaction
[Serializable]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    // General Data
    public int id;
    [SerializeField] private string buildingName;
    [SerializeField] private Sprite icon;
    public string output;
    public int uiIndex;
    public int level;
    private BuildingState state = BuildingState.PLACING;
    // Identifiers
    public bool isHovered = false;
    public bool isPlacementValid = true;
    // In-game characteristics
    [SerializeField] private float radius = 5.0f; // for construction
    private int numOfCollisions = 0;
    // Building
    public BuildingResources resourceCost;
    public BuildingResources resourceProduction;
    public Crewmate assignee1;
    public Crewmate assignee2;
    // Components
    private MeshRenderer buildingRender;
    private NavMeshObstacle navObsticle;
    // Emissions
    private Color normalEmission = Color.black;
    private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);
    // Materials
    [SerializeField] private BuildingComponents components;
    [SerializeField] private Material builtMaterial;

    [Header("Events")]
    public GameEvent onCrewmateAssigned;

    public string Name
    {
        get { return buildingName; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public float Radius
    {
        get { return radius; }
    }
    public bool IsComplete
    {
        get { return state == BuildingState.COMPLETE; }
    }
    public BuildingResources Cost
    {
        get { return resourceCost; }
    }

    private void Awake()
    {
        buildingRender = GetComponentInChildren<MeshRenderer>();
        navObsticle = GetComponent<NavMeshObstacle>();
        navObsticle.enabled = false;
        id = gameObject.GetInstanceID();
        uiIndex = -1;
        level = 0;
        state = BuildingState.PLACING;
        buildingRender.material = components.placingMaterial;
        isHovered = false;
        numOfCollisions = 0;
    }
    private void Update()
    {
        if(isHovered)
        {
            HandleSelection();
            HandleAssignment();
        }
    }

    public void Place()
    {
        buildingRender.material = components.needsAssignmentMaterial;
        navObsticle.enabled = true;
        state = BuildingState.WAITING_FOR_ASSIGNMENT;
    }
    public bool CanAssign()
    {
        if(state == BuildingState.WAITING_FOR_ASSIGNMENT)
        {
            return !assignee1;
        }
        else if (state == BuildingState.COMPLETE)
        {
            return !assignee1 || !assignee2;
        }
        else
        {
            Debug.LogWarning("Building is not in a state to assign");
            return false;
        }
    }
    public void AssignCrewmate(Crewmate assignee)
    {
        onCrewmateAssigned.Raise(this, assignee);
        // Assign
        if(assignee1 == null)
        {
            assignee1 = assignee;
        }
        else if(assignee2 == null)
        {
            assignee2 = assignee;
        }
        assignee.GiveJob(this);
        // Update building state
        if(state == BuildingState.WAITING_FOR_ASSIGNMENT)
        {
            buildingRender.material = components.buildingMaterial;
            state = BuildingState.BUILDING;
        }
    }
    public void CompleteBuild()
    {
        state = BuildingState.COMPLETE;
        buildingRender.material = builtMaterial;
        level = 1;
        navObsticle.enabled = true; // maybe cache? I am not sure why this is disabled when coming back to outpost
        FreeAssignees();
    }
    public void Upgrade()
    {
        // conditional
        level++;
    }
    public void Demolish()
    {
        FreeAssignees();
        Destroy(gameObject);
    }
    private void FreeAssignees()
    {
        if (assignee1 != null)
        {
            assignee1.Free();
            assignee1 = null;
        }
        if (assignee2 != null)
        {
            assignee2.Free();
            assignee2 = null;
        }
    }
    public string GetStatus()
    {
        string value = string.Empty;
        switch (state)
        {
            case BuildingState.PLACING:
                value = "Placing";
                break;
            case BuildingState.WAITING_FOR_ASSIGNMENT:
                value = "Needs assignment";
                break;
            case BuildingState.BUILDING:
                value = "Building";
                break;
            case BuildingState.COMPLETE:
                value = "Level " + level.ToString();
                break;
        }
        return value;
    }
    public void PlacementValid()
    {
        if (isPlacementValid || numOfCollisions > 0) return;

        switch (state)
        {
            case BuildingState.PLACING:
                buildingRender.material = new Material(components.placingMaterial);
                break;
            case BuildingState.WAITING_FOR_ASSIGNMENT:
                buildingRender.material = new Material(components.needsAssignmentMaterial);
                break;
            case BuildingState.BUILDING:
                buildingRender.material = new Material(components.buildingMaterial);
                break;
            case BuildingState.COMPLETE:
                buildingRender.material = new Material(builtMaterial);
                break;
        }
        isPlacementValid = true;
    }
    public void PlacementInvalid()
    {
        if (!isPlacementValid) return;

        buildingRender.material = new Material(components.collisionMaterial);
        isPlacementValid = false;
    }

    // Handle
    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) && state != BuildingState.PLACING)
        {
            BuildingUI.Instance.FillUI(this);
        }
    }
    private void HandleAssignment()
    {
        if (Input.GetMouseButtonDown(1) && CrewmateManager.Instance.selectedCrewmates.Count > 0)
        {
            Crewmate mate = CrewmateManager.Instance.selectedCrewmates[0];
            if (CanAssign())
            {
                AssignCrewmate(mate);
                BuildingUI.Instance.FillUI(this);
            }
            else
            {
                Debug.Log("Building assignments are full");
            }
        }
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        if (state != BuildingState.PLACING)
        {
            buildingRender.material.SetColor("_EmissionColor", hoveredEmission);
        }
    }
    private void OnMouseExit()
    {
        if (buildingRender && isHovered)
        {
            buildingRender.material.SetColor("_EmissionColor", normalEmission);
        }
        isHovered = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius); // offset: collider.center? wont work in inspector
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Building")
        {
            if(numOfCollisions == 0)
            {
                PlacementInvalid();
            }

            numOfCollisions++;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Building")
        {
            numOfCollisions--;

            if (numOfCollisions == 0)
            {
                PlacementValid();
            }
        }
    }
}
