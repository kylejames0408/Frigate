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
    private float elapsedTime = 0.0f;
    [SerializeField] private GameState state = GameState.PLAYING;
    [SerializeField] private GamePhase initPhase = GamePhase.MAIN_MENU;
    [SerializeField] private bool initTutorial = true;
    private static GamePhase phase;
    private static bool isTutorial;
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
            phase = initPhase;
            isTutorial = initTutorial;
        }
        else
        {
            Debug.Log("Deleting duplicate global Manager");
            Destroy(gameObject);
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
    private void OnDestroy()
    {
        //PlayerDataManager.SaveTutorialProgress(isTutorial);
        //PlayerDataManager.SaveTimestamp(elapsedTime);
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
