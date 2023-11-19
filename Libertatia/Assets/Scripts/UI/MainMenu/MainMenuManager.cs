using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuInterface;
    [SerializeField] GameObject optionsInterface;
    [SerializeField] GameObject creditsInterface;

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
