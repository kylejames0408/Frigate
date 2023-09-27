using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuildingUI : MonoBehaviour
{
    // tutorial
    private const int MAX_BUILDINGS = 2;
    private int buildingAmount = 0;
    // General
    public static BuildingUI instance;
    private bool isPlacing = false;
    private int buildingIndex = 0;
    // Ghost Building
    private Building placingBuilding;
    private Mesh buildingMesh;
    private Quaternion buildingRotation;
    // Ghost Building Attributes
    [SerializeField] private Material placingBuildingMat;
    // UI
    //private CanvasGroup canvasGroup;
    //public Transform resourceGroup;
    private Button[] buttons;
    private Button[] devButtons;
    [SerializeField] private GameObject devMenu; // could move this into DevUI class/file

    public static BuildingUI Instance
    {
        get
        {
            return instance;
        }
    }
    public bool IsPlacing
    {
        get
        {
            return isPlacing;
        }
    }

    private void Awake()
    {
        instance = this;
        //canvasGroup = GetComponent<CanvasGroup>();
    }
    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() => SelectBuilding(index));

            Building b = BuildingManager.Instance.GetBuildingPrefab(index);
            buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = GetButtonText(b);
        }
        buildingRotation = Quaternion.identity;

        // Dev tools
        devButtons = devMenu.GetComponentsInChildren<Button>();
        devButtons[0].onClick.AddListener(() => BuildingManager.Instance.BuildAll(100));
    }

    private void Update()
    {
        if (isPlacing)
        {
            Vector3 position = Vector3.zero;
            if (Physics.Raycast(
                CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition), // Camera.main?
                out RaycastHit info, 300, LayerMask.GetMask("Terrain")))
            {
                position = info.point;
            }


            placingBuilding.transform.position = position;

            // Different method
            //if (buildingAmount >= MAX_BUILDINGS)
            //{
            //    isPlacing = false;
            //    return;
            //}
            //Graphics.DrawMesh(buildingMesh, position, buildingRotation, placingBuildingMat, 0);
            //Vector3 position = CameraManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition); // this clips objects

            // check collision
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject() && !placingBuilding.IsColliding)
            {
                Destroy(placingBuilding.gameObject);
                isPlacing = false;
                buildingAmount++;
                BuildingManager.Instance.SpawnBuilding(buildingIndex, position);
                //canvasGroup.alpha = 1;
            }
        }
    }

    private void SelectBuilding(int index)
    {
        if (buildingAmount >= MAX_BUILDINGS)
        {
            return;
        }
        else if (placingBuilding)
        {
            Destroy(placingBuilding.gameObject);
        }
        buildingIndex = index;
        //ActorManager.instance.DeselectActors();
        //canvasGroup.alpha = 0;
        isPlacing = true;
        Building prefab = BuildingManager.Instance.GetBuildingPrefab(index);
        buildingMesh = prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        buildingRotation = prefab.transform.rotation;
        placingBuilding = Instantiate(BuildingManager.Instance.GetBuildingPrefab(buildingIndex), Vector3.zero, buildingRotation);
        placingBuilding.Placing();
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

    //public void RefreshResources()
    //{
    //    for (int i = 0; i < resourceGroup.childCount; i++)
    //        resourceGroup.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = BuildingManager.Instance.currentResources[i].ToString();
    //}
}
