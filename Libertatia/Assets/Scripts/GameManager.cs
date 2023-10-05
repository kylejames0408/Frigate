using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum GameMode
{
    TUTORIAL,
    REGULAR
}
public enum GameState
{
    PLAYING,
    PAUSED
}
public enum GamePhase
{
    OUTPOST,
    ENEMY_TERRITORY
}

public struct Resources
{
    public int wood;
    public int food;
    public int gold;
    public int loyalty;
}
public class PlayerData : ScriptableObject
{
    public GameState gameState;
    public GamePhase gamePhase;
    public GameMode gameMode;
    public float gameTimer;
    public Resources resources;
}

public class GameManager : MonoBehaviour
{
    // Class references
    public static GameManager instance;
    [SerializeField] private CeneManager sm;
    [SerializeField] private BuildingManager bm;
    // Game & Player Data
    [SerializeField] private GameState state = GameState.PLAYING;
    [SerializeField] private GamePhase phase = GamePhase.OUTPOST;
    [SerializeField] private GameMode mode = GameMode.TUTORIAL;
    [SerializeField] private float gameTimer = 0.0f;
    [SerializeField] private Resources resources;
    // Player Data Storage
    [SerializeField] private PlayerData data; // look into making this public to the inspector

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name;
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    public Resources Resources
    {
        get { return resources; }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // Game init
        data = new PlayerData(); // would use this data if loading a game
        state = GameState.PLAYING;
        phase = GamePhase.OUTPOST;
        mode = GameMode.TUTORIAL;
        gameTimer = 0.0f;
        // Scene
        sm = new CeneManager();
        //sm.LoadMainMenu();
        // Load building man
    }

    private void Update()
    {
        // Update game time
        gameTimer += Time.deltaTime;
        gameTimer %= 60;

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

    public void SavePlayerData(Resources resources)
    {
        this.resources = resources;
    }

    public void Quit()
    {
        Application.Quit(0);
    }
}
