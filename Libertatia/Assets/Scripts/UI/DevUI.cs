using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DevUI : MonoBehaviour
{
    private bool isOpen = false;
    private float interfaceAnimSpeed = 0.6f;

    [SerializeField] private BuildingManager bm;
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ResourcesUI rui;

    [SerializeField] private Button btnArrow;
    [SerializeField] private Button btnBuildAll;
    [SerializeField] private Button btnAddWood;
    [SerializeField] private Button btnAddGold;
    [SerializeField] private Button btnNewMate;

    private void Awake()
    {
        isOpen = false;

        if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rui == null) { rui = FindObjectOfType<ResourcesUI>(); }

        btnArrow.onClick.AddListener(OpenCloseMenu);
        btnBuildAll.onClick.AddListener(CompleteAllBuildings);
        btnAddWood.onClick.AddListener(AddWood);
        btnAddGold.onClick.AddListener(AddGold);
        btnNewMate.onClick.AddListener(NewMate);
    }

    private void OpenCloseMenu()
    {
        if(isOpen)
        {
            isOpen = false;
            transform.DOMoveX(331 + Screen.width, interfaceAnimSpeed); // doesont do local
        }
        else
        {
            isOpen = true;
            transform.DOMoveX(Screen.width, interfaceAnimSpeed);
        }
    }
    // Manager
    private void CompleteAllBuildings()
    {
        bm.BuildAll();
    }
    private void NewMate()
    {
        cm.SpawnNewCrewmate();
    }
    // UI
    private void AddWood()
    {
        GameManager.data.resources.wood += 100;
        rui.UpdateWoodUI(GameManager.data.resources.wood);
    }
    private void AddGold()
    {
        GameManager.data.resources.doubloons += 100;
        rui.UpdateDubloonUI(GameManager.data.resources.doubloons);
    }

}
