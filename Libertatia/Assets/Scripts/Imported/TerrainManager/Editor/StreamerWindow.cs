using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class StreamerWindow : EditorWindow
{
    private const int Step = 2; // шаг слайдера (1 bad value)

    private Rect headerSize; // размер шапки
    private Rect bodySize;   // размер тела
    private Rect footerSize; // размер футера
    private Rect sliderSize; // размеры для слайдера

    private Texture2D headerColor; // цвет фона шапки

    private Terrain[] terrains;       // список террейнов
    private string[] nameOfTerrains;      // имена террейнов
    private int selectedTerrain = 0;          // индекс выбранного террейна
    private int lastSelectedTerrain = -1; // индекс послденего террейна
    private int pow = 1;                  // шаг изменение значения слайдера
    private int divider = Step;           // деление террейна
    private int countDivisors;

    //private bool isRemove = false; // удалить исходный terrain

    private TerrainManager tManager;
    private ChunkData chunkData;

    [MenuItem("Window/Split Terrain")]
    private static void OpenWindow()
    {
        StreamerWindow sWindow = (StreamerWindow)GetWindow(typeof(StreamerWindow));
        sWindow.minSize = new Vector2(400, 400);
        sWindow.maxSize = new Vector2(400, 400);
        sWindow.titleContent = new GUIContent("Split");
        sWindow.Show();
    }
    /// <summary>
    /// Аналог Start() или Awake()
    /// </summary>
    private void OnEnable()
    {
        headerColor = new Texture2D(1, 1);
        headerColor.SetPixel(0, 0, Color.gray);
        headerColor.Apply();

        FindTerrains();
        tManager = new TerrainManager();

        tManager.Progress += TManager_Progress;
        tManager.Complete += TManager_Complete;
    }

    private void OnDestroy()
    {
        tManager.Progress -= TManager_Progress;
        tManager.Progress -= TManager_Complete;
    }

    private void TManager_Progress(object sender, ProgressEventArgs e)
    {
        EditorUtility.DisplayProgressBar(e.Caption, e.Content, e.Progress);
    }

    private void TManager_Complete(object sender, EventArgs e)
    {
        EditorUtility.ClearProgressBar();
    }
    /// <summary>
    /// Аналог Update()
    /// </summary>
    private void OnGUI()
    {
        if (terrains.Length < 1) {
            Error("Terrains not found!");
            return;
        }
        /* Количество степений последнего выбранного террейна */
        if (selectedTerrain != lastSelectedTerrain) {
            chunkData = new ChunkData(
                terrains[selectedTerrain].terrainData.size,
                terrains[selectedTerrain].terrainData.heightmapResolution,
                terrains[selectedTerrain].terrainData.detailResolution,
                terrains[selectedTerrain].terrainData.detailResolutionPerPatch,
                terrains[selectedTerrain].terrainData.alphamapResolution,
                terrains[selectedTerrain].terrainData.baseMapResolution,
                "Assets/Chunks"
            );

            countDivisors = NumberOfDivisors(Mathf.Min(chunkData.Width, chunkData.Length), Step);
            lastSelectedTerrain = selectedTerrain;
        }

        Layouts();
        Header();
        Body();
        Footer();
    }
    /// <summary>
    /// Размеры Rect
    /// </summary>
    private void Layouts()
    {
        headerSize.x = 0;
        headerSize.y = 0;
        headerSize.width = position.width;
        headerSize.height = 100;

        sliderSize.x = 0;
        sliderSize.y = 100;
        sliderSize.width = position.width + 50;
        sliderSize.height = 20;

        bodySize.x = 0;
        bodySize.y = 120;
        bodySize.width = position.width;
        bodySize.height = 300;

        footerSize.x = 0;
        footerSize.y = 375;
        footerSize.width = position.width;
        footerSize.height = 50;

        GUI.DrawTexture(headerSize, headerColor);
    }
    /// <summary>
    /// Компоненты отрисовываемые в шапке
    /// </summary>
    private void Header()
    {
        GUILayout.BeginArea(headerSize);

        selectedTerrain = GUILayout.SelectionGrid(selectedTerrain, nameOfTerrains, 1);

        GUILayout.EndArea();
    }
    /// <summary>
    /// Компоненты отрисовываемые в теле
    /// </summary>
    private void Body()
    {
        GUILayout.BeginArea(sliderSize);

        pow = EditorGUILayout.IntSlider("Scale", pow, countDivisors, 1);
        divider = Step;
        divider = (int)Math.Pow(divider, pow);
        GUILayout.EndArea();

        GUILayout.BeginArea(bodySize);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("   Chunk count:");
        GUILayout.Label($"{divider} x {divider}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        chunkData.Width  = terrains[selectedTerrain].terrainData.size.x / divider;
        chunkData.Length = terrains[selectedTerrain].terrainData.size.z / divider;
        GUILayout.Label("   Chunk size:");
        GUILayout.Label($"{chunkData.Width} x {chunkData.Length} x {chunkData.Height}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Resolution");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        chunkData.HeigthmapResolution = terrains[selectedTerrain].terrainData.heightmapResolution / divider;
        GUILayout.Label("   Heigthmap:");
        GUILayout.Label($"{chunkData.HeigthmapResolution}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        chunkData.DetailResolutioinPerPatch = terrains[selectedTerrain].terrainData.detailResolutionPerPatch / divider;
        GUILayout.Label("   Detail Per Patch:");
        GUILayout.Label($"{chunkData.DetailResolutioinPerPatch}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        chunkData.DetailResolution = terrains[selectedTerrain].terrainData.detailResolution / divider;
        GUILayout.Label("   Detail:");
        GUILayout.Label($"{chunkData.DetailResolution}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        chunkData.ControlTextureResolution = terrains[selectedTerrain].terrainData.alphamapResolution / divider;
        GUILayout.Label("   Control Texture:");
        GUILayout.Label($"{chunkData.ControlTextureResolution}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        chunkData.BaseTextureResolution = terrains[selectedTerrain].terrainData.baseMapResolution / divider;
        GUILayout.Label("   Base Texture:");
        GUILayout.Label($"{chunkData.BaseTextureResolution}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Other");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("   Path:");
        chunkData.Path = GUILayout.TextField(chunkData.Path, 128, GUILayout.MaxWidth(200));
        EditorGUILayout.EndHorizontal();

        /*EditorGUILayout.BeginHorizontal();
        GUILayout.Label("   Remove source terrain:");
        isRemove = GUILayout.Toggle(isRemove, "");
        EditorGUILayout.EndHorizontal();*/

        GUILayout.EndArea();
    }
    /// <summary>
    /// Компоненты отрисовываемые внизу
    /// </summary>
    private void Footer()
    {
        GUILayout.BeginArea(footerSize);

        if (GUILayout.Button("Split", GUILayout.Height(25)))
        {
            if (!Directory.Exists(chunkData.Path))
                Directory.CreateDirectory(chunkData.Path);

            tManager.Split(terrains[selectedTerrain], chunkData, divider);
        }

        GUILayout.EndArea();
    }
    /// <summary>
    /// Вывод ошибки
    /// </summary>
    /// <param name="message">Сообщение</param>
    private void Error(string message)
    {
        GUILayout.BeginArea(new Rect(0, 0, position.width / 2, position.height / 2));

        GUILayout.Label(message);

        GUILayout.EndArea();
    }
    /// <summary>
    /// Поиск terrain-ов на сцене
    /// </summary>
    private void FindTerrains()
    {
        var handleTerrains = FindObjectsOfType(typeof(Terrain));
        terrains = new Terrain[handleTerrains.Length];
        nameOfTerrains = new string[handleTerrains.Length];

        for (int i = 0; i < handleTerrains.Length; i++)
        {
            terrains[i] = handleTerrains[i] as Terrain;
            nameOfTerrains[i] = handleTerrains[i].name;
        }
    }
    /// <summary>
    /// Возвращает количество делителей числа
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="step">Шаг</param>
    /// <returns>Количество</returns>
    private int NumberOfDivisors(float value, int step)
    {
        if (step == 1)
            return 0;

        var count = 0;

        while (value % step == 0) {
            value /= step;
            count++;
        }

        return count;
    }
}
