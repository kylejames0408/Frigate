using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class Building : MonoBehaviour
{
    // Tracking
    [SerializeField] private int id = -1;
    [SerializeField] private ObjectData[] assignees = new ObjectData[2];
    [SerializeField] private BuildingState state = BuildingState.PLACING;
    [SerializeField] private BuildingResources resourceCost; // move to scriptable object as "rules", will change with level however
    [SerializeField] private BuildingResources resourceProduction; // same here
    // Characteristics
    [SerializeField] private string buildingName;
    [SerializeField] private int uiIndex = -1; // Type of building
    [SerializeField] private int level = 0;
    [SerializeField] private string output;
    // UI
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isHovered = false;
    // In-game characteristics
    [SerializeField] private float radius = 5.0f; // for construction
    [SerializeField] private int numOfCollisions = 0;
    // Components
    [SerializeField] private MeshRenderer buildingRenderer;
    [SerializeField] private NavMeshObstacle navObsticle;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private LineRenderer lineRenderer;
    // Emissions - move to manager or set from manager
    [SerializeField] private Color normalEmission = Color.black;
    [SerializeField] private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);
    // Materials
    [SerializeField] private Material matBuilt;
    // UI
    [SerializeField] private RectTransform canvasTrans;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool isPopUpOpen = false;
    [SerializeField] private float animTimeInterface = 0.3f;
    [SerializeField] private TextMeshProUGUI uiLevel;
    [SerializeField] private TextMeshProUGUI uiName;
    [SerializeField] private Image uiStatus;
    [SerializeField] private Image uiAsign1;
    [SerializeField] private Image uiAsign2;
    // Cache
    [SerializeField] private Sprite iconEmptyAsssignment;

    [Header("Events")]
    public UnityEvent onCrewmateAssigned;
    public UnityEvent onDemolish;
    public UnityEvent onFreeAssignees;
    public UnityEvent onFirstAssignment;
    public UnityEvent onConstructionCompleted;
    public UnityEvent onSelection;
    public UnityEvent onCollision;
    public UnityEvent onNoCollisions;

    public int ID
    {
        get { return id; }
    }
    public ObjectData[] Assignees
    {
        get { return assignees; }
    }
    public ObjectData Assignee1
    {
        get { return assignees[0]; }
    }
    public ObjectData Assignee2
    {
        get { return assignees[1]; }
    }
    public BuildingState State
    {
        get { return state; }
    }
    public BuildingResources Cost
    {
        get { return resourceCost; }
    }
    public BuildingResources Production
    {
        get { return resourceProduction; }
    }
    public string Name
    {
        get { return buildingName; }
    }
    public int Type
    {
        get { return uiIndex; }
    }
    public int Level
    {
        get { return level; }
    }
    public string Output
    {
        get { return output; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public bool IsBuilt
    {
        get { return state == BuildingState.BUILT; }
    }
    public bool IsColliding
    {
        get { return numOfCollisions > 0; }
    }

    private void Awake()
    {
        // Get/set components
        buildingRenderer = GetComponentInChildren<MeshRenderer>();
        navObsticle = GetComponentInChildren<NavMeshObstacle>();
        boxCollider = GetComponent<BoxCollider>();
        lineRenderer = GetComponent<LineRenderer>();
        navObsticle.enabled = false; // prevents moving crewmates
        canvasGroup = canvasTrans.GetComponent<CanvasGroup>();

        // Set variables' initial state
        id = gameObject.GetInstanceID();
        assignees[0] = new ObjectData(-1, iconEmptyAsssignment);
        assignees[1] = new ObjectData(-1, iconEmptyAsssignment);
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

        // Render line - bounds.extent calculations are off?
        float halfWidth = boxCollider.size.x/2.0f;
        float halfHeight = boxCollider.size.y/2.0f;
        float halfDepth = boxCollider.size.z/2.0f;
        Vector3 topRight = new Vector3(halfWidth + boxCollider.center.x, halfHeight + boxCollider.center.y, halfDepth + boxCollider.center.z);
        Vector3 topLeft = new Vector3(-halfWidth + boxCollider.center.x, halfHeight + boxCollider.center.y, halfDepth + boxCollider.center.z);
        Vector3 bottomLeft = new Vector3(-halfWidth + boxCollider.center.x, halfHeight + boxCollider.center.y, -halfDepth + boxCollider.center.z);
        Vector3 bottomRight = new Vector3(halfWidth + boxCollider.center.x, halfHeight + boxCollider.center.y, -halfDepth + boxCollider.center.z);
        Vector3[] boarderPositions =
        {
            topRight, topLeft, bottomLeft, bottomRight
        };
        lineRenderer.positionCount = 4;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPositions(boarderPositions);
        float lineThickness = 0.1f;
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }
    private void Update()
    {
        if (isHovered)
        {
            HandleSelection();
            //HandleAssignment();
        }
    }

    internal void Init(BuildingData data)
    {
        // Tracking / State
        id = data.id;
        assignees[0] = data.assignee1;
        assignees[1] = data.assignee2;
        state = data.state;
        // Characteristics
        buildingName = data.name;
        uiIndex = data.uiIndex;
        level = data.level;
        output = data.output;
        // UI
        icon = data.icon;
        // Spacial
        transform.position = data.position;
        transform.rotation = data.rotation;
    }

    // Setters
    public void SetMaterial(Material material)
    {
        buildingRenderer.material = material;
    }
    public void SetType(int type)
    {
        uiIndex = type;
    }

    // UI
    public void SetUI(Sprite stateIcon, Sprite emptyAssignmentIcon)
    {
        iconEmptyAsssignment = emptyAssignmentIcon;
        uiLevel.text = "Lv. " + level.ToString();
        uiName.text = buildingName;
        uiStatus.sprite = stateIcon; // might not need if Init is done before

        // Should be moved somewhere else
        if(state == BuildingState.BUILT || state == BuildingState.CONSTRUCTING)
        {
            uiAsign1.sprite = assignees[0].icon;
            uiAsign2.sprite = assignees[1].icon;
        }
        else if (state == BuildingState.RECRUIT)
        {
            uiAsign1.sprite = iconEmptyAsssignment;
            uiAsign2.sprite = iconEmptyAsssignment;
            uiAsign2.transform.parent.gameObject.SetActive(false);
            assignees[0].Reset(iconEmptyAsssignment);
            assignees[1].Reset(iconEmptyAsssignment);
        }
    }
    private void SetLevelUI(int level)
    {
        uiLevel.text = "Lv. " + level.ToString();
    }
    public void SetStatusIcon(Sprite stateIcon)
    {
        uiStatus.sprite = stateIcon;
    }

    // Requests
    public bool CanAssign()
    {
        if (state == BuildingState.RECRUIT)
        {
            return assignees[0].id == -1;
        }
        else if (state == BuildingState.BUILT)
        {
            return assignees[0].id == -1 || assignees[1].id == -1;
        }
        else
        {
            Debug.LogWarning("Building is not in a state to assign");
            return false;
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
            case BuildingState.RECRUIT:
                value = "Recruit";
                break;
            case BuildingState.CONSTRUCTING:
                value = "Constructing";
                break;
            case BuildingState.BUILT:
                value = "Level " + level.ToString();
                break;
        }
        return value;
    }

    // Actions
    public void Place()
    {
        navObsticle.enabled = true;
        state = BuildingState.RECRUIT;
    }
    public void AssignCrewmate(Crewmate mate)
    {
        if (assignees[0].id == -1)
        {
            assignees[0].id = mate.ID;
            assignees[0].icon = mate.Icon;
        }
        else if(assignees[1].id == -1)
        {
            assignees[1].id = mate.ID;
            assignees[1].icon = mate.Icon;
        }

        // Calculate target destination
        mate.Assign(id, icon,  GetDestination());

        // Update building state
        if(state == BuildingState.RECRUIT)
        {
            onFirstAssignment.Invoke();
            state = BuildingState.CONSTRUCTING;
        }

        // Update UI
        uiAsign1.sprite = assignees[0].icon;
        uiAsign2.sprite = assignees[1].icon;
    }
    public void CompleteConstruction()
    {
        state = BuildingState.BUILT;
        buildingRenderer.material = matBuilt;
        Upgrade();
        navObsticle.enabled = true; // maybe cache? I am not sure why this is disabled when coming back to outpost
        uiAsign2.transform.parent.gameObject.SetActive(true);
        FreeAssignees();
        onConstructionCompleted.Invoke();
    }
    public void Upgrade()
    {
        level++;
        SetLevelUI(level);
    }
    public void Demolish()
    {
        FreeAssignees();
        onDemolish.Invoke();
        Destroy(gameObject);
    }
    private void FreeAssignees()
    {
        onFreeAssignees.Invoke();

        if (assignees[0].id != -1)
        {
            assignees[0].Reset(iconEmptyAsssignment);
        }
        if (assignees[1].id != -1)
        {
            assignees[1].Reset(iconEmptyAsssignment);
        }

        // Update UI
        uiAsign1.sprite = assignees[0].icon;
        uiAsign2.sprite = assignees[1].icon;
    }

    // Util
    private Vector3 GetDestination()
    {
        // Get random position around unit circle
        Vector2 circlePosition = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 destinationOffset = new Vector3(circlePosition.x * radius, 0, -(Mathf.Abs(circlePosition.y)) * radius); // keeps crewmate in-front of the building, maybe tweak
        return transform.position + destinationOffset;
    }
    internal void UnassignCrewmate(int crewmateID)
    {
        if (assignees[0].id == crewmateID)
        {
            if(assignees[1].id == -1)
            {
                assignees[0].Reset(iconEmptyAsssignment);
            }
            else
            {
                assignees[0] = assignees[1];
                assignees[1].Reset(iconEmptyAsssignment);
            }

            // Change state if nobody is assigned
            if (state == BuildingState.CONSTRUCTING)
            {
                state = BuildingState.RECRUIT;
            }
        }
        else if (assignees[1].id == crewmateID)
        {
            assignees[1].Reset(iconEmptyAsssignment);
        }

        // Update UI
        uiAsign1.sprite = assignees[0].icon;
        uiAsign2.sprite = assignees[1].icon;
    }

    // Handlers
    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && state != BuildingState.PLACING)
        {
            onSelection.Invoke();
        }
    }
    public void HandleAssignmentDragDrop()
    {
        if (isHovered)
        {
            if (CanAssign())
            {
                onCrewmateAssigned.Invoke();
            }
            else
            {
                Debug.Log("Building assignments are full");
            }
        }
    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            isHovered = true;
            if (state != BuildingState.PLACING)
            {
                buildingRenderer.material.SetColor("_EmissionColor", hoveredEmission);
                canvasGroup.DOFade(1.0f, animTimeInterface);
            }
        }
    }
    private void OnMouseExit()
    {
        if (buildingRenderer && isHovered)
        {
            buildingRenderer.material.SetColor("_EmissionColor", normalEmission);
            canvasGroup.DOFade(0.0f, animTimeInterface);
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
        if (collision.transform.tag == "Building" && state == BuildingState.PLACING)
        {
            if(numOfCollisions == 0)
            {
                onCollision.Invoke();
            }

            numOfCollisions++;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Building" && state == BuildingState.PLACING)
        {
            numOfCollisions--;

            if (numOfCollisions == 0)
            {
                onNoCollisions.Invoke();
            }
        }
    }
}
