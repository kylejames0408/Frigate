using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;
// Notes
// Could probably have a refresh rate dropdown if there are different options
// Might make sense to set the slider max and min, but I hard coded it to -80 to 20 based on mixer
// Could probably save these settings with the rest of the data

public class MainMenuManager : MonoBehaviour
{
    private enum MainMenuInterfaces
    {
        MAIN,
        SETTINGS,
        CREDITS
    }
    [Header("Development")]
    [SerializeField] private MainMenuInterfaces interfaces = MainMenuInterfaces.MAIN;
    [Header("Interfaces")]
    [SerializeField] private GameObject mainInterface;
    [SerializeField] private GameObject settingsInterface;
    [SerializeField] private GameObject creditsInterface;
    [Header("Main")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button showSettingButton;
    [SerializeField] private Button showCreditsButton;
    [SerializeField] private Button quitButton;
    [Header("Settings")]
    [SerializeField] private Button settingsToMainButton;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    // Affected
    [SerializeField] private AudioMixer masterMixer;
    [Header("Credits")]
    [SerializeField] private Button creditsToMainButton;
    //[SerializeField] private Button fanPortfolioButton;
    //[SerializeField] private Button jamesPortfolioButton;
    //[SerializeField] private Button jaradatPortfolioButton;
    //[SerializeField] private Button wengPortfolioButton;
    //[SerializeField] private Button zafframPortfolioButton;


    private void Awake()
    {
        InitMainInterface();
        InitSettingsInterface();
        InitCreditsInterface();
        ShowMainMenu();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        switch (interfaces)
        {
            case MainMenuInterfaces.MAIN:
                ShowMainMenu();
                break;
            case MainMenuInterfaces.SETTINGS:
                ShowSettings();
                break;
            case MainMenuInterfaces.CREDITS:
                ShowCredits();
                break;
        }
    }
#endif
    // Main Interface Initializations
    private void InitMainInterface()
    {
        playButton.onClick.AddListener(Play);
        showSettingButton.onClick.AddListener(ShowSettings);
        showCreditsButton.onClick.AddListener(ShowCredits);
        quitButton.onClick.AddListener(Quit);
    }
    // Settings Interface Initializations
    private void InitSettingsInterface()
    {
        settingsToMainButton.onClick.AddListener(ShowMainMenu);
        volumeSlider.onValueChanged.AddListener(ChangeVolumeCallback);
        resolutionDropdown.onValueChanged.AddListener(ChangeResolutionCallback);
        qualityDropdown.onValueChanged.AddListener(ChangeQualityCallback);
        fullscreenToggle.onValueChanged.AddListener(ChangeScreenModeCallback);

        InitVolumeSettings();
        InitResolutionSettings();
        InitQualitySettings();
    }
    private void InitVolumeSettings()
    {
        volumeSlider.maxValue = 20f;
        volumeSlider.minValue = -80f;
        volumeSlider.value = 0f;
    }
    private void InitResolutionSettings()
    {
        Resolution[] resolutions = Screen.resolutions;
        int currentResolutionIndex = -1;
        Resolution currentResolution = Screen.currentResolution;
        List<TMP_Dropdown.OptionData> resolutionOptions = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].height == currentResolution.height &&
                resolutions[i].width == currentResolution.width)
            {
                currentResolutionIndex = i;
            }

            //string text = resolutions[i].ToString();
            string text = resolutions[i].width.ToString() + "x" + resolutions[i].height;
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(text);
            resolutionOptions.Add(optionData);
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    private void InitQualitySettings()
    {
        List<TMP_Dropdown.OptionData> qualityOptions = new List<TMP_Dropdown.OptionData>();
        string[] qualityLevelNames = QualitySettings.names;
        for (int i = 0; i < QualitySettings.count; i++)
        {
            string text = qualityLevelNames[i];
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(text);
            qualityOptions.Add(optionData);
        }
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }
    // Credit Interface Initializations
    private void InitCreditsInterface()
    {
        creditsToMainButton.onClick.AddListener(ShowMainMenu);
    }

    // Core Actions
    private void Play()
    {
        CeneManager.LoadOutpost();
    }
    private void Quit()
    {
        CeneManager.Quit();
    }
    // Menu Actions
    private void ShowMainMenu()
    {
        mainInterface.SetActive(true);
        settingsInterface.SetActive(false);
        creditsInterface.SetActive(false);
        interfaces = MainMenuInterfaces.MAIN;
    }
    private void ShowSettings()
    {
        mainInterface.SetActive(false);
        settingsInterface.SetActive(true);
        creditsInterface.SetActive(false);
        interfaces = MainMenuInterfaces.SETTINGS;
    }
    private void ShowCredits()
    {
        mainInterface.SetActive(false);
        settingsInterface.SetActive(false);
        creditsInterface.SetActive(true);
        interfaces = MainMenuInterfaces.CREDITS;
    }
    // Callbacks
    private void ChangeVolumeCallback(float volumePercentage)
    {
        masterMixer.SetFloat("volume", volumePercentage);
    }
    private void ChangeQualityCallback(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    private void ChangeScreenModeCallback(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    private void ChangeResolutionCallback(int resolutionIndex)
    {
        Resolution newResolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
    }
    public void OpenPortfolioCallback(string url)
    {
        Application.OpenURL(url);
    }
}
