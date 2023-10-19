using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    private static BuildingUI instance;
    private float interfaceAnimSpeed = 0.6f;
    private float interfaceWidth;
    [SerializeField] private Sprite emptyAssignmentIcon; 
    // Building information objects
    [SerializeField] private Transform iconUI;
    [SerializeField] private Transform nameUI;
    [SerializeField] private Transform levelUI;
    [SerializeField] private Transform outputUI;
    //[SerializeField] private Transform assignmentUI; // will need this if more information is portrayed
    [SerializeField] private Transform assignmentIconUI;
    // Buttons
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Button demolishBtn;
    // Dynamic/tracking information
    private Building activeBuilding;

    public static BuildingUI Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        upgradeBtn.onClick.AddListener(UpgradeCallback);
        demolishBtn.onClick.AddListener(DemolishCallback);
        interfaceWidth = GetComponent<RectTransform>().rect.width;
    }

    private void UpgradeCallback()
    {
        activeBuilding.Upgrade();
    }

    private void DemolishCallback()
    {
        activeBuilding.Demolish();
    }

    private void AssignCallback()
    {
        // some information would need to be passed through
        //activeBuilding.AssignBuilder();
    }

    public void FillUI(Building building)
    {
        iconUI.GetComponent<Image>().sprite = building.Icon;
        nameUI.GetComponent<TextMeshProUGUI>().text = building.Name;
        levelUI.GetComponent<TextMeshProUGUI>().text = building.level.ToString(); // will also need to show status
        outputUI.GetComponent<TextMeshProUGUI>().text = building.output; // not sure what output is yet

        if(building.builder != null)
        {
            assignmentIconUI.GetComponent<Image>().sprite = building.builder.Icon;
        }
        else
        {
            assignmentIconUI.GetComponent<Image>().sprite = emptyAssignmentIcon;
        }

        activeBuilding = building;
    }

    // Minimizes menu
    public void OpenMenu()
    {
        transform.DOMoveX(0, interfaceAnimSpeed);
    }
    // Minimizes menu
    public void CloseMenu()
    {
        transform.DOMoveX(-interfaceWidth-10, interfaceAnimSpeed); // cant get height in start
    }
}
