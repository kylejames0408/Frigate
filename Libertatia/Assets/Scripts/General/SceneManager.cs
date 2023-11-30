using UnityEngine;
using UnityEngine.SceneManagement;

public class CeneManager : MonoBehaviour
{
    // Gets usable build index by "clearing" it. Parses the index into our possible indexes.
    private static int GetBuildIndex(int buildIndex)
    {
        return buildIndex % SceneManager.sceneCountInBuildSettings;
    }
    // Gets a safe build index that is offset from current build index
    private static int GetRelativeBuildIndex(int buildIndexOffset = 0)
    {
        return GetBuildIndex(SceneManager.GetActiveScene().buildIndex + buildIndexOffset);
    }
    public static void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
    }
    public static void LoadSceneAndActivate(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
        SetSceneActive();
    }
    private static void SetSceneActive()
    {
        SceneManager.sceneLoaded += SceneLoadedCallback;
    }
    private static void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
    }
    public static void UnloadScene(int buildIndex)
    {
        SceneManager.UnloadSceneAsync(buildIndex, UnloadSceneOptions.None);
    }

    // Loads main menu scene if it does not already exists
    public static void LoadScene(string sceneName)
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        if (SceneManager.sceneCountInBuildSettings > 1)
        {
            UnloadScene(buildIndex);
        }
    }
    public static void LoadMainMenu()
    {
        GameManager.outpostVisitNumber = 0;
        GameManager.explorationVisitNumber = 0;
        GameManager.combatVisitNumber = 0;
        //GameManager.ResetPlayerData();

        if(GameManager.mainMenuTesting)
            SceneManager.LoadScene("MainMenu-Testing", LoadSceneMode.Single);
        else
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public static void LoadOutpostFromMainMenu()
    {
        if(GameManager.outpostTesting)
            SceneManager.LoadScene("Outpost-Testing", LoadSceneMode.Single);
        else
            SceneManager.LoadScene("Outpost", LoadSceneMode.Single);
        //SceneManager.UnloadSceneAsync("MainMenu");
        //SceneManager.sceneLoaded += OnLoadCallback;
    }
    public static void LoadExploration()
    {
        SceneManager.LoadScene("Exploration", LoadSceneMode.Single);
    }
    public static void LoadOutpost()
    {
        SceneManager.LoadScene("Outpost", LoadSceneMode.Single);
    }
    public static void LoadCombat()
    {
        SceneManager.LoadScene("Combat-Testing", LoadSceneMode.Single);
    }

    public static void LoadCombatFromOutpost()
    {
        if(GameManager.combatTesting)
            SceneManager.LoadScene("Combat-Testing", LoadSceneMode.Single);
        else
            SceneManager.LoadScene("Combat", LoadSceneMode.Single);
    }
    public static void LoadOutpostFromCombat()
    {
        if (GameManager.outpostTesting)
            SceneManager.LoadScene("Outpost-Testing", LoadSceneMode.Single);
        else
            SceneManager.LoadScene("Outpost", LoadSceneMode.Single);
    }
    public static void OnLoadCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
    }
    public static void Quit()
    {
        Application.Quit(0);
    }
}