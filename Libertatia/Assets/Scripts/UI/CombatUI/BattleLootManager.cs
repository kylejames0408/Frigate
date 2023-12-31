using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleLootManager : MonoBehaviour
{
    public int initDoubloonValue;
    public int initFoodValue;
    public int initWoodValue;
    public int initStoneValue;

    [SerializeField] private CombatResourcesUI combatResources;

    [SerializeField] private TextMeshProUGUI initDoubloonText;
    [SerializeField] private TextMeshProUGUI increasedDoubloonValue;
    [SerializeField] private TextMeshProUGUI currentDoubloonText;

    [SerializeField] private TextMeshProUGUI initFoodText;
    [SerializeField] private TextMeshProUGUI increasedFoodValue;
    [SerializeField] private TextMeshProUGUI currentFoodText;

    [SerializeField] private TextMeshProUGUI initWoodText;
    [SerializeField] private TextMeshProUGUI increasedWoodValue;
    [SerializeField] private TextMeshProUGUI currentWoodText;

    //[SerializeField] private TextMeshProUGUI initStoneText;
    //[SerializeField] private TextMeshProUGUI increasedStoneValue;
    //[SerializeField] private TextMeshProUGUI currentStoneText;

    PlayerData data = GameManager.Data;

    // Start is called before the first frame update
    void Start()
    {
        initDoubloonValue = data.resources.doubloons;
        initDoubloonText.text = initDoubloonValue.ToString();

        initFoodValue = data.resources.food;
        initFoodText.text = initFoodValue.ToString();

        initWoodValue = data.resources.wood;
        initWoodText.text = initWoodValue.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        //Updates current resource values
        CurrentResourceValue(increasedDoubloonValue, combatResources.doubloonAmount, currentDoubloonText, initDoubloonValue);
        CurrentResourceValue(increasedFoodValue, combatResources.foodAmount, currentFoodText, initFoodValue);
        CurrentResourceValue(increasedWoodValue, combatResources.woodAmount, currentWoodText, initWoodValue);
    }

    /// <summary>
    /// Updates the current resource value based on resources obtained in combat
    /// </summary>
    /// <param name="increasedValue"></param>
    /// <param name="combatResourceValue"></param>
    /// <param name="currentResourceValue"></param>
    /// <param name="initResourceValue"></param>
    public void CurrentResourceValue(TextMeshProUGUI increasedValue, int combatResourceValue, TextMeshProUGUI currentResourceValue, int initResourceValue)
    {
        increasedValue.text = "+ " + combatResourceValue.ToString();
        currentResourceValue.text = (initResourceValue + combatResourceValue).ToString();
    }

    public void ReturnToOutpost()
    {
        CeneManager.LoadOutpostFromCombat();
    }
}
