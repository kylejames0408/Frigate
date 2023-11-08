using System;
using System.Collections.Generic;
using UnityEngine;

// Resources the player uses through the game
[Serializable]
public struct PlayerResourceData
{
    public int wood;
    public int doubloons;
    public int food;
    public int loyalty;

    public int foodProduction;
    public int foodConsumption;
}
// Necessary building Data
[Serializable]
public struct BuildingData
{
    // Tracking / State
    public int id;
    public AssigneeCrewmateData crewmate1ID;
    public AssigneeCrewmateData crewmate2ID;
    public BuildingState state;
    // Characteristics
    public string name;
    public int uiIndex;
    public int level;
    public string output;
    // UI
    public Sprite icon;
    // Spacial
    public Vector3 position;
    public Quaternion rotation;

    public BuildingData(Building building)
    {
        id = building.ID;
        crewmate1ID = building.Assignee1;
        crewmate2ID = building.Assignee2;
        state = building.State;
        name = building.Name;
        uiIndex = building.Type;
        level = building.Level;
        output = building.Output;
        icon = building.Icon;
        position = building.transform.position;
        rotation = building.transform.rotation;
    }
}
// Necessary crewmate Data
[Serializable]
public struct CrewmateData
{
    // Tracking / State
    public int id;
    public AssignedBuildingData building;
    // Characteristics
    public string name;
    public int health;
    public int strength;
    public int agility;
    public int stamina;
    // UI
    public Sprite icon;
    // Spacial
    public Vector3 position;
    public Quaternion rotation;

    public CrewmateData(Crewmate mate)
    {
        id = mate.ID;
        building = mate.Building;
        name = mate.Name;
        health = mate.Health;
        strength = mate.Strength;
        agility = mate.Agility;
        stamina = mate.Stamina;
        icon = mate.Icon;
        position = mate.transform.position;
        rotation = mate.transform.rotation;
    }
}
// Holds important player data that should persist when game ends
[Serializable]
public struct PlayerData
{
    // Game data
    public float gameTimer;
    public bool isTutorial;
    // Player data
    public PlayerResourceData resources;
    public int outpostCrewCapacity;
    // maybe make arrays or even lookup tables
    public List<BuildingData> buildings; // turn into dict
    public List<CrewmateData> crewmates; // same here
}

// Manages player data - creating and converting the data - should this be separate from data tracking?
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
    private const int STARTING_CREW_AMOUNT = 6;
    private const int STARTING_CREW_CAPACITY = 0;

    // Creates new player data file and fills starting information
    public static PlayerData CreateNewData()
    {
        PlayerData data;
        // Research difference b/w instance and default contructor
        data.isTutorial = true;
        data.gameTimer = 0.0f;
        // Resources
        data.resources.wood = STARTING_WOOD_AMOUNT;
        data.resources.doubloons = STARTING_DOUBLOON_AMOUNT;
        data.resources.food = STARTING_FOOD_AMOUNT;
        data.resources.foodProduction = 0;
        data.resources.foodConsumption = 0;
        data.resources.food = STARTING_FOOD_AMOUNT;
        data.resources.loyalty = STARTING_LOYALTY_AMOUNT;
        // Crewmates
        data.crewmates = new List<CrewmateData>(STARTING_CREW_AMOUNT);
        data.outpostCrewCapacity = STARTING_CREW_CAPACITY;
        // Buildings
        data.buildings = new List<BuildingData>();
        return data;
    }
    // TODO:
    // - Save data to a file
}