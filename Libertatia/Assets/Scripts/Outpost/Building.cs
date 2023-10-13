using System.Collections.Generic;
using UnityEngine;

public enum BuildingState
{
    PLACING,
    WAITING_FOR_ASSIGNMENT,
    BUILDING,
    COMPLETE
}
// Move some individualstic data to scriptable objects, no need for prefabs atm
// Building objects are put together upon interaction
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    // General Data
    [SerializeField] private string buildingName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int level;
    [SerializeField] private float radius = 5.0f; // for construction
    private BuildingState state = BuildingState.PLACING;
    [ColorUsage(true, true)]
    [SerializeField] private Color[] stateColors; // Not sure how this is used yet
    // Identifiers
    public bool isHovered = false;
    public bool isColliding = false;
    // Building
    public BuildingResources resourceCost;
    public int builderCapacity = 1;
    private List<Crewmate> builders;
    // Components
    private MeshRenderer buildingRender;
    // Emissions
    private Color normalEmission;
    private Color hoveredEmission;
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
    public bool IsAssigned
    {
        get
        {
            return state == BuildingState.BUILDING;
        }
    }
    public bool IsPlacing
    {
        get
        {
            return state == BuildingState.PLACING;
        }
    }
    public bool IsComplete
    {
        get { return state == BuildingState.COMPLETE; }
    }
    public bool IsColliding
    {
        get { return isColliding; }
    }
    public BuildingResources Cost
    {
        get { return resourceCost; }
    }

    private void Awake()
    {
        buildingRender = transform.GetComponentInChildren<MeshRenderer>();
    }
    private void Start()
    {
        isHovered = false;
        normalEmission = Color.black;
        hoveredEmission = new Color(0.3f, 0.3f, 0.3f);
        // Init state
        state = BuildingState.PLACING;
        buildingRender.material = components.placingMaterial;
    }
    private void Update()
    {
        if(isHovered && Input.GetMouseButtonDown(1))
        {
            if(CrewmateManager.Instance.unitsSelected.Count > 0)
            {
                Crewmate mate = CrewmateManager.Instance.unitsSelected[0].GetComponent<Crewmate>();
                if (CanAssign())
                {
                    AssignBuilder(mate);
                }
                else
                {
                    Debug.Log("Building assignments are full");
                }
            }
        }
    }

    public void Place()
    {
        builders = new List<Crewmate>(builderCapacity);
        buildingRender.material = components.needsAssignmentMaterial;
        state = BuildingState.WAITING_FOR_ASSIGNMENT;
    }
    public bool CanAssign()
    {
        return !IsComplete && builders.Count < builders.Capacity;
    }
    // can likely make private
    public void AssignBuilder(Crewmate builder)
    {
        onCrewmateAssigned.Raise(this, builder);
        builders.Add(builder);
        builder.GiveJob(this);
        buildingRender.material = components.buildingMaterial;
        state = BuildingState.BUILDING;
    }
    public void CompleteBuild()
    {
        state = BuildingState.COMPLETE;
        buildingRender.material = builtMaterial;
        // Free builders
        foreach (Crewmate builder in builders)
        {
            builder.Free();
        }
        builders.Clear();
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        if (buildingRender && IsPlacing && !CameraManager.Instance.IsMouseMove)
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
            buildingRender.material = components.collisionMaterial;
            isColliding = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Building")
        {
            switch(state)
            {
                case BuildingState.PLACING:
                    buildingRender.material = components.placingMaterial;
                    break;
                case BuildingState.WAITING_FOR_ASSIGNMENT:
                    buildingRender.material = components.needsAssignmentMaterial;
                    break;
                case BuildingState.BUILDING:
                    buildingRender.material = components.buildingMaterial;
                    break;
                case BuildingState.COMPLETE:
                    buildingRender.material = builtMaterial;
                    break;
            }
            isColliding = false;
        }
    }
}
