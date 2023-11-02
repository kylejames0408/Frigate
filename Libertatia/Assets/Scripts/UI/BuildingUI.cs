using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    private static BuildingUI instance;
    // Components
    [SerializeField] private BuildingManager bm;
    // ?
    private float interfaceAnimSpeed = 0.6f;
    [SerializeField] private Sprite emptyAssignmentIcon;
    // Building information objects
    [SerializeField] private Transform iconUI;
    [SerializeField] private Transform nameUI;
    [SerializeField] private Transform levelUI;
    [SerializeField] private Transform outputUI;
    //[SerializeField] private Transform assignmentUI; // will need this if more information is portrayed
    [SerializeField] private Transform assignment1IconUI;
    [SerializeField] private Transform assignment2IconUI;
    // Buttons
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Button demolishBtn;
    // Dynamic/tracking information
    private int activeBuildingID;
    private RectTransform bounds;

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
        assignment2IconUI.gameObject.SetActive(false);
    }
    private void Start()
    {
        closeBtn.onClick.AddListener(CloseMenu);
        upgradeBtn.onClick.AddListener(UpgradeCallback);
        demolishBtn.onClick.AddListener(DemolishCallback);
        if (GameManager.Data.isTutorial)
        {
            upgradeBtn.interactable = false;
            demolishBtn.interactable = false;
        }
    }
    private void Update()
    {
        HandleClicking();
    }

    public void FillUI(Building building)
    {
        iconUI.GetComponent<Image>().sprite = building.Icon;
        nameUI.GetComponent<TextMeshProUGUI>().text = building.Name;
        levelUI.GetComponent<TextMeshProUGUI>().text = building.GetStatus();
        outputUI.GetComponent<TextMeshProUGUI>().text = building.resourceProduction.ToString(); // not sure what output is yet

        if (building.assignee1 != null)
        {
            assignment1IconUI.GetComponent<Image>().sprite = building.assignee1.Icon;
        }
        else
        {
            assignment1IconUI.GetComponent<Image>().sprite = emptyAssignmentIcon;
        }

        if (building.IsComplete)
        {
            assignment2IconUI.gameObject.SetActive(true);
            if (building.assignee2 != null)
            {
                assignment2IconUI.GetComponent<Image>().sprite = building.assignee2.Icon;
            }
            else
            {
                assignment2IconUI.GetComponent<Image>().sprite = emptyAssignmentIcon;
            }
        }
        else
        {
            assignment2IconUI.gameObject.SetActive(false);
        }

        if (!GameManager.Data.isTutorial)
        {
            //upgradeBtn.interactable = true;
            demolishBtn.interactable = true;
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
        levelUI.GetComponent<TextMeshProUGUI>().text = bm.UpgradeBuilding(activeBuildingID);
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
        transform.DOMoveX(680, interfaceAnimSpeed);
    }
    public void CloseMenu()
    {
        transform.DOMoveX(0, interfaceAnimSpeed); // cant get height in start
    }
}
