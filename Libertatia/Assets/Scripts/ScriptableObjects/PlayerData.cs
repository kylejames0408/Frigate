using System;
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
public struct PlayerCrewData
{
    public int amount;
    public int capacity;
}
[Serializable]
public struct PlayerBuildingData
{
    public int amount;
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
    public PlayerCrewData crew;
    public PlayerBuildingData buildings;
}

// Manages player data - loading, saving, and converting the data
public class PlayerDataManager
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
    private const int STARTING_BUILDING_AMOUNT = 0;
    private PlayerData data;
    public PlayerData Data
    {
        get { return data; }
    }

    public PlayerDataManager()
    {
        if (!Fetch())
        {
            CreateNewData();
        }
    }

    // Creates new player data file and saves it to folder
    public void CreateNewData()
    {
        // Research difference b/w instance and default contructor
        data = ScriptableObject.CreateInstance<PlayerData>();
        data.gamePhase = GamePhase.MAIN_MENU;
        data.gameMode = GameMode.TUTORIAL;
        data.gameTimer = 0.0f;
        data.resources.wood = STARTING_WOOD_AMOUNT;
        data.resources.doubloons = STARTING_DOUBLOON_AMOUNT;
        data.resources.food = STARTING_FOOD_AMOUNT;
        data.resources.loyalty = STARTING_LOYALTY_AMOUNT;
        data.resources.foodPerAP = STARTING_FOOD_PER_AP;
        data.crew.amount = STARTING_CREW_AMOUNT;
        data.crew.capacity = STARTING_CREW_CAPACITY;
        data.buildings.amount = STARTING_BUILDING_AMOUNT;
        AssetDatabase.CreateAsset(data, TESTING_REL_PATH);
    }
    public bool Fetch()
    {
        data = AssetDatabase.LoadAssetAtPath<PlayerData>(TESTING_REL_PATH);
        return data != null;
    }
    public void Update(PlayerData data)
    {
        this.data = data; // how does this work
    }

    internal void UpdateTimer(float gameTimer)
    {
        data.gameTimer = gameTimer;
    }
}