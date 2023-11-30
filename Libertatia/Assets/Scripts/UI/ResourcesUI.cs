using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpCrewmateAmt;
    [SerializeField] private TextMeshProUGUI tmpCrewmateCapacity;
    [SerializeField] private TextMeshProUGUI tmpFoodAmt;
    [SerializeField] private TextMeshProUGUI tmpFoodTrend;
    [SerializeField] private TextMeshProUGUI tmpDubloonAmt;
    [SerializeField] private TextMeshProUGUI tmpWoodAmt;

    [SerializeField] private ResourceTab foodTab;
    // Popup
    [SerializeField] private GameObject foodPopupUI;
    [SerializeField] private TextMeshProUGUI tmpFoodProduction;
    [SerializeField] private TextMeshProUGUI tmpFoodConsumption;
    [SerializeField] private TextMeshProUGUI tmpFoodCalculation;
    [SerializeField] private float animTimeHoverInterface = 0.1f;

    [SerializeField] private Button btnMenu;
    [SerializeField] private PauseMenu uiPause;

    private void Awake()
    {
        foodTab.onHover.AddListener(ResourceHoveredCallback);
        foodTab.onHoverExit.AddListener(ResourceHoveredExitCallback);
        foodPopupUI.GetComponent<CanvasGroup>().alpha = 0;
    }
    private void Start()
    {
        btnMenu.onClick.AddListener(uiPause.Open);
    }

    public void Init()
    {
        PlayerData data = GameManager.Data;
        tmpCrewmateAmt.text = data.outpostCrew.Count.ToString();
        tmpCrewmateCapacity.text = data.outpostCrewCapacity.ToString();
        tmpFoodAmt.text = data.resources.food.ToString();
        UpdateFoodUI(data.resources);
        tmpDubloonAmt.text = data.resources.doubloons.ToString();
        tmpWoodAmt.text = data.resources.wood.ToString();
    }

    public void UpdateCrewAmountUI(int crewAmt)
    {
        tmpCrewmateAmt.text = crewAmt.ToString();
    }
    public void UpdateCrewCapacityUI(int crewCapacity)
    {
        tmpCrewmateCapacity.text = crewCapacity.ToString();
    }
    public void UpdateFoodAmountUI(int foodAmt)
    {
        tmpFoodAmt.text = foodAmt.ToString();
    }
    public void UpdateFoodUI(PlayerResourceData data)
    {
        tmpFoodProduction.text = data.foodProduction > 0 ? "+ " + data.foodProduction.ToString() : data.foodProduction.ToString();
        tmpFoodConsumption.text = (-data.foodConsumption).ToString();
        int foodCalculation = data.foodProduction - data.foodConsumption;
        tmpFoodCalculation.text = foodCalculation > 0 ? "+ " + foodCalculation.ToString() + " Per AP" : foodCalculation.ToString() + " Per AP";
        if (foodCalculation > 0)
        {
            tmpFoodTrend.text = "+";
        }
        else
        {
            tmpFoodTrend.text = "-";
        }
    }
    public void UpdateDubloonUI(int dubloonAmt)
    {
        tmpDubloonAmt.text = dubloonAmt.ToString();
    }
    public void UpdateWoodUI(int woodAmt)
    {
        tmpWoodAmt.text = woodAmt.ToString();
    }

    // Callbacks
    private void ResourceHoveredCallback()
    {
        foodPopupUI.GetComponent<CanvasGroup>().DOFade(1, animTimeHoverInterface);
    }
    private void ResourceHoveredExitCallback()
    {
        foodPopupUI.GetComponent<CanvasGroup>().DOFade(0, animTimeHoverInterface);
    }
}