using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
// Note
// could probably throw whole file into a preprocessor since it is benine code
// Couldnt figure out how to use dotween in unity editor
// Extend tutorial funcationallity to remove dialogue
// Remove all buildings (clear)
// Add ship controls such as depart

public class DevUI : MonoBehaviour
{
    private bool isOpen = false;
    private float interfaceAnimSpeed = 0.6f;

    [SerializeField] private BuildingManager bm;
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ResourceManager rm;

    [SerializeField] private Button btnArrow;
    [SerializeField] private Transform foodControls;
    [SerializeField] private Transform woodControls;
    [SerializeField] private Transform doubloonControls;
    [SerializeField] private Transform buildingControls;
    [SerializeField] private Transform crewmateControls;
    [SerializeField] private Transform tutorialControls;

    private Button[] foodButtons;
    private Button[] woodButtons;
    private Button[] doubloonButtons;
    private Button[] buildingButtons;
    private Button[] crewmateButtons;
    private Button[] tutorialButtons;

    private void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rm == null) { rm = FindObjectOfType<ResourceManager>(); }
        isOpen = false;

        btnArrow.onClick.AddListener(OpenCloseMenu);

        foodButtons = foodControls.GetComponentsInChildren<Button>(false);
        foodButtons[0].onClick.AddListener(() => { EditFood(100); });
        foodButtons[1].onClick.AddListener(() => { EditFood(-100); });

        woodButtons = woodControls.GetComponentsInChildren<Button>(false);
        woodButtons[0].onClick.AddListener(() => { EditWood(100); });
        woodButtons[1].onClick.AddListener(() => { EditWood(-100); });

        doubloonButtons = doubloonControls.GetComponentsInChildren<Button>(false);
        doubloonButtons[0].onClick.AddListener(() => { EditDoubloons(100); });
        doubloonButtons[1].onClick.AddListener(() => { EditDoubloons(-100); });

        buildingButtons = buildingControls.GetComponentsInChildren<Button>(false);
        buildingButtons[0].onClick.AddListener(CompleteAllBuildings);

        crewmateButtons = crewmateControls.GetComponentsInChildren<Button>(false);
        crewmateButtons[0].onClick.AddListener(() => { EditCrewmates(1); });
        crewmateButtons[1].onClick.AddListener(() => { EditCrewmates(-1); });

        tutorialButtons = tutorialControls.GetComponentsInChildren<Button>(false);
        tutorialButtons[0].onClick.AddListener(EndTutorial);

        // Had issues with this method
        //foodControls.Find("Add").GetComponent<Button>().onClick.AddListener(AddFood);
        //foodControls.Find("Subtract").GetComponent<Button>().onClick.AddListener(SubtractFood);
    }
    private void Update()
    {
        if(bm.Buildings.Length > 0)
        {
            buildingButtons[0].interactable = true;
        }
        else
        {
            buildingButtons[0].interactable = false;
        }
        if (cm.Crewmates.Length > 0)
        {
            crewmateButtons[1].interactable = true;
        }
        else
        {
            crewmateButtons[1].interactable = false;
        }
    }

    // Resource Controls
    private void EditFood(int amount)
    {
        rm.AddFood(amount);
    }
    private void EditWood(int amount)
    {
        rm.AddWood(amount);
    }
    private void EditDoubloons(int amount)
    {
        rm.AddDoubloons(amount);
    }
    // Building Controls
    private void CompleteAllBuildings()
    {
        bm.BuildAll();
    }
    // Crewmate Controls
    private void EditCrewmates(int amount)
    {
        cm.EditCrewmates(amount);
    }
    // Tutorial Controls
    private void EndTutorial()
    {
        GameManager.EndTutorial();
    }
    // Menu Function
    private void OpenCloseMenu()
    {
        if(isOpen)
        {
            isOpen = false;
            float menuWidth = transform.GetChild(1).GetComponent<RectTransform>().rect.width;
            transform.DOMoveX(Screen.width + menuWidth, interfaceAnimSpeed);
        }
        else
        {
            isOpen = true;
            transform.DOMoveX(Screen.width, interfaceAnimSpeed);
        }
    }
}
