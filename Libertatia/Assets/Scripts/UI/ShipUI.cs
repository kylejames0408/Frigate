using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    [Header("Tracking")] // Dynamic/tracking information
    private RectTransform bounds;

    [Header("Static Data")]
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private Sprite iconEmptyAssignment;

    [SerializeField] private GameObject prefabAssigneeCard;
    [SerializeField] private Transform assigneeContainer;
    [SerializeField] private int rowAmt = 2;
    [SerializeField] private int colAmt = 6;
    // UI
    [SerializeField] private AssigneeCard[] assigneeCards;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnDepart;


    private void Awake()
    {
        bounds = GetComponent<RectTransform>();
    }
    private void Start()
    {
        btnClose.onClick.AddListener(CloseMenu);
    }
    private void Update()
    {
        HandleClicking();
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

    internal void AddCrewmate(int index, ObjectData crewmate)
    {
        assigneeCards[index].Set(crewmate);
    }
    public void AddRow()
    {
        rowAmt++;
        GameObject row = Instantiate(new GameObject(), assigneeContainer);
        row.name = "Row" + rowAmt;
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

    private void HandleClicking()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && (
            Input.mousePosition.x < bounds.offsetMin.x ||
            Input.mousePosition.x > bounds.offsetMax.x ||
            Input.mousePosition.y < bounds.offsetMin.y ||
            Input.mousePosition.y > bounds.offsetMax.y))
        {
            //CloseMenu();
        }
    }

    internal void OpenMenu()
    {
        transform.DOMoveX(690, animSpeedInterface);
        Debug.Log("Open");
    }
    internal void CloseMenu()
    {
        transform.DOMoveX(-10, animSpeedInterface);
        Debug.Log("Close");
    }
}

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
