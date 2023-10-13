using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OutpostUI : MonoBehaviour
{
    public TextMeshProUGUI tmpCrewmateAmt;
    public TextMeshProUGUI tmpCrewmateCapacity;
    public TextMeshProUGUI tmpFoodAmt;
    public TextMeshProUGUI tmpFoodConsumption;
    public TextMeshProUGUI tmpDubloonAmt;
    public TextMeshProUGUI tmpWoodAmt;

    public void Init()
    {
        PlayerData data = GameManager.Instance.DataManager.Data;
        tmpCrewmateAmt.text = data.crew.amount.ToString();
        tmpCrewmateCapacity.text = data.crew.capacity.ToString();
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