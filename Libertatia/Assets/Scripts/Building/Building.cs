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
    [SerializeField] private float radius = 5.0f; // for construction
    private BuildingState state = BuildingState.PLACING;
    private int numOfCollisions = 0;
    // Identifiers
    public bool isHovered = false;
    // Building
    public BuildingResources resourceCost;
    public int builderCapacity = 1;
    public Crewmate builder;
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
    public bool IsColliding
    {
        get { return numOfCollisions > 0; }
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
        return !IsComplete && !builder;
    }
    public void AssignBuilder(Crewmate builder)
    {
        onCrewmateAssigned.Raise(this, builder);
        this.builder = builder;
        builder.GiveJob(this);
        buildingRender.material = components.buildingMaterial;
        state = BuildingState.BUILDING;
    }
    public void CompleteBuild()
    {
        state = BuildingState.COMPLETE;
        buildingRender.material = builtMaterial;
        FreeBuilder();
    }
    public void Upgrade()
    {
        // conditional
        //level++;
    }
    public void Demolish()
    {
        FreeBuilder();
        Destroy(gameObject);
    }
    private void FreeBuilder()
    {
        // only can occur through dev menu
        if (builder != null)
        {
            builder.Free(); // Free builders
            builder = null;
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
        if (Input.GetMouseButtonDown(1) && CrewmateManager.Instance.unitsSelected.Count > 0)
        {
            Crewmate mate = CrewmateManager.Instance.unitsSelected[0].GetComponent<Crewmate>();
            if (CanAssign())
            {
                AssignBuilder(mate);
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
                buildingRender.material = new Material(components.collisionMaterial);
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
            }
        }
    }
}
