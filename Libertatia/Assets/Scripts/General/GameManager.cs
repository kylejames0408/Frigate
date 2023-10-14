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
    MAIN_MENU,
    OUTPOST,
    ENEMY_TERRITORY
}

public class GameManager : MonoBehaviour
{
    // Class references
    public static GameManager instance;
    // Game Data
    private float gameTimer = 0.0f;
    private GameState state = GameState.PLAYING;
    public bool outpostVisited = false;

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

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // Game init
        PlayerDataManager.Init();

        // TODO: Might be good to have realtime variables per Unity forum

        // Scene
        //CeneManager.LoadMainMenu();
        // Load building man
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

    private void OnDestroy()
    {
        //pdm.UpdateTimer(gameTimer);
    }
}
