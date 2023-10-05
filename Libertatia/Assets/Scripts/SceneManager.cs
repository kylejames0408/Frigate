using UnityEngine.SceneManagement;

public class CeneManager
{

    /// <summary>
    /// Gets usable build index by "clearning" it. Parses the index into our possible indexes.
    /// </summary>
    /// <param name="buildIndex">An unsafe build index</param>
    /// <returns>A safe build index</returns>
    private int GetBuildIndex(int buildIndex)
    {
        return buildIndex % SceneManager.sceneCountInBuildSettings;
    }
    /// <summary>
    /// Gets a safe build index that is offset from current build index
    /// </summary>
    /// <param name="buildIndexOffset">Offset from current build index</param>
    /// <returns>New safe build index</returns>
    private int GetRelativeBuildIndex(int buildIndexOffset = 0)
    {
        return GetBuildIndex(SceneManager.GetActiveScene().buildIndex + buildIndexOffset);
    }
    /// <summary>
    /// Loads scene from build index. Does not unload other scenes
    /// </summary>
    /// <param name="buildIndex">Build index of the scene to load</param>
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
    }
    public void LoadSceneAndActivate(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);
        SetSceneActive();
    }
    private void SetSceneActive()
    {
        SceneManager.sceneLoaded += SceneLoadedCallback;
    }
    private void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
    }

    public void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(index, UnloadSceneOptions.None);
    }
    public void NextScene()
    {
        LoadSceneAndActivate(GetRelativeBuildIndex(1));
    }
    public void PreviousScene()
    {
        LoadSceneAndActivate(GetRelativeBuildIndex(-1));
    }

    public void LoadScenes()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            SceneManager.LoadScene(i, LoadSceneMode.Additive);
        }
    }
    // Specifics
    /// <summary>
    /// Loads main menu scene if it does not already exists
    /// </summary>
    public void LoadMainMenu()
    {
        Scene menu = SceneManager.GetSceneByName("MainMenu");
        if (menu == null)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }
}
