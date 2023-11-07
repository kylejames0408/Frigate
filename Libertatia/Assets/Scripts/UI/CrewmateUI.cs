using DG.Tweening;
using TMPro;
using UnityEngine;
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


    [Header("Tracking")] // Dynamic/tracking information
    [SerializeField] private int activeCrewmateID;
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
    private void Update()
    {
        HandleClicking();
    }

    internal void FillUI(Crewmate mate)
    {
        uiCrewmateIcon.sprite = mate.icon;
        uiName.text = mate.name;
        uiHealth.text = mate.health.ToString();
        for (int i = 0; i < mate.strength; i++)
        {
            dotsStrength[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < mate.agility; i++)
        {
            dotsAgility[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < mate.stamina; i++)
        {
            dotsStamina[i].gameObject.SetActive(true);
        }
        //uiBuildingIcon.sprite = uiBuildingIcon;
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
