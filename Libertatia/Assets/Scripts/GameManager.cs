using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum GameState
{
    PLAY,
    PAUSE
}

public struct Resources
{
    public int wood;
    public int stone;

    public Resources(int wood, int stone) : this()
    {
        this.wood = wood;
        this.stone = stone;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // Game Data
    private GameState state;
    private float gameTimer = 0.0f;
    // Player Data // THOUGHT: player data can just be stored in a scriptable object and the GM interacts with it
    private Resources resources;
    private int buildingAmount = 0;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name;
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    private List<Building> buildings;

    public List<Building> Buildings
    {
        get { return buildings; }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        buildings = new List<Building>();
    }

    private void Start()
    {
        gameTimer = 0.0f;
        resources = new Resources(0,0);
    }

    private void Update()
    {
        // Update game time
        gameTimer += Time.deltaTime;
        gameTimer %= 60;

        switch (state)
        {
            case GameState.PLAY:
                {
                    // Gameplay
                }
                break;
            case GameState.PAUSE:
                {
                    // Bypass gameplay
                }
                break;
        }
    }

}
