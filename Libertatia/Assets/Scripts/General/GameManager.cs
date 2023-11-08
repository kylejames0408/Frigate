using System;
using UnityEngine;

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
    public static bool outpostVisited = false;
    public static PlayerData data;

    // starting to act like the actual data manager - read/write
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

    private void Awake()
    {
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

    // NOTE: will be necessary for when loading is implemented
    private void OnDestroy()
    {
        data.gameTimer = gameTimer;
    }

    internal static void RemoveCrewmateData(int crewmateID)
    {
        foreach(CrewmateData mateData in data.crewmates)
        {
            if(mateData.id == crewmateID)
            {
                data.crewmates.Remove(mateData);
                return;
            }
        }
    }
}
