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
    public Button btnMenu;

    public void Init(int crewAmt = 0,
        int crewCapacity = 0,
        int foodAmt = 0,
        int foodConsumption = 0,
        int dubloonAmt = 0,
        int woodAmt = 0)
    {
        tmpCrewmateAmt.text = crewAmt.ToString();
        tmpCrewmateCapacity.text = crewCapacity.ToString();
        tmpFoodAmt.text = foodAmt.ToString();
        tmpFoodConsumption.text = foodConsumption.ToString();
        tmpDubloonAmt.text = dubloonAmt.ToString();
        tmpWoodAmt.text = woodAmt.ToString();
        //btnMenu.onclick
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