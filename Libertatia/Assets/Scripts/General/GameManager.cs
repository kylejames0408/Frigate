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
    // Game Data
    private float gameTimer = 0.0f;
    private GameState state = GameState.PLAYING;
    public static PlayerData data;
    public static bool outpostVisited = false;

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

    public virtual void Awake()
    {
        GameObject obj = new GameObject();
        obj.name = typeof(GameManager).Name;
        data = PlayerDataManager.Load();
        DontDestroyOnLoad(obj);
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
