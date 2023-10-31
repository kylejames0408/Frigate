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

    public static bool MainMenuTesting = false;
    public static bool OutpostTesting = true;
    public static bool CombatTesting = false;

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

    private void Awake()
    {
        data = PlayerDataManager.CreateNewData();
        DontDestroyOnLoad(gameObject); // Required for persistance
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

    // Game to editor and other programs
    void OnApplicationFocus(bool hasFocus)
    {
        //Debug.Log("Focus: " + hasFocus);
    }
    // Game to other programs
    void OnApplicationPause(bool pauseStatus)
    {
        //Debug.Log("Pause: " + pauseStatus);
    }
}
