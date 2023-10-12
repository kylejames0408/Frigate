using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[Serializable]
public struct BuildingResources
{
    public int wood;
    public int dubloons;
}

public class BuildingManager : MonoBehaviour
{
    // Game/Player Data
    private List<Building> buildings;
    private PlayerResources resources;
    private PlayerData realtimeData;
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent; // happens to be the object it is on
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    // Components
    private BuildingUI buildingUI;
    private OutpostUI outpostUI;
    private bool isPlacing = false;
    private Building activeBuilding;

    private void Start()
    {
        // should check building folder for buildings
        realtimeData = GameManager.Instance.DataManager.Data;
        resources = realtimeData.resources;

        if (buildings == null) // GameManager would fill buildings if any, unless we just stick to the scene
        {
            buildings = new List<Building>();
        }
        else if (buildings.Count > 0)
        {
            foreach (Building b in buildings)
            {
                buildings.Add(b);
                b.CompleteBuild();
            }
        }
        buildingUI = FindObjectOfType<BuildingUI>(); // init both of these
        outpostUI = FindObjectOfType<OutpostUI>();
        outpostUI.Init(realtimeData.crewmateAmount,
            5,  // crew capacity
            resources.food,
            5,  // AP consumption of food
            resources.doubloons,
            resources.wood);
    }
    private void Update()
    {
        if (isPlacing)
        {
            // Camera.main or Camera.current?
            Physics.Raycast(CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition),
                out RaycastHit info, 300, LayerMask.GetMask("Terrain"));

            activeBuilding.transform.position = info.point;
            //Graphics.DrawMesh(buildingMesh, position, buildingRotation, placingBuildingMat, 0);

            // check collision
            if (Input.GetMouseButtonDown(0) &&  !activeBuilding.IsColliding) //!IsPointerOverUIObject()
            {
                SpawnBuilding(activeBuilding, info.point);
            }
        }
    }

    // selecting building UI
    public void SelectBuilding(int index)
    {
        if (isPlacing)
        {
            Destroy(activeBuilding.gameObject);
        }
        isPlacing = true;

        Building prefab = buildingPrefabs[index];
        activeBuilding = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, buildingParent);
    }
    // placing a building
    public void SpawnBuilding(Building building, Vector3 position)
    {
        isPlacing = false;

        // Check if there are enough resources - possible move
        BuildingResources cost = building.Cost;
        if (resources.wood < cost.wood || resources.doubloons < cost.dubloons)
        {
            Destroy(building.gameObject);
            Debug.Log("Cannot build; Insufficient resources"); // UI
            return;
        }

        // Subtract resources - move to Building or bring CanBuild in here
        resources.wood -= cost.wood;
        resources.doubloons -= cost.dubloons;
        //resources.Print();

        outpostUI.UpdateWoodUI(resources.wood);
        outpostUI.UpdateDubloonUI(resources.doubloons);

        // Create Building
        building.Place();
        buildings.Add(building);
    }

    // Dev functions
    public void BuildAll()
    {
        foreach (Building building in buildings)
        {
            building.CompleteBuild();
        }
    }

    // Checks if mouse is over any UI - might not need with canvasgroup
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

    // Update player data when scene is unloaded
    private void OnDestroy()
    {
        //realtimeData.resources = resources; // disable for now
        realtimeData.buildingAmount = buildings.Count;
        //GameManager.Instance.DataManager.Update(realtimeData);
    }
}
