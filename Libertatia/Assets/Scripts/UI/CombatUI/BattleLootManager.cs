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

    [SerializeField] private GameObject battleLootUI;

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

    private bool updateResourceBool;

    private int doubloonAmount, foodAmount, woodAmount;

    // Start is called before the first frame update
    void Start()
    {
        initDoubloonValue = data.resources.doubloons;
        initDoubloonText.text = initDoubloonValue.ToString();

        initFoodValue = data.resources.food;
        initFoodText.text = initFoodValue.ToString();

        initWoodValue = data.resources.wood;
        initWoodText.text = initWoodValue.ToString();

        updateResourceBool = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Updates current resource values
        CurrentResourceValue(increasedDoubloonValue, combatResources.doubloonAmount, currentDoubloonText, initDoubloonValue);
        CurrentResourceValue(increasedFoodValue, combatResources.foodAmount, currentFoodText, initFoodValue);
        CurrentResourceValue(increasedWoodValue, combatResources.woodAmount, currentWoodText, initWoodValue);

        if(updateResourceBool == false)
        {
            StartCoroutine(UpdateResourceValues());
        }

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

    /// <summary>
    /// Updates the resources in the battle loot ui by showing the increase in numbers
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateResourceValues()
    {
        if (battleLootUI.activeSelf)
        {
            updateResourceBool = true;

            doubloonAmount = combatResources.doubloonAmount;
            woodAmount = combatResources.woodAmount;
            foodAmount = combatResources.foodAmount;

            //Wait before updating the resource values
            yield return new WaitForSeconds(1f);

            //doubloons
            for (int i = 0; i < doubloonAmount; i++)
            {
                yield return new WaitForSeconds(0.05f);

                combatResources.doubloonAmount -= 1;
                initDoubloonValue += 1;

                initDoubloonText.text = initDoubloonValue.ToString();
                CurrentResourceValue(increasedDoubloonValue, combatResources.doubloonAmount, currentDoubloonText, initDoubloonValue);
            }

            //wood
            for (int i = 0; i < woodAmount; i++)
            {
                yield return new WaitForSeconds(0.05f);

                combatResources.woodAmount -= 1;
                initWoodValue += 1;

                initWoodText.text = initWoodValue.ToString();
                CurrentResourceValue(increasedWoodValue, combatResources.woodAmount, currentWoodText, initWoodValue);
            }

            //food
            for (int i = 0; i < foodAmount; i++)
            {
                yield return new WaitForSeconds(0.05f);

                combatResources.foodAmount -= 1;
                initFoodValue += 1;

                initFoodText.text = initFoodValue.ToString();
                CurrentResourceValue(increasedFoodValue, combatResources.foodAmount, currentFoodText, initFoodValue);
            }

            updateResourceBool = false;
        }
    }
}
