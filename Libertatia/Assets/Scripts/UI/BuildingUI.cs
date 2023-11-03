using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    private static BuildingUI instance;

    [Header("Components")]
    [SerializeField] private BuildingManager bm;

    [Header("Static Data")]
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private Sprite iconEmptyAssignment;

    [Header("Interface")] // Building information objects
    [SerializeField] private Transform uiIcon;
    [SerializeField] private Transform uiName;
    [SerializeField] private Transform uiLevel;
    [SerializeField] private Transform uiOutpost;
    [SerializeField] private Transform uiAsign1;
    [SerializeField] private Transform uiAsign2;

    [Header("Buttons")]
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Button btnDemolish;
    
    [Header("Tracking")] // Dynamic/tracking information
    [SerializeField] private int activeBuildingID;
    [SerializeField] private RectTransform bounds;

    public static BuildingUI Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        if(bm == null) { bm = FindObjectOfType<BuildingManager>(); }


        bounds = GetComponent<RectTransform>();
        uiAsign2.gameObject.SetActive(false);
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

    public void FillUI(Building building)
    {
        uiIcon.GetComponent<Image>().sprite = building.Icon;
        uiName.GetComponent<TextMeshProUGUI>().text = building.Name;
        uiLevel.GetComponent<TextMeshProUGUI>().text = building.GetStatus();
        uiOutpost.GetComponent<TextMeshProUGUI>().text = building.resourceProduction.ToString(); // not sure what output is yet

        if (building.assignee1 != null)
        {
            uiAsign1.GetComponent<Image>().sprite = building.assignee1.Icon;
        }
        else
        {
            uiAsign1.GetComponent<Image>().sprite = iconEmptyAssignment;
        }

        if (building.IsComplete)
        {
            uiAsign2.gameObject.SetActive(true);
            if (building.assignee2 != null)
            {
                uiAsign2.GetComponent<Image>().sprite = building.assignee2.Icon;
            }
            else
            {
                uiAsign2.GetComponent<Image>().sprite = iconEmptyAssignment;
            }
        }
        else
        {
            uiAsign2.gameObject.SetActive(false);
        }

        if (!GameManager.Data.isTutorial)
        {
            //upgradeBtn.interactable = true;
            btnDemolish.interactable = true;
        }

        activeBuildingID = building.id;
        OpenMenu();
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

    // Callbacks
    private void UpgradeCallback()
    {
        uiLevel.GetComponent<TextMeshProUGUI>().text = bm.UpgradeBuilding(activeBuildingID);
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
    public void OpenMenu()
    {
        transform.DOMoveX(680, animSpeedInterface);
    }
    public void CloseMenu()
    {
        transform.DOMoveX(0, animSpeedInterface); // cant get height in start
    }
}
