using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public TextMeshProUGUI tmpCrewmateAmt;
    public TextMeshProUGUI tmpCrewmateCapacity;
    public TextMeshProUGUI tmpFoodAmt;
    public TextMeshProUGUI tmpFoodConsumption;
    public TextMeshProUGUI tmpDubloonAmt;
    public TextMeshProUGUI tmpWoodAmt;
    public TextMeshProUGUI tmpStoneAmt;

    private void Start()
    {
        PlayerData data = GameManager.Data;
        //tmpCrewmateAmt.text = data.crewmates.Count.ToString();
        //tmpCrewmateCapacity.text = data.crewmates.Count.ToString();

        //Test values - TO BE REMOVED OR COMMENTED OUT
        tmpCrewmateAmt.text = 6.ToString();
        tmpCrewmateCapacity.text = 6.ToString();

        tmpFoodAmt.text = 0.ToString();
        tmpFoodConsumption.text = 0.ToString();
        tmpDubloonAmt.text = 0.ToString();
        tmpWoodAmt.text = 0.ToString();
    }

    public void Init()
    {
        //PlayerData data = GameManager.Data;
        //tmpCrewmateAmt.text = data.crewmates.Count.ToString();
        //tmpCrewmateCapacity.text = data.outpostCrewCapacity.ToString();
        //tmpFoodAmt.text = data.resources.food.ToString();
        //tmpFoodConsumption.text = data.resources.foodPerAP.ToString();
        //tmpDubloonAmt.text = data.resources.doubloons.ToString();
        //tmpWoodAmt.text = data.resources.wood.ToString();


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