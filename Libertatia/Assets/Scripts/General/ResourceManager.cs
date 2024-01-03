using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private OutpostResourcesUI outpostRUI;
    [SerializeField] private CombatResourcesUI combatRUI;
    [SerializeField] private ResourceData resources;

    private void Awake()
    {
        if (outpostRUI == null) { outpostRUI = FindObjectOfType<OutpostResourcesUI>(); }
    }
    private void Start()
    {
        resources = PlayerDataManager.LoadResourceData();
        OutpostData outpostData = PlayerDataManager.LoadOutpostData();

        if (GameManager.Phase == GamePhase.BUILDING)
        {
            outpostRUI.Init(resources, outpostData);
        }
        else if (GameManager.Phase == GamePhase.PLUNDERING)
        {
            combatRUI.Init(resources, outpostData);
        }
    }
    private void OnDestroy()
    {
        PlayerDataManager.SaveResourceData(resources);
    }

    internal void AddBuilding(BuildingResources cost, BuildingResources production)
    {
        resources.wood -= cost.wood;
        resources.food -= cost.food;
        resources.production.wood += production.wood;
        resources.production.food += production.food;

        outpostRUI.UpdateWoodUI(resources.wood);
        outpostRUI.UpdateFoodUI(resources);
    }
    internal void RemoveBuilding(BuildingResources cost, BuildingResources production)
    {
        resources.wood += cost.wood;
        resources.food += cost.food;
        resources.production.wood -= production.wood;
        resources.production.food -= production.food;

        outpostRUI.UpdateWoodUI(resources.wood);
        outpostRUI.UpdateFoodUI(resources);
    }

    // Figure out a different management method... might need to leave it to the BuildingManager to update - maybe make outpost manager?
    internal void UpdateHousingSpace(int housingSpace)
    {
        outpostRUI.UpdateCrewCapacityUI(housingSpace);
    }
    // Helpers
    internal bool CanConstruct(BuildingResources cost)
    {
        return resources.wood >= cost.wood &&
            resources.food >= cost.food;
    }

    internal void SpawnCrewmate(int foodConsumption)
    {
        resources.consumption.food += foodConsumption;
        outpostRUI.UpdateFoodUI(resources);
    }
    internal void CrewmateDied(int crewmateCount)
    {
        combatRUI.UpdateCrewAmountUI(crewmateCount);
    }

    internal void EnemyKilled(int lootValue)
    {
        resources.doubloons += lootValue;
        combatRUI.UpdateDubloonUI(resources.doubloons);
    }

    internal void CompleteIsland(IslandResources islandResources)
    {
        //resources += islandResources;
        resources.wood += islandResources.wood;
        resources.food += islandResources.food;
        resources.doubloons += islandResources.doubloons;
    }

    internal void ClearedZone(int woodAmount, int foodAmount) // make into struct
    {
        resources.wood += woodAmount;
        resources.food += foodAmount;
        combatRUI.UpdateWoodUI(resources.wood);
        combatRUI.UpdateFoodAmountUI(resources.food);
    }

    internal void AddWood(int woodAmount)
    {
        resources.wood += woodAmount;
        outpostRUI.UpdateWoodUI(resources.wood);
    }
    internal void AddFood(int foodAmount)
    {
        resources.food += foodAmount;
        outpostRUI.UpdateFoodAmountUI(resources.food);
    }
    internal void AddDoubloons(int doubloonAmount)
    {
        resources.doubloons += doubloonAmount;
        outpostRUI.UpdateDubloonUI(resources.doubloons);
    }
}
