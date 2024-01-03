using System;
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
    BUILDING,
    EXPLORATION,
    PLUNDERING,
    COUNT
}

public class GameManager : MonoBehaviour
{
    // Game Data
    [SerializeField] private GameState state = GameState.PLAYING;
    private static GamePhase phase = GamePhase.MAIN_MENU;
    [SerializeField] private float elapsedTime = 0.0f;
    private static bool isTutorial = true;
    // UI
    [SerializeField] private PauseMenu pauseMenu;

    // Tutorial
    public static int outpostVisitNumber = 0;
    public static int combatVisitNumber = 0;
    public static int explorationVisitNumber = 0;
    // - Events
    public UnityEvent onAttack;

    public static GamePhase Phase
    {
        get { return phase; }
    }
    public static bool IsTutorial
    {
        get { return isTutorial; }
    }

    private void Awake()
    {
        if(!PlayerDataManager.IsInitialized)
        {
            PlayerDataManager.NewGame();
        }
        else
        {
            Debug.Log("player data is already initialized - confirm");
            Destroy(gameObject); // if player data exists, this manager is a duplicate
            return;
        }

        if (pauseMenu == null) { pauseMenu = FindObjectOfType<PauseMenu>(); }
        DontDestroyOnLoad(gameObject); // Required for persistance

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
#endif
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;

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

    internal static void EndTutorial()
    {
        isTutorial = false;
    }
}
