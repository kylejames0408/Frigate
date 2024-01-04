
using System;
using UnityEngine;

public static class PlayerDataManager
{
    private const string DIR_PATH = "Assets\\Scripts\\ScriptableObjects\\";
    private const string FILE_EXT = ".asset";
    private const string SAVED_FILE_NAME = "PlayerData";
    private const string SAVED_REL_PATH = DIR_PATH + SAVED_FILE_NAME + FILE_EXT;
    private const int STARTING_WOOD_AMOUNT = 50;
    private const int STARTING_DOUBLOON_AMOUNT = 10;
    private const int STARTING_FOOD_AMOUNT = 100;
    private const int STARTING_FOOD_PER_AP = 5;
    private const int STARTING_LOYALTY_AMOUNT = 0;
    private const int STARTING_OUTPOST_CREW_AMOUNT = 0;
    private const int STARTING_SHIP_CREW_CAPACITY = 12;
    public const int STARTING_CREW_AMOUNT = 6;

    private static PlayerData data;
    private static bool isInitialized = false;

    public static bool IsInitialized
    {
        get { return isInitialized; }
    }

    // Game Management
    internal static void NewGame()
    {
        // Init PlayerData
        data.elapsedTime = 0.0f;
        data.hasCompletedTutorial = false;
        data.resources = new ResourceData(
            STARTING_WOOD_AMOUNT,
            STARTING_DOUBLOON_AMOUNT,
            STARTING_FOOD_AMOUNT,
            STARTING_LOYALTY_AMOUNT);
        data.outpost = new OutpostData(STARTING_OUTPOST_CREW_AMOUNT);
        data.ship = new ShipData(STARTING_SHIP_CREW_CAPACITY);
        isInitialized = true;
    }
    internal static void LoadGame()
    {
        // Logic for loading a game
        isInitialized = true;
    }
    internal static void SaveGame()
    {
        // Logic for saving a game
    }

    // Loading
    internal static OutpostData LoadOutpostData()
    {
        return data.outpost;
    }
    internal static int LoadOutpostCrewCapacity()
    {
        return data.outpost.crewCapacity;
    }
    internal static BuildingData[] LoadOutpostBuildings()
    {
        return data.outpost.buildings;
    }

    internal static ResourceData LoadResourceData()
    {
        return data.resources;
    }
    internal static ResourceProduction LoadResourceProduction()
    {
        return data.resources.production;
    }
    internal static ResourceConsumption LoadResourceConsumption()
    {
        return data.resources.consumption;
    }

    internal static ShipData LoadShipData()
    {
        return data.ship;
    }

    // Saving
    internal static void SaveOutpostData(Building[] buildings, int housingSpace)
    {
        data.outpost.buildings = new BuildingData[buildings.Length];
        for (int i = 0; i < buildings.Length; i++)
        {
            data.outpost.buildings[i] = new BuildingData(buildings[i]);
        }
        data.outpost.crewCapacity = housingSpace;
    }
    internal static void SaveOutpostData(Crewmate[] crewmates)
    {

    }
    internal static void SaveOutpostCrewCapacity(int housingSpace)
    {
        data.outpost.crewCapacity = housingSpace;
    }
    internal static void SaveOutpostBuildings(Building[] buildings)
    {
        data.outpost.buildings = new BuildingData[buildings.Length];
        for (int i = 0; i < buildings.Length; i++)
        {
            data.outpost.buildings[i] = new BuildingData(buildings[i]);
        }
    }

    internal static void SaveResourceData(ResourceData resources)
    {
        data.resources = resources;
    }
    internal static void SaveResourceProduction(ResourceProduction resourceProduction)
    {
        data.resources.production = resourceProduction;
    }
    internal static void SaveResourceConsumption(ResourceConsumption resourceConsumptionn)
    {
        data.resources.consumption = resourceConsumptionn;
    }

    internal static void SaveOutpostCrewmates(Crewmate[] crewmates)
    {
        data.outpost.crew = new CrewmateData[crewmates.Length];
        for (int i = 0; i < crewmates.Length; i++)
        {
            data.outpost.crew[i] = new CrewmateData(crewmates[i]);
        }
    }

    internal static void SaveShipData(Ship ship)
    {
        data.ship = new ShipData(ship);
    }

    // General
    internal static void SaveTutorialProgress(bool isTutorialComplete)
    {
        data.hasCompletedTutorial = isTutorialComplete;
    }
    internal static void SaveTimestamp(float elapsedTime)
    {
        data.elapsedTime = elapsedTime;
    }
}
