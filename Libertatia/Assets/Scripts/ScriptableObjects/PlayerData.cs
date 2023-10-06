﻿using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct Resources
{
    public int wood;
    public int food;
    public int gold;
    public int loyalty;
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Libertatia/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public float gameTimer;
    public int buildingAmount;  // amount of buildings; will be replaced idealy
    public int crewmateAmount;  // crew size; will be replaced with a list idealy
    public GamePhase gamePhase; // likely wont need
    public GameMode gameMode;
    public Resources resources;
}

public class PlayerDataManager
{
    private const string DIR_PATH = "Assets\\Scripts\\ScriptableObjects\\";
    private const string FILE_EXT = ".asset";
    private const string SAVED_FILE_NAME = "PlayerData";
    private const string SAVED_REL_PATH = DIR_PATH + SAVED_FILE_NAME + FILE_EXT;
    private const string STARTING_FILE_NAME = "StartingPlayerData";
    private const string STARTING_REL_PATH = DIR_PATH + STARTING_FILE_NAME + FILE_EXT;
    private const string TESTING_FILE_NAME = "PlayerDataTest";
    private const string TESTING_REL_PATH = DIR_PATH + TESTING_FILE_NAME + FILE_EXT;
    private PlayerData data;

    public PlayerData Data
    {
        get { return data; }
    }

    public PlayerDataManager(int startingCrewSize)
    {
        if (!Fetch())
        {
            CreateNewData(startingCrewSize);
        }
    }

    /// <summary>
    /// Creates new player data file and saves it to folder
    /// </summary>
    /// <param name="startingCrewSize"></param>
    public void CreateNewData(int startingCrewSize)
    {
        // Research difference b/w instance and default contructor
        data = ScriptableObject.CreateInstance<PlayerData>();
        data.gamePhase = GamePhase.OUTPOST;
        data.gameMode = GameMode.TUTORIAL;
        data.gameTimer = 0.0f;
        data.buildingAmount = 0;
        data.crewmateAmount = startingCrewSize; // determine this too
        //data.resources = initResources; // Determine starting resources
        AssetDatabase.CreateAsset(data, TESTING_REL_PATH);
    }
    public bool Fetch()
    {
        //data = UnityEngine.Resources.Load<PlayerData>(FILE_NAME + FILE_EXT); // backup method
        data = AssetDatabase.LoadAssetAtPath<PlayerData>(TESTING_REL_PATH); //STARTING_REL_PATH
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