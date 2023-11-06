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
    }
    private void Start()
    {

    }
    private void Update()
    {
        HandleClicking();
    }

    internal void FillUI(Crewmate mate)
    {

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
