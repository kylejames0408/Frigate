using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Move some individualstic data to scriptable objects, no need for prefabs atm
// Building objects are put together upon interaction
[Serializable]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshObstacle))]
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
    // Emissions - move to manager or set from manager
    private Color normalEmission = Color.black;
    private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);
    // Materials
    [SerializeField] private Material builtMaterial;
    // UI
    [SerializeField] private RectTransform canvasTrans;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool isPopUpOpen = false;
    private float animTimeInterface = 0.3f;
    [SerializeField] private TextMeshProUGUI uiLevel;
    [SerializeField] private TextMeshProUGUI uiName;
    [SerializeField] private Image uiStatus;
    [SerializeField] private TextMeshProUGUI uiAP;
    [SerializeField] private Image uiAsign1;
    [SerializeField] private Image uiAsign2;

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
        // Get/set components
        buildingRender = GetComponentInChildren<MeshRenderer>();
        navObsticle = GetComponent<NavMeshObstacle>();
        navObsticle.enabled = false; // prevents moving crewmates
        canvasGroup = canvasTrans.GetComponent<CanvasGroup>();

        // Set variables' initial state
        id = gameObject.GetInstanceID();
        uiIndex = -1;
        level = 0;
        state = BuildingState.PLACING;
        isHovered = false;
        numOfCollisions = 0;
    }
    private void Start()
    {
        // Set UI
        Vector3 lookARotation = canvasTrans.eulerAngles;
        lookARotation.x = CameraManager.Instance.Camera.transform.eulerAngles.x;
        canvasTrans.eulerAngles = lookARotation;
        canvasGroup.alpha = 0;
    }
    private void Update()
    {
        if (isHovered)
        {
            HandleSelection();
            HandleAssignment();
        }
    }

    public void SetMaterial(Material material)
    {
        buildingRender.material = material;
    }

    public void FillUI(Sprite stateIcon, Sprite assignmentIcon)
    {
        uiLevel.text = "Lv. " + level;
        uiName.text = buildingName;
        uiStatus.sprite = stateIcon;
        uiAP.text = resourceCost.ap.ToString();
        uiAsign1.sprite = assignmentIcon;
        uiAsign2.sprite = assignmentIcon;
    }

    public void Place()
    {
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
            //buildingRender.material = components.buildingMaterial;
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
    public string Upgrade()
    {
        // conditional
        level++;
        return GetStatus();
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
                break;
            case BuildingState.WAITING_FOR_ASSIGNMENT:
                break;
            case BuildingState.BUILDING:
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
        if (Input.GetMouseButtonDown(1) && CrewmateManager.Instance.selectedCrewmateIDs.Count > 0)
        {
            Crewmate mate = CrewmateManager.Instance.GetCrewmate(CrewmateManager.Instance.selectedCrewmateIDs[0]);
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
        canvasGroup.DOFade(1.0f, animTimeInterface);
    }
    private void OnMouseExit()
    {
        if (buildingRender && isHovered)
        {
            buildingRender.material.SetColor("_EmissionColor", normalEmission);
        }
        canvasGroup.DOFade(0.0f, animTimeInterface);
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
