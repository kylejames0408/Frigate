using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuildingUI : MonoBehaviour
{
    // General
    public static BuildingUI instance;
    // Ghost Building
    private Button[] buttons;
    private Button[] devButtons;
    [SerializeField] private GameObject devMenu; // could move this into DevUI class/file
    public GameObject attackBtn;
    // Components
    private BuildingManager bm;

    private void Awake()
    {
        //canvasGroup = GetComponent<CanvasGroup>();
        bm = FindAnyObjectByType<BuildingManager>();
    }

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() => bm.SelectBuilding(index));
        }

        // Dev tools
        //devButtons = devMenu.GetComponentsInChildren<Button>();
        //devButtons[0].onClick.AddListener(() => BuildingManager.Instance.BuildAll());
        //attackBtn.GetComponent<Button>().onClick.AddListener(() => CeneManager.NextScene());
        attackBtn.SetActive(false);
    }

    private string GetButtonText(Building b)
    {
        string buildingName = b.buildingName;
        //int resourceAmount = b.resourceCost.Length;
        //string[] resourceNames = new string[] { "Wood", "Stone" };
        //string resourceString = string.Empty;
        //for (int j = 0; j < resourceAmount; j++)
        //{
        //    resourceString += "\n " + resourceNames[j] + " (" + b.resourceCost[j] + ")";
        //}

        //return "<size=23><b>" + buildingName + "</b></size>" + resourceString;
        return "<size=50><b>" + buildingName + "</b></size>";
    }
}
