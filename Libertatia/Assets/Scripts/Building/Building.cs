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
    public int[] resourceCost;
    public int builderCapacity = 1;
    private List<Builder> builders;
    // Components
    private MeshRenderer buildingRender;
    // Emissions
    private Color normalEmission;
    private Color hoveredEmission;
    // Materials
    [SerializeField] private Material placingMaterial;
    [SerializeField] private Material collisionMaterial;
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private Material assignedMaterial;
    [SerializeField] private Material builtMaterial;
    // Triggerables
    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;

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
    public int[] Cost
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
        buildingRender.material = placingMaterial;
    }

    public bool CanBuild(Resources resources)
    {
        return resources.wood >= resourceCost[0] && resources.gold >= resourceCost[1];
    }
    public void Place()
    {
        state = BuildingState.WAITING_FOR_ASSIGNMENT;
        builders = new List<Builder>(builderCapacity);
        buildingRender.material = buildingMaterial;
    }
    public bool AssignBuilder(Builder builder)
    {
        if (IsComplete || builders.Count >= builderCapacity)
        {
            return false;
        }
        builders.Add(builder);
        onCrewmateAssigned.Raise(this, builder);
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
            placingMaterial = buildingRender.material;
            buildingRender.material = collisionMaterial;
            isColliding = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Building")
        {
            buildingRender.material = placingMaterial;
            isColliding = false;
        }
    }
}
