using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
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

    [Header("Tracking")] // Dynamic/tracking information
    private RectTransform bounds;

    // Events
    public UnityEvent<int> onUnassign;

    private void Awake()
    {
        // Get component
        bounds = GetComponent<RectTransform>();

        assigneeCards[1].Disable();
    }
    private void Start()
    {
        btnClose.onClick.AddListener(CloseMenu);

        if (GameManager.Data.isTutorial)
        {
            btnUpgrade.interactable = false;
            btnDemolish.interactable = false;
        }
    }
    private void Update()
    {
        HandleClicking();
    }

    internal void FillUI(Building building)
    {
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
    }
    internal void SetStatusUI(string status)
    {
        uiStatus.text = status;
    }

    // Handlers
    private void HandleClicking()
    {
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && (
            Input.mousePosition.x < bounds.offsetMin.x ||
            Input.mousePosition.x > bounds.offsetMax.x ||
            Input.mousePosition.y < bounds.offsetMin.y ||
            Input.mousePosition.y > bounds.offsetMax.y))
        {
            CloseMenu();
        }
    }

    // Callbacks
    private void UnassignCallback(int assigneeIndex, int crewmateID)
    {
        // Shift UI if crewmate is still in the second slot
        if(assigneeIndex == 0 && !assigneeCards[1].IsEmpty())
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

    // Open/close
    internal void OpenMenu()
    {
        transform.DOMoveX(680, animSpeedInterface);
    }
    internal void CloseMenu()
    {
        transform.DOMoveX(0, animSpeedInterface); // cant get height in start
    }
}
