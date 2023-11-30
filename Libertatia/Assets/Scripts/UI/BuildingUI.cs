using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    // References
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private CrewmateUI crewmateUI;

    [Header("Static Data")]
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private Sprite iconEmptyAssignment;

    [Header("Interface")] // Building information objects
    [SerializeField] private Image uiIcon;
    [SerializeField] private TextMeshProUGUI uiName;
    [SerializeField] private TextMeshProUGUI uiStatus;
    [SerializeField] private TextMeshProUGUI uiProduction;
    [SerializeField] private AssigneeCard[] assigneeCards;

    [Header("Buttons")]
    public Button btnUpgrade;
    public Button btnDemolish;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnBuilding;
    [SerializeField] private Button btnAssignee1;
    [SerializeField] private Button btnAssignee2;

    [Header("Tracking")] // Dynamic/tracking information
    private RectTransform bounds;
    private int buildingID = -1;
    [SerializeField] private bool isOpen = false;

    // Events
    public UnityEvent<int> onUnassign;
    public UnityEvent<int> onClickBuilding;
    public UnityEvent<int, int> onClickAssignee;

    private void Awake()
    {
        // Get component
        bounds = GetComponent<RectTransform>();

        assigneeCards[1].Disable();
    }
    private void Start()
    {
        btnClose.onClick.AddListener(CloseInterface);
        btnBuilding.onClick.AddListener(OnClickCrewmateCallback);
        btnAssignee1.onClick.AddListener(() =>{ OnClickAssigneeCallback(0); });
        btnAssignee2.onClick.AddListener(() =>{ OnClickAssigneeCallback(1); });

        if (GameManager.Data.isTutorial)
        {
            btnUpgrade.interactable = false;
            btnDemolish.interactable = false;
        }
        isOpen = false;
    }
    private void Update()
    {
        if (isOpen)
        {
            HandleClicking();
        }
    }

    internal void FillAndOpenInterface(Building building)
    {
        buildingID = building.ID;
        uiIcon.sprite = building.Icon;
        uiName.text = building.Name;
        uiStatus.text = building.GetStatus();
        uiProduction.text = building.Production.ToString();

        // Might not need the branch here
        if (!building.Assignee1.IsEmpty())
        {
            assigneeCards[0].Set(building.Assignee1);
            assigneeCards[0].btnUnassign.onClick.AddListener(() => { UnassignCallback(0, building.Assignee1.id); });
        }
        else
        {
            assigneeCards[0].ResetCard(iconEmptyAssignment);
        }

        if (building.IsBuilt)
        {
            assigneeCards[1].Enable();
            // or here if crewmate
            if (!building.Assignee2.IsEmpty())
            {
                assigneeCards[1].Set(building.Assignee2);
                assigneeCards[1].btnUnassign.onClick.AddListener(() => { UnassignCallback(1, building.Assignee2.id); });
            }
            else
            {
                assigneeCards[1].ResetCard(iconEmptyAssignment);
            }
        }
        else
        {
            assigneeCards[1].Disable();
        }

        if (!GameManager.Data.isTutorial)
        {
            btnDemolish.interactable = true;
        }

        if (!isOpen)
        {
            OpenInterface();
        }
    }
    internal void SetStatusUI(string status)
    {
        uiStatus.text = status;
    }

    // Handlers
    private void HandleClicking()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && (
            Input.mousePosition.x < bounds.offsetMin.x ||
            Input.mousePosition.x > bounds.offsetMax.x ||
            Input.mousePosition.y < bounds.offsetMin.y ||
            Input.mousePosition.y > bounds.offsetMax.y))
        {
            CloseInterface();
        }
    }

    // Callbacks
    private void UnassignCallback(int assigneeIndex, int crewmateID)
    {
        // Shift UI if crewmate is still in the second slot
        if (assigneeIndex == 0 && !assigneeCards[1].IsEmpty())
        {
            assigneeCards[0].Set(assigneeCards[1].CrewmateData);
            int tempCrewmateID = assigneeCards[1].CrewmateData.id;
            assigneeCards[0].btnUnassign.onClick.AddListener(() => { UnassignCallback(0, tempCrewmateID); });
            assigneeCards[1].ResetCard(iconEmptyAssignment);
        }
        else
        {
            assigneeCards[assigneeIndex].ResetCard(iconEmptyAssignment);
        }

        onUnassign.Invoke(crewmateID);
    }
    private void OnClickCrewmateCallback()
    {
        onClickBuilding.Invoke(buildingID);
    }
    private void OnClickAssigneeCallback(int assigneeIndex)
    {
        if (!assigneeCards[assigneeIndex].IsEmpty())
        {
            onClickAssignee.Invoke(buildingID, assigneeIndex);
        }
    }

    // Open/close
    internal void OpenInterface()
    {
        transform.DOMoveX(690, animSpeedInterface);
        isOpen = true;
        crewmateUI.CloseInterface();
        shipUI.CloseMenu();
    }
    internal void CloseInterface()
    {
        transform.DOMoveX(-10, animSpeedInterface); // make relative
        isOpen = false;
    }
}