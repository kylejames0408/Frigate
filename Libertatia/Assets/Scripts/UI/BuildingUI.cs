using DG.Tweening;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Image uiAsign1;
    [SerializeField] private Image uiAsign2;

    [Header("Buttons")]
    public Button btnUpgrade;
    public Button btnDemolish;
    [SerializeField] private Button btnClose;

    [Header("Tracking")] // Dynamic/tracking information
    private RectTransform bounds;

    private void Awake()
    {
        // Get component
        bounds = GetComponent<RectTransform>();

        uiAsign2.transform.parent.gameObject.SetActive(false);
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
        if (building.Assignee1.id != -1)
        {
            uiAsign1.sprite = building.Assignee1.icon;
        }
        else
        {
            uiAsign1.sprite = iconEmptyAssignment;
        }

        if (building.IsBuilt)
        {
            uiAsign2.transform.parent.gameObject.SetActive(true);
            // or here if crewmate
            if (building.Assignee2.id != -1)
            {
                uiAsign2.sprite = building.Assignee2.icon;
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

    // Open/close
    internal void OpenMenu()
    {
        transform.DOMoveX(690, animSpeedInterface);
    }
    internal void CloseMenu()
    {
        transform.DOMoveX(-10, animSpeedInterface); // make relative
    }
}
