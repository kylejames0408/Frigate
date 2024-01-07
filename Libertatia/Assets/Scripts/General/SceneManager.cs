using UnityEngine;
using UnityEngine.SceneManagement;

public class CeneManager : MonoBehaviour
{
    private static CeneManager instance;
    [SerializeField] private bool UseMainMenuTestingScene = false;
    [SerializeField] private bool UseOutpostTestingScene = false;
    [SerializeField] private bool UseExplorationTestingScene = false;
    [SerializeField] private bool UseCombatTestingScene = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static void LoadMainMenu()
    {
        if(instance.UseMainMenuTestingScene)
        {
            SceneManager.LoadScene("MainMenu-Testing", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
    public static void LoadExploration()
    {
        if(instance.UseExplorationTestingScene)
        {
            SceneManager.LoadScene("Exploration-Testing", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Exploration", LoadSceneMode.Single);
        }
    }
    public static void LoadOutpost()
    {
        if (instance.UseOutpostTestingScene)
        {
            SceneManager.LoadScene("Outpost-Testing", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Outpost", LoadSceneMode.Single);
        }
    }
    public static void LoadCombat()
    {
        if (instance.UseCombatTestingScene)
        {
            SceneManager.LoadScene("Combat-Testing", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Combat", LoadSceneMode.Single);
        }
    }
    public static void Quit()
    {
        Application.Quit(0);
    }
}