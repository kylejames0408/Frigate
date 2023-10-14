using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Resources the player uses through the game
[Serializable]
public struct PlayerResourceData
{
    //public BuildingResources buildingResources; // maybe?
    public int wood;
    public int doubloons;
    public int food;
    public int loyalty;
    public int foodPerAP;
    public void Print()
    {
        Debug.LogFormat("Wood: {0}\nDubloons: {1}\nFood: {2}\nLoyalty: {3}",
            wood, doubloons, food, loyalty);
    }
}
[Serializable]
public struct CrewmateData
{
    public Crewmate script; // remove
    public string name;
    public Sprite icon;
    public int buildingID;
}

[Serializable]
public struct BuildingData
{
    public Building script; // remove
    public int id;
    public int uiIndex; // type?
    public int level;
    public int assignedCrewmateAmount;
    public Vector3 position;
    public Quaternion rotation;
    public BuildingState state;
}

// Holds important player data that should persist when game ends
[CreateAssetMenu(fileName = "PlayerData", menuName = "Libertatia/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    // Game data
    public float gameTimer;
    public GamePhase gamePhase; // likely wont need
    public GameMode gameMode;
    // Player data
    public PlayerResourceData resources;
    public List<CrewmateData> crewmates;
    public int outpostCrewCapacity;
    public List<BuildingData> buildings;
}

// Manages player data - loading, saving, and converting the data
public static class PlayerDataManager
{
    private const string DIR_PATH = "Assets\\Scripts\\ScriptableObjects\\";
    private const string FILE_EXT = ".asset";
    private const string SAVED_FILE_NAME = "PlayerData";
    private const string SAVED_REL_PATH = DIR_PATH + SAVED_FILE_NAME + FILE_EXT;
    private const string TESTING_FILE_NAME = "PlayerDataStart";
    private const string TESTING_REL_PATH = DIR_PATH + TESTING_FILE_NAME + FILE_EXT;
    private const int STARTING_WOOD_AMOUNT = 50;
    private const int STARTING_DOUBLOON_AMOUNT = 10;
    private const int STARTING_FOOD_AMOUNT = 100;
    private const int STARTING_FOOD_PER_AP = 5;
    private const int STARTING_LOYALTY_AMOUNT = 0;
    private const int STARTING_CREW_AMOUNT = 6;
    private const int STARTING_CREW_CAPACITY = 0;
    private static PlayerData data;
    public static PlayerData Data
    {
        get { return data; }
    }

    public static void Init()
    {
        //if (!Fetch())
        //{
        //    CreateNewData();
        //}
        CreateNewData();
    }
    // Creates new player data file and saves it to folder
    private static void CreateNewData()
    {
        // Research difference b/w instance and default contructor
        data = ScriptableObject.CreateInstance<PlayerData>();
        data.gamePhase = GamePhase.MAIN_MENU;
        data.gameMode = GameMode.TUTORIAL;
        data.gameTimer = 0.0f;
        //
        data.resources.wood = STARTING_WOOD_AMOUNT;
        data.resources.doubloons = STARTING_DOUBLOON_AMOUNT;
        data.resources.food = STARTING_FOOD_AMOUNT;
        data.resources.loyalty = STARTING_LOYALTY_AMOUNT;
        data.resources.foodPerAP = STARTING_FOOD_PER_AP;
        //
        data.crewmates = new List<CrewmateData>(STARTING_CREW_AMOUNT);
        data.outpostCrewCapacity = STARTING_CREW_CAPACITY;
        //
        data.buildings = new List<BuildingData>();
        //AssetDatabase.CreateAsset(data, TESTING_REL_PATH); // not saving for playtest
    }
    //private static bool Fetch()
    //{
    //    data = AssetDatabase.LoadAssetAtPath<PlayerData>(TESTING_REL_PATH); // not sure why this is build erroring
    //    return data != null;
    //}
}