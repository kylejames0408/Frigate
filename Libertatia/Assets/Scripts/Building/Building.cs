using System.Collections.Generic;
using UnityEngine;

public enum BuildingState
{
    PLACING,
    WAITING_FOR_ASSIGNMENT,
    BUILDING,
    COMPLETE
}

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    // General Data
    public string buildingName;
    public float radius = 5.0f; // for construction
    private BuildingState state = BuildingState.PLACING;
    [ColorUsage(true, true)]
    [SerializeField] private Color[] stateColors; // Not sure how this is used yet
    // Identifiers
    public bool isHovered = false;
    public bool isColliding = false;
    // Building
    public BuildingResources resourceCost;
    public int builderCapacity = 1;
    private List<Builder> builders;
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

    public bool IsAssigned
    {
        get
        {
            return state == BuildingState.BUILDING;
        }
    }
    public bool IsFullyAssigned
    {
        get
        {
            return builders.Count == builders.Capacity;
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

    public void Place()
    {
        builders = new List<Builder>(builderCapacity);
        buildingRender.material = components.needsAssignmentMaterial;
        state = BuildingState.WAITING_FOR_ASSIGNMENT;
    }
    public bool AssignBuilder(Builder builder)
    {
        if (IsComplete || builders.Count >= builderCapacity)
        {
            return false;
        }
        onCrewmateAssigned.Raise(this, builder);
        builders.Add(builder);
        buildingRender.material = components.buildingMaterial;
        state = BuildingState.BUILDING;
        return true;
    }
    public void CompleteBuild()
    {
        state = BuildingState.COMPLETE;
        buildingRender.material = builtMaterial;
        // Free builders
        foreach (Builder builder in builders)
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
            buildingRender.material = components.placingMaterial;
            isColliding = false;
        }
    }
}
