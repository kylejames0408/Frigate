using System;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    PLAYING,
    PAUSED
}
public enum GamePhase
{
    MAIN_MENU,
    OUTPOST,
    ENEMY_TERRITORY
}

public class GameManager : MonoBehaviour
{
    // Game Data
    private float gameTimer = 0.0f;
    private GameState state = GameState.PLAYING;
    public static int outpostVisitNumber = 0;
    public static int combatVisitNumber = 0;
    public static PlayerData data;
    public static bool MainMenuTesting = false;
    public static bool OutpostTesting = false;
    public static bool CombatTesting = false;

    public UnityEvent onAttack;

    // Player data management - maybe move to manager, if so, will storage persist?
    public static PlayerData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }
    public static void AddBuilding(BuildingData data)
    {
        Data.buildings.Add(data); // why is this.data not working???
    }
    public static void RemoveBuilding(int buildingID)
    {
        foreach (BuildingData data in Data.buildings)
        {
            if(data.id == buildingID)
            {
                Data.buildings.Remove(data);
                return;
            }
        }
    }
    public static void AddCrewmate(CrewmateData data)
    {
        Data.crewmates.Add(data);
    }
    internal static void RemoveCrewmateData(int crewmateID)
    {
        foreach (CrewmateData mateData in data.crewmates)
        {
            if (mateData.id == crewmateID)
            {
                data.crewmates.Remove(mateData);
                return;
            }
        }
    }

    private void Awake()
    {
        Debug.Log("GameManager initialized");
        data = PlayerDataManager.CreateNewData();
        DontDestroyOnLoad(gameObject); // Required for persistance
#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
#endif
    }
    private void Update()
    {
        gameTimer += Time.deltaTime;

        switch (state)
        {
            case GameState.PLAYING:
                {
                    // Gameplay
                }
                break;
            case GameState.PAUSED:
                {
                    // Bypass gameplay
                }
                break;
        }
    }
}
