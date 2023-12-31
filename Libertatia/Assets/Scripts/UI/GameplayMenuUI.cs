using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayMenuUI : MonoBehaviour
{
    public GameObject gameplayMenu;
    public Button resumeBtn;
    public Button optionsBtn;
    public Button mainMenuBtn;

    private void Start()
    {
        resumeBtn.onClick.AddListener(() => { gameplayMenu.SetActive(false); });
        mainMenuBtn.onClick.AddListener(() => { GameManager.outpostVisitNumber = 0; GameManager.combatVisitNumber = 0; CeneManager.LoadMainMenu(); });
        gameplayMenu.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            gameplayMenu.SetActive(true);
        }
    }

    public void OpenMenu()
    {
        gameplayMenu.SetActive(true);
    }
}
