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
    public void UnloadScene(int buildIndex)
    {
        SceneManager.UnloadSceneAsync(buildIndex, UnloadSceneOptions.None);
    }
    public void UnloadCurrentScene()
    {
        UnloadScene(SceneManager.GetActiveScene().buildIndex);
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
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        UnloadScene(buildIndex);
    }
}
