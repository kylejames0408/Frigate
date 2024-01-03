using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    // References
    [SerializeField] private GameObject prefabAssigneeCard;
    [SerializeField] private Transform assigneeContainer;
    [SerializeField] private Ship ship;
    [SerializeField] private CrewmateUI crewmateUI;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private IslandUI islandUI;

    [Header("Static Data")]
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private Sprite iconEmptyAssignment;

    [Header("Tracking")] // Dynamic/tracking information
    private RectTransform bounds;
    [SerializeField] private int rowAmt = 2;
    [SerializeField] private int colAmt = 6;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private AssigneeCard[] assigneeCards;

    // UI
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnDepart;

    // Events
    public UnityEvent<int> onUnassign;


    private void Awake()
    {
        bounds = GetComponent<RectTransform>();
    }
    private void Start()
    {
        if(btnDepart != null)
        {
            btnDepart.onClick.AddListener(CeneManager.LoadExploration);
        }
        btnClose.onClick.AddListener(CloseMenu);
        isOpen = false;
    }
    private void Update()
    {
        if(isOpen)
        {
            HandleClicking();

            // Player must assign at least one crewmate to leave
            if(btnDepart)
            {
                if (ship.Crewmates.Length > 0)
                {
                    btnDepart.interactable = true;
                }
                else
                {
                    btnDepart.interactable = false;
                }
            }
        }
    }

    internal void Set(int crewSize)
    {
        rowAmt = crewSize/colAmt;

        assigneeCards = GetComponentsInChildren<AssigneeCard>();

        if (assigneeCards.Length != crewSize)
        {
            throw new Exception("Error: unmatching array sizes");
        }
    }
    internal void SetCrewmate(int index, ObjectData crewmate)
    {
        assigneeCards[index].Set(crewmate);
        if(assigneeCards[index].btnUnassign)
        {
            assigneeCards[index].btnUnassign.onClick.AddListener(() => { UnassignCallback(index, crewmate.id); }); //assigneeCards[assigneeIndex].CrewmateID
        }
    }
    internal void AddRow()
    {
        rowAmt++;
        // could probably turn into prefab
        GameObject row = new GameObject("Row" + rowAmt);
        row.transform.parent = assigneeContainer;
        HorizontalLayoutGroup horizontal = row.AddComponent<HorizontalLayoutGroup>();
        horizontal.childAlignment = TextAnchor.MiddleCenter;
        horizontal.childForceExpandHeight = false;
        horizontal.childForceExpandWidth = false;
        horizontal.childControlHeight = true;
        horizontal.childControlWidth = true;

        for (int i = 0; i < colAmt; i++)
        {
            Instantiate(prefabAssigneeCard, row.transform);
        }
    }

    // Callbacks
    private void UnassignCallback(int assigneeIndex, int crewmateID)
    {
        // Reset UI
        assigneeCards[assigneeIndex].ResetCard(iconEmptyAssignment);
        assigneeCards[assigneeIndex].btnUnassign.onClick.RemoveAllListeners();
        onUnassign.Invoke(crewmateID);
    }
    internal void ResetCard(int assigneeIndex)
    {
        assigneeCards[assigneeIndex].ResetCard(iconEmptyAssignment);
        assigneeCards[assigneeIndex].btnUnassign.onClick.RemoveAllListeners();
    }

    // Callbacks
    private void HandleClicking()
    {
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject() &&
            !ship.IsHovered && (
            Input.mousePosition.x < bounds.offsetMin.x ||
            Input.mousePosition.x > bounds.offsetMax.x ||
            Input.mousePosition.y < bounds.offsetMin.y ||
            Input.mousePosition.y > bounds.offsetMax.y))
        {
            CloseMenu();
        }
    }

    internal void OpenMenu()
    {
        transform.DOMoveX(690, animSpeedInterface);
        isOpen = true;
        if(crewmateUI)
        {
            crewmateUI.CloseInterface();
        }
        if(buildingUI)
        {
            buildingUI.CloseInterface();
        }
        if(islandUI)
        {
            islandUI.CloseInterface();
        }
    }
    internal void CloseMenu()
    {
        transform.DOMoveX(-10, animSpeedInterface);
        isOpen = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ShipUI))]
public class ShipUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShipUI shipUI = (ShipUI)target;

        if (GUILayout.Button("Add Row"))
        {
            shipUI.AddRow();
        }
    }
}
#endif
