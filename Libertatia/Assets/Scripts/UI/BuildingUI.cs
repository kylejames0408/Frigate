using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BuildingManager bm;

    [Header("Static Data")]
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private Sprite iconEmptyAssignment;

    [Header("Interface")] // Building information objects
    [SerializeField] private Image uiIcon;
    [SerializeField] private TextMeshProUGUI uiName;
    [SerializeField] private TextMeshProUGUI uiStatus;
    [SerializeField] private TextMeshProUGUI uiProduction;
    [SerializeField] private Image uiAsign1;
    [SerializeField] private Image uiAsign2;

    [Header("Buttons")]
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Button btnDemolish;
    [SerializeField] private Button btnClose;

    [Header("Tracking")] // Dynamic/tracking information
    [SerializeField] private int activeBuildingID;
    private RectTransform bounds;

    private void Awake()
    {
        // Get component
        if(bm == null) { bm = FindObjectOfType<BuildingManager>(); }
        bounds = GetComponent<RectTransform>();


        uiAsign2.transform.parent.gameObject.SetActive(false);
    }
    private void Start()
    {
        btnClose.onClick.AddListener(CloseMenu);
        btnUpgrade.onClick.AddListener(UpgradeCallback);
        btnDemolish.onClick.AddListener(DemolishCallback);
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
        uiProduction.text = building.resourceProduction.ToString();

        if (building.assignee1 != null)
        {
            uiAsign1.sprite = building.assignee1.Icon;
        }
        else
        {
            uiAsign1.sprite = iconEmptyAssignment;
        }

        if (building.IsBuilt)
        {
            uiAsign2.transform.parent.gameObject.SetActive(true);
            if (building.assignee2 != null)
            {
                uiAsign2.sprite = building.assignee2.Icon;
            }
            else
            {
                uiAsign2.sprite = iconEmptyAssignment;
            }
        }
        else
        {
            uiAsign2.transform.parent.gameObject.SetActive(false);
        }

        if (!GameManager.Data.isTutorial)
        {
            //upgradeBtn.interactable = true;
            btnDemolish.interactable = true;
        }

        activeBuildingID = building.id;
    }
    internal void SetStatusUI(string status)
    {
        uiStatus.text = status;
    }

    // Handlers
    private void HandleClicking()
    {
        if(Input.GetMouseButtonDown(0) && (
            Input.mousePosition.x < bounds.offsetMin.x ||
            Input.mousePosition.x > bounds.offsetMax.x ||
            Input.mousePosition.y < bounds.offsetMin.y ||
            Input.mousePosition.y > bounds.offsetMax.y))
        {
            CloseMenu();
        }
    }

    // Callbacks - probably get dir
    private void UpgradeCallback()
    {
        bm.UpgradeBuilding(activeBuildingID);
    }
    private void DemolishCallback()
    {
        bm.DemolishBuilding(activeBuildingID);
        CloseMenu();
    }
    private void AssignCallback()
    {
        //BuildingManager.Instance.AssignBuilding(activeBuildingID);
        // some information would need to be passed through
        //activeBuilding.AssignBuilder();
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
