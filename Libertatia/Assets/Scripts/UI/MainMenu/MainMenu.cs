using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuInterface;
    [SerializeField] GameObject optionsInterface;
    [SerializeField] GameObject creditsInterface;

    [SerializeField] private Button btnFan;
    [SerializeField] private Button btnJames;
    [SerializeField] private Button btnJaradat;
    [SerializeField] private Button btnWeng;
    [SerializeField] private Button btnZaffram;

    public void OpenPortfolio(string url)
    {
        Application.OpenURL(url);
    }

    public void ShowMainMenu()
    {
        mainMenuInterface.SetActive(true);
        optionsInterface.SetActive(false);
        creditsInterface.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenuInterface.SetActive(false);
        optionsInterface.SetActive(true);
        creditsInterface.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuInterface.SetActive(false);
        optionsInterface.SetActive(false);
        creditsInterface.SetActive(true);
    }
}
