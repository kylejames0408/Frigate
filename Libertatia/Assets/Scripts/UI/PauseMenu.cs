using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button mainMenuBtn;

    private void Start()
    {
        // Set Callbacks
        resumeBtn.onClick.AddListener(Close);
        mainMenuBtn.onClick.AddListener(ToMainMenu);

        Close();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void ToMainMenu()
    {
        GameManager.outpostVisitNumber = 0;
        GameManager.combatVisitNumber = 0;
        CeneManager.LoadMainMenu();
    }
}
