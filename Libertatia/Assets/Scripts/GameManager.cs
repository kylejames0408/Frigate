using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
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

public class GameManager : MonoBehaviour
{
    // Constants
    private const int STARTING_CREW_SIZE = 3;
    // Class references
    public static GameManager instance;
    [SerializeField] private CeneManager sm;
    // Player Data Storage
    [SerializeField] private PlayerDataManager pdm; // look into making this public to the inspector // also is linked through inspector
    private float gameTimer = 0.0f;
    private GameState state = GameState.PLAYING;

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
    public PlayerDataManager DataManager
    {
        get { return pdm; }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // Game init
        pdm = new PlayerDataManager(STARTING_CREW_SIZE);

        // TODO: Might be good to have realtime variables per Unity forum

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

    public void Quit()
    {
        Application.Quit(0);
    }
    private void OnDestroy()
    {
        //pdm.UpdateTimer(gameTimer);
    }
}
