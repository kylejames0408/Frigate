using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OutpostResourcesUI : MonoBehaviour
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

    public void Init(ResourceData resourceData, OutpostData outpostData)
    {
        tmpCrewmateCapacity.text = outpostData.crewCapacity.ToString();
        tmpFoodAmt.text = resourceData.food.ToString();
        UpdateFoodUI(resourceData);
        tmpDubloonAmt.text = resourceData.doubloons.ToString();
        tmpWoodAmt.text = resourceData.wood.ToString();
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
    public void UpdateFoodUI(ResourceData data)
    {
        tmpFoodProduction.text = data.production.food > 0 ? "+ " + data.production.food.ToString() : data.production.food.ToString();
        tmpFoodConsumption.text = (-data.consumption.food).ToString();
        int foodCalculation = data.production.food - data.consumption.food;
        tmpFoodCalculation.text = foodCalculation > 0 ? "+ " + foodCalculation.ToString() + " Per EP" : foodCalculation.ToString() + " Per EP";
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