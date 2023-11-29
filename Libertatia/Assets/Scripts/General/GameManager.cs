using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    PLAYING,
    PAUSED,
    COUNT
}
public enum GamePhase
{
    MAIN_MENU,
    OUTPOST,
    EXPLORATION,
    ENEMY_TERRITORY,
    COUNT
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
    public static bool CombatTesting = true;

    // UI
    [SerializeField] private PauseMenu pauseMenu;

    // Events
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
    internal static void AddBuilding(BuildingData data)
    {
        Data.buildings.Add(data); // why is this.data not working???
    }
    internal static void RemoveBuilding(int buildingID)
    {
        foreach (BuildingData data in Data.buildings)
        {
            if (data.id == buildingID)
            {
                Data.buildings.Remove(data);
                return;
            }
        }
    }
    internal static void AddCrewmate(Crewmate mate)
    {
        Data.crewmates.Add(new CrewmateData(mate));
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
    // Update from Outpost
    internal static void UpdateCrewmateData(Crewmate[] crewmates)
    {
        data.crewmates.Clear();
        for (int i = 0; i < crewmates.Length; i++)
        {
            Data.crewmates.Add(new CrewmateData(crewmates[i]));
        }
    }
    internal static void UpdateCrewmateData(CrewmateData[] crewmates)
    {
        data.crewmates.Clear();
        for (int i = 0; i < crewmates.Length; i++)
        {
            Data.crewmates.Add(crewmates[i]);
        }
    }
    internal static void SeparateCrew()
    {
        data.outpostCrew.Clear();
        for (int i = 0; i < data.crewmates.Count; i++)
        {
            if (data.crewmates[i].building.id == -1)
            {
                data.combatCrew.Add(data.crewmates[i]);
            }
            else
            {
                data.outpostCrew.Add(data.crewmates[i]);
            }
        }
    }
    // Update from Combat
    internal static void UpdateCombatCrew(Crewmate[] crewmates)
    {
        data.combatCrew.Clear();
        for (int i = 0; i < crewmates.Length; i++)
        {
            data.combatCrew.Add(new CrewmateData(crewmates[i]));
        }
    }
    internal static void UpdateCombatCrew(CrewmateData[] crewmates)
    {
        data.combatCrew.Clear();
        for (int i = 0; i < crewmates.Length; i++)
        {
            data.combatCrew.Add(crewmates[i]);
        }
    }
    internal static void UpdateCrewmateData()
    {
        for (int i = 0; i < data.combatCrew.Count; i++)
        {
            data.outpostCrew.Add(data.combatCrew[i]);
        }
        data.combatCrew.Clear();
    }
    internal static void UpdateBuildingData(Building[] buildings)
    {
        data.buildings.Clear();
        for (int i = 0; i < buildings.Length; i++)
        {
            data.buildings.Add(new BuildingData(buildings[i]));
        }
    }

    private void Awake()
    {
        //Debug.Log("GameManager initialized");
        if(data.Equals(default(PlayerData))) // Prevents reinit when debugging
        {
            data = PlayerDataManager.CreateNewData();
            DontDestroyOnLoad(gameObject); // Required for persistance
        }
        else
        {
            Destroy(gameObject);
        }

        if(pauseMenu == null) { pauseMenu = FindObjectOfType<PauseMenu>(); }

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
#endif

    }
    private void Update()
    {
        gameTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GameState.PLAYING)
            {
                Pause();
            }
            else if (state == GameState.PAUSED)
            {
                Unpause();
            }
        }
    }
    private void OnDestroy()
    {
        data.gameTimer = gameTimer;
    }

    private void Pause()
    {
        state = GameState.PAUSED;
        pauseMenu.Open();
    }
    private void Unpause()
    {
        state = GameState.PLAYING;
        pauseMenu.Close();
    }
}
