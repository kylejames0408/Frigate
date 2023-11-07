using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Move some individualstic data to scriptable objects, no need for prefabs atm
// Building objects are put together upon interaction
[Serializable]
public class Building : MonoBehaviour
{
    // General Data
    public int id;
    [SerializeField] private string buildingName;
    [SerializeField] private Sprite icon;
    public string output;
    public int uiIndex;
    public int level;
    public BuildingState state = BuildingState.PLACING;
    // Identifiers
    public bool isHovered = false;
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
    [SerializeField] private Material matBuilt;
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
    // Cache
    private Sprite iconEmptyAsssignment;

    [Header("Events")]
    public UnityEvent onCrewmateAssigned;
    public UnityEvent onFirstAssignment;
    public UnityEvent onConstructionCompleted;
    public UnityEvent onSelection;
    public UnityEvent onCollision;
    public UnityEvent onNoCollisions;

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
    public bool IsBuilt
    {
        get { return state == BuildingState.BUILT; }
    }
    public BuildingResources Cost
    {
        get { return resourceCost; }
    }
    public bool IsColliding
    {
        get { return numOfCollisions > 0; }
    }

    private void Awake()
    {
        // Get/set components
        buildingRender = GetComponentInChildren<MeshRenderer>();
        navObsticle = GetComponentInChildren<NavMeshObstacle>();
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

    // UI
    public void SetUI(Sprite stateIcon, Sprite emptyAssignmentIcon)
    {
        iconEmptyAsssignment = emptyAssignmentIcon;
        uiLevel.text = "Lv. " + level.ToString();
        uiName.text = buildingName;
        uiStatus.sprite = stateIcon;
        uiAP.text = resourceCost.ap.ToString();
        uiAsign1.sprite = iconEmptyAsssignment;
        uiAsign2.sprite = iconEmptyAsssignment;
        uiAsign2.transform.parent.gameObject.SetActive(false);
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
        if (state == BuildingState.WAITING_FOR_ASSIGNMENT)
        {
            return !assignee1;
        }
        else if (state == BuildingState.BUILT)
        {
            return !assignee1 || !assignee2;
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
            case BuildingState.WAITING_FOR_ASSIGNMENT:
                value = "Needs assignment";
                break;
            case BuildingState.CONSTRUCTING:
                value = "Building";
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
        state = BuildingState.WAITING_FOR_ASSIGNMENT;
    }
    public void AssignCrewmate(Crewmate assignee)
    {
        // Assign
        if(assignee1 == null)
        {
            assignee1 = assignee;
            uiAsign1.sprite = assignee.Icon;
        }
        else if(assignee2 == null)
        {
            assignee2 = assignee;
            uiAsign2.sprite = assignee.Icon;
        }
        assignee.GiveJob(this);

        // Update building state
        if(state == BuildingState.WAITING_FOR_ASSIGNMENT)
        {
            onFirstAssignment.Invoke();
            state = BuildingState.CONSTRUCTING;
        }
        onCrewmateAssigned.Invoke();
    }
    public void CompleteConstruction()
    {
        state = BuildingState.BUILT;
        buildingRender.material = matBuilt;
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
        Destroy(gameObject);
    }
    private void FreeAssignees()
    {
        if (assignee1 != null)
        {
            assignee1.Free();
            assignee1 = null;
            uiAsign1.sprite = iconEmptyAsssignment;
        }
        if (assignee2 != null)
        {
            assignee2.Free();
            assignee2 = null;
            uiAsign1.sprite = iconEmptyAsssignment;
        }
    }

    // Handlers
    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && state != BuildingState.PLACING)
        {
            onSelection.Invoke();
        }
    }
    private void HandleAssignment()
    {
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && CrewmateManager.Instance.selectedCrewmateIDs.Count > 0)
        {
            Crewmate mate = CrewmateManager.Instance.GetCrewmate(CrewmateManager.Instance.selectedCrewmateIDs[0]);
            if (CanAssign())
            {
                AssignCrewmate(mate);
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
            canvasGroup.DOFade(1.0f, animTimeInterface);
        }
    }
    private void OnMouseExit()
    {
        if (buildingRender && isHovered)
        {
            buildingRender.material.SetColor("_EmissionColor", normalEmission);
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
        if (collision.transform.tag == "Building")
        {
            if(numOfCollisions == 0)
            {
                onCollision.Invoke(); // maybe give building the collision mat, but then placement angle logic would be separate
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
                if(state == BuildingState.BUILT)
                {
                    buildingRender.material = matBuilt;
                }
                else
                {
                    onNoCollisions.Invoke();
                }
            }
        }
    }
}
