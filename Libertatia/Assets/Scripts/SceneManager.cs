using UnityEngine;
using UnityEngine.SceneManagement;

public class CeneManager : MonoBehaviour
{
    private static Scene scene;
    private static LoadSceneParameters mode;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
        mode = new LoadSceneParameters(LoadSceneMode.Single); // Unloads current scene
    }

    public static void NextScene()
    {
        int buildIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(buildIndex, mode);
    }
    public static void PreviousScene()
    {
        int buildIndex = (scene.buildIndex - 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(buildIndex, mode);
    }

    public void Quit()
    {
        Application.Quit(0);
    }
}
