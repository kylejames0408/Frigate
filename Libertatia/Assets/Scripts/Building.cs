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
    private int workProgress = 0;
    [SerializeField] private int totalWorkToComplete = 100;
    private float percentComplete = 0.0f;
    private int builderAmount = 0; // could make this a list of builders if we wanted to improve UI
    private int builderCapacity = 1;
    private List<Builder> builders;
    // Components
    private MeshRenderer buildingRender;
    // Colors
    private Color normalEmmision;
    private Color hoveredEmmision;
    // Materials
    [SerializeField] private Material builtMaterial;
    [SerializeField] private Material placingMaterial;
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private Material assignedMaterial;
    [SerializeField] private Material collisionMaterial;

    [Header("Events")]
    public GameEvent onCrewmateAssigned;

    public bool IsAssigned
    {
        get
        {
            return state == BuildingState.PLACING;
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
    void Start()
    {
        state = BuildingState.PLACING;
        buildingRender.material = placingMaterial;
        isHovered = false;
        workProgress = 0;
        percentComplete = 0.0f;
        // Colors
        normalEmmision = Color.black;
        hoveredEmmision = new Color(0.3f, 0.3f, 0.3f);
        builders = new List<Builder>();
    }
    public bool CanBuild(Resources resources)
    {
        return resources.wood >= resourceCost[0] && resources.gold >= resourceCost[1];
    }
    public bool AssignBuilder(Builder builder)
    {
        if (IsComplete || builderAmount >= builderCapacity)
        {
            return false;
        }
        builderAmount++;
        builders.Add(builder);
        onCrewmateAssigned.Raise(this, builder);
        state = BuildingState.BUILDING;
        return true;
    }
    public void Build(int work)
    {
        workProgress += work; // Add progress
        percentComplete = (float)workProgress / totalWorkToComplete; // Update percentage

        // Check if completed
        if(percentComplete >= 1.0f)
        {
            CompleteBuild();
        }
    }
    private void CompleteBuild()
    {
        state = BuildingState.COMPLETE;
        buildingRender.material = builtMaterial;
        builderAmount = 0;
    }
    public void FreeBuilders()
    {
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
            buildingRender.material.SetColor("_EmissionColor", hoveredEmmision);
        }
    }
    private void OnMouseExit()
    {
        if (buildingRender && isHovered)
        {
            buildingRender.material.SetColor("_EmissionColor", normalEmmision);
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
