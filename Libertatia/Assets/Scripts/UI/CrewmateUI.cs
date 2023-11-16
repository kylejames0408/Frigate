using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrewmateUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CrewmateManager cm;

    [Header("Static Data")]
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private Sprite iconBuildingAssignedTo;

    [Header("Interface")] // Crewmate information objects
    [SerializeField] private Image uiCrewmateIcon;
    [SerializeField] private TextMeshProUGUI uiName;
    [SerializeField] private TextMeshProUGUI uiHealth;
    [SerializeField] private Image[] dotsStrength;
    [SerializeField] private Image[] dotsAgility;
    [SerializeField] private Image[] dotsStamina;
    [SerializeField] private Image uiBuildingIcon;

    [Header("Buttons")]
    [SerializeField] private Button btnClose;

    [Header("Tracking")] // Dynamic/tracking information
    private RectTransform bounds; // for clicking off

    private void Awake()
    {
        // Get component
        if(cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        bounds = GetComponent<RectTransform>();
        foreach (Image dot in dotsStrength)
        {
            dot.gameObject.SetActive(false);
        }
        foreach (Image dot in dotsAgility)
        {
            dot.gameObject.SetActive(false);
        }
        foreach (Image dot in dotsStamina)
        {
            dot.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        btnClose.onClick.AddListener(CloseMenuCallback);
    }
    private void Update()
    {
        HandleClicking();
    }

    internal void FillUI(Crewmate mate)
    {
        uiCrewmateIcon.sprite = mate.Icon;
        uiName.text = mate.FirstName;
        uiHealth.text = mate.Health.ToString();

        for (int i = 0; i < 5; i++)
        {
            if(i< mate.Strength)
            {
                dotsStrength[i].gameObject.SetActive(true);
            }
            else
            {
                dotsStrength[i].gameObject.SetActive(false);
            }

            if (i < mate.Agility)
            {
                dotsAgility[i].gameObject.SetActive(true);
            }
            else
            {
                dotsAgility[i].gameObject.SetActive(false);
            }

            if (i < mate.Stamina)
            {
                dotsStamina[i].gameObject.SetActive(true);
            }
            else
            {
                dotsStamina[i].gameObject.SetActive(false);
            }
        }
        uiBuildingIcon.sprite = mate.Building.icon;
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
            CloseMenuCallback();
        }
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
    private void CloseMenuCallback()
    {
        CloseMenu();
        cm.DeselectAllCrewmatesShare();
    }
}
