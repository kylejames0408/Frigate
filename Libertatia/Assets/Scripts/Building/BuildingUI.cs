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
        //devButtons[0].onClick.AddListener(() => BuildingManager.Instance.BuildAll(100));
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

    /// <summary>
    /// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current. This is a replacement
    /// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
    /// </summary>
    private bool IsPointerOverUIObject()
    {
        // https://stackoverflow.com/questions/52064801/unity-raycasts-going-through-ui
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = (Vector2)Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
