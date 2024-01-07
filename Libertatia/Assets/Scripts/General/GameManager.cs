using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    private static GameManager instance;
    // Game Data
    private float elapsedTime = 0.0f;
    [SerializeField] private GameState state = GameState.PLAYING;
    [SerializeField] private GamePhase initPhase = GamePhase.MAIN_MENU;
    private static GamePhase phase;
    // Components
    [SerializeField] private InterfaceManager interfaceManager; // will need different method for obtaining

    public static GamePhase Phase
    {
        get { return phase; }
    }

    private void Awake()
    {
        if(instance == null) { instance = this;}
        if (interfaceManager == null) { interfaceManager = FindObjectOfType<InterfaceManager>(); }

        if (!PlayerDataManager.IsInitialized)
        {
            PlayerDataManager.NewGame();
            phase = initPhase;
        }
        else
        {
            Debug.Log("Deleting duplicate global Manager");
            Destroy(gameObject);
            return;
        }

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
    private void OnDestroy()
    {
        //PlayerDataManager.SaveTutorialProgress(isTutorial);
        //PlayerDataManager.SaveTimestamp(elapsedTime);
    }

    // Gameplay actions
    private void Pause()
    {
        interfaceManager.PauseGame();
        state = GameState.PAUSED;
    }
    private void Unpause()
    {
        interfaceManager.UnpauseGame();
        state = GameState.PLAYING;
    }
    internal static void Quit()
    {
        CeneManager.Quit();
    }
    // Scene shifting
    internal static void ToMainMenu()
    {
        CeneManager.LoadMainMenu();
        phase = GamePhase.MAIN_MENU;
    }
    internal static void MainMenuToBuildingPhase()
    {
        CeneManager.LoadOutpost();
        phase = GamePhase.BUILDING;
    }
    internal static void ToBuildingPhase()
    {
        CeneManager.LoadOutpost();
        TutorialManager.LoadOutpost();
        phase = GamePhase.BUILDING;
    }
    internal static void ToExplorationPhase()
    {
        CeneManager.LoadExploration();
        phase = GamePhase.EXPLORATION;
    }
    // Dev Actions
    internal static void EndTutorial()
    {
        instance.interfaceManager.EndTutorial();
        TutorialManager.EndTutorial();
    }
}
