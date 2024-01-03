using UnityEngine;
using TMPro;

public class CombatResourcesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpCrewmateAmt;
    [SerializeField] private TextMeshProUGUI tmpCrewmateCapacity;
    [SerializeField] private TextMeshProUGUI tmpFoodAmt;
    [SerializeField] private TextMeshProUGUI tmpFoodTrend;
    [SerializeField] private TextMeshProUGUI tmpDubloonAmt;
    [SerializeField] private TextMeshProUGUI tmpWoodAmt;

    public void Init(ResourceData resourceData, OutpostData outpostData)
    {
        tmpCrewmateAmt.text = outpostData.crew.Length.ToString();
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
        if (data.production.food - data.consumption.food > 0)
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
}