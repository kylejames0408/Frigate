using UnityEngine;
using UnityEngine.SceneManagement;

public class CeneManager : MonoBehaviour
{
    // Gets usable build index by "clearning" it. Parses the index into our possible indexes.
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
    public static void UnloadCurrentScene()
    {
        UnloadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void NextScene()
    {
        LoadSceneAndActivate(GetRelativeBuildIndex(1));
    }
    public static void PreviousScene()
    {
        LoadSceneAndActivate(GetRelativeBuildIndex(-1));
    }
    public static void LoadScenes()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            SceneManager.LoadScene(i, LoadSceneMode.Additive);
        }
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
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

        if (SceneManager.sceneCountInBuildSettings > 1)
        {
            UnloadScene(buildIndex);
        }
    }
    public static void LoadOutpostFromMainMenu()
    {
        SceneManager.LoadScene("Outpost", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu");
    }
    public static void Quit()
    {
        Application.Quit(0);
    }
}