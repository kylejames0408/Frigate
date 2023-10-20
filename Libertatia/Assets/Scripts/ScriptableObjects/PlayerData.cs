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
    public int foodPerAP;
}
// Necessary building Data
[Serializable]
public struct BuildingData
{
    public int id;
    public int uiIndex; // type?
    public int level;
    public int assignedCrewmateID;
    public Vector3 position;
    public Quaternion rotation;
    public BuildingState state;
}
// Necessary crewmate Data
[Serializable]
public struct CrewmateData
{
    public string name;
    public Sprite icon;
    public int buildingID;
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
    public List<BuildingData> buildings;
    public List<CrewmateData> crewmates;
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
        data.resources.loyalty = STARTING_LOYALTY_AMOUNT;
        data.resources.foodPerAP = STARTING_FOOD_PER_AP;
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