using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// TODO: end tutorial - added to build all button, but should be its own
public class DevUI : MonoBehaviour
{
    private bool isOpen = false;
    private float interfaceAnimSpeed = 0.6f;

    [SerializeField] private BuildingManager bm;
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ResourceManager rm;

    [SerializeField] private Button btnArrow;
    [SerializeField] private Button btnBuildAll;
    [SerializeField] private Button btnAddWood;
    [SerializeField] private Button btnAddGold;
    [SerializeField] private Button btnNewMate;
    [SerializeField] private Button btnEndTutorial;

    private void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        isOpen = false;

        if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rm == null) { rm = FindObjectOfType<ResourceManager>(); }

        btnArrow.onClick.AddListener(OpenCloseMenu);
        btnBuildAll.onClick.AddListener(CompleteAllBuildings);
        btnAddWood.onClick.AddListener(AddWood);
        btnAddGold.onClick.AddListener(AddGold);
        btnNewMate.onClick.AddListener(NewMate);
        btnEndTutorial.onClick.AddListener(EndTutorial);
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
        rm.AddWood(100);
    }
    private void AddGold()
    {
        rm.AddDoubloons(100);
    }
    private void EndTutorial()
    {
        GameManager.EndTutorial();
    }

}
