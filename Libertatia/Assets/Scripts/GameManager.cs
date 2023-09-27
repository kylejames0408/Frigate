using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private GameState state;
    private Resources resources;
    private float gameTimer = 0.0f;
    public float buildInterval = 3.0f;

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

                }
                break;
            case GameState.PAUSE:
                {

                }
                break;
        }
    }
}
