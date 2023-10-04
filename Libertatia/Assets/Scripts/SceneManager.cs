using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CeneManager
{

    /// <summary>
    /// Gets usable build index by "clearning" it. Parses the index into our possible indexes.
    /// </summary>
    /// <param name="buildIndex">An unsafe build index</param>
    /// <returns>A safe build index</returns>
    private static int GetBuildIndex(int buildIndex)
    {
        return buildIndex % SceneManager.sceneCountInBuildSettings;
    }
    /// <summary>
    /// Gets a safe build index that is offset from current build index
    /// </summary>
    /// <param name="buildIndexOffset">Offset from current build index</param>
    /// <returns>New safe build index</returns>
    private static int GetRelativeBuildIndex(int buildIndexOffset = 0)
    {
        return GetBuildIndex(SceneManager.GetActiveScene().buildIndex + buildIndexOffset);
    }
    /// <summary>
    /// Loads scene from build index. Does not unload other scenes
    /// </summary>
    /// <param name="buildIndex">Build index of the scene to load</param>
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

    public static void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(index, UnloadSceneOptions.None);
    }
    public static void NextScene()
    {
        LoadSceneAndActivate(GetRelativeBuildIndex(1));
    }
    public static void PreviousScene()
    {
        LoadSceneAndActivate(GetRelativeBuildIndex(-1));
    }
}
