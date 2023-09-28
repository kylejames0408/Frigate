using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
//using DG.Tweening;

// Turns out a rigidbody is required for collisions to work
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour
{
    // General Data
    public string buildingName;
    public float radius = 5.0f; // for construction
    [ColorUsage(true, true)]
    [SerializeField] private Color[] stateColors; // Not sure how this is used yet
    // Identifiers
    public bool isHovered = false;
    public bool isPlacing = false;
    public bool isAttackable = false;
    // Placement
    public bool isColliding = false;
    // Building
    public int[] resourceCost = default;
    private int workProgress = 0;
    [SerializeField] private int totalWorkToComplete = 100;
    private float percentComplete = 0.0f;
    private bool isComplete = false;
    private int builderAmount = 0; // could make this a list of builders if we wanted to improve UI
    private int builderCapacity = 1;
    private List<Builder> builders;
    // Building animation
    private Vector3 startPos;
    private Vector3 endPos;
    [SerializeField] private float height;
    // Components
    private Damageable damageScript;        // used to indicate if destroyed
    private MeshRenderer buildingRender;    // Unused RN - for animations
    // Colors
    private Color normalEmmision;
    private Color hoveredEmmision;
    // Materials
    [SerializeField] private Material placingMaterial;
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private Material builtMaterial;
    [SerializeField] private Material collisionMaterial;

    [Header("Events")]
    public GameEvent onCrewmateAssigned;

    public bool IsAssigned
    {
        get
        {
            return builderAmount == builderCapacity;
        }
    }
    public bool IsComplete
    {
        get { return isComplete; }
    }
    public bool IsColliding
    {
        get { return isColliding; }
    }
    public int[] Cost
    {
        get { return resourceCost; }
    }
    public Damageable DamageScript
    {
        get { return damageScript; }
    }

    private void Awake()
    {
        damageScript = GetComponent<Damageable>();
        buildingRender = transform.GetComponentInChildren<MeshRenderer>();
    }
    void Start()
    {
        if(damageScript)
        {
            isAttackable = true; // if component exists
        }
        isHovered = false;
        isAttackable = false;
        isPlacing = true;
        isComplete = false;
        workProgress = 0;
        percentComplete = 0.0f;
        // Colors
        normalEmmision = Color.black;
        hoveredEmmision = new Color(0.3f, 0.3f, 0.3f);
        // Building animation
        endPos = transform.localPosition;
        Vector3 startingOffset = Vector3.down * height;
        startPos = endPos + startingOffset;
        //transform.localPosition = startPos; // show
        builders = new List<Builder>();
    }

    public void Placing()
    {
        buildingRender.material = placingMaterial;
    }
    public void Place()
    {
        buildingRender.material = buildingMaterial;
    }
    public void FreeBuilders()
    {
        foreach(Builder builder in builders)
        {
            builder.Free();
        }
        builders.Clear();
    }
    public void Build(int work)
    {
        if(isComplete)
        {
            return;
        }

        workProgress += work; // Add progress
        percentComplete = (float)workProgress / totalWorkToComplete; // Update percentage
        // Check if completed
        if(percentComplete >= 1.0f)
        {
            CompleteBuild();
            return;
        }
        //transform.localPosition = Vector3.Lerp(startPos, endPos, percentComplete); // animation

        //visual
        //buildingTransform.DOComplete();
        //buildingTransform.DOShakeScale(.5f, .2f, 10, 90, true);
        //BuildingManager.Instance.PlayParticle(transform.position);
    }
    public bool CanBuild(int[] resources)
    {
        bool canBuild = true;
        for (int i = 0; i < resourceCost.Length; i++)
        {
            if (resources[i] < resourceCost[i])
            {
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }
    public bool AssignBuilder(Builder builder)
    {
        if(isComplete || builderAmount >= builderCapacity)
        {
            return false;
        }
        builderAmount++;
        builders.Add(builder);
        onCrewmateAssigned.Raise(this, builder);
        return true;
    }
    private void CompleteBuild()
    {
        isComplete = true;
        builderAmount = 0;
        buildingRender.material = builtMaterial;
        //buildingRender.material.DOColor(stateColors[1], "_EmissionColor", .1f).OnComplete(() => buildingRender.material.DOColor(stateColors[0], "_EmissionColor", .5f));
        //if (impulse)
        //    impulse.GenerateImpulse();
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        if (buildingRender && !BuildingUI.Instance.IsPlacing && !CameraManager.Instance.IsMouseMove)
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
