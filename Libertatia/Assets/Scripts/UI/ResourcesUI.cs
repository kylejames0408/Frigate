using UnityEngine;
using TMPro;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpCrewmateAmt;
    [SerializeField] private TextMeshProUGUI tmpCrewmateCapacity;
    [SerializeField] private TextMeshProUGUI tmpFoodAmt;
    [SerializeField] private TextMeshProUGUI tmpFoodConsumption;
    [SerializeField] private TextMeshProUGUI tmpDubloonAmt;
    [SerializeField] private TextMeshProUGUI tmpWoodAmt;

    public void Init()
    {
        PlayerData data = GameManager.Data;
        tmpCrewmateAmt.text = data.crewmates.Count.ToString();
        tmpCrewmateCapacity.text = data.outpostCrewCapacity.ToString();
        tmpFoodAmt.text = data.resources.food.ToString();
        tmpFoodConsumption.text = data.resources.foodPerAP.ToString();
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
    public void UpdateFoodConsumptionUI(int consumption)
    {
        tmpFoodConsumption.text = consumption.ToString();
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