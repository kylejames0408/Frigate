using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public struct BuildingResources
{
    public int wood;
}

public class BuildingManager : MonoBehaviour
{
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent; // happens to be the object it is on
    // Components
    private OutpostManagementUI omui;
    private ResourcesUI oui;
    private bool isPlacing = false;
    private Building activeBuilding;
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    public UnityEvent placedBuilding;
    // Tracking
    public List<Building> buildings; // maybe make a lookup table for buildings since they have an ID

    private void Start()
    {
        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }// init both of these
        if (oui == null) { oui = FindObjectOfType<ResourcesUI>(); }

        // Init Building (make own function)
        buildings = new List<Building>(); //GameManager.Data.buildings.Count
        for (int i = 0; i < GameManager.Data.buildings.Count; i ++) // cache buildings list?
        {
            Building building = Instantiate(buildingPrefabs[GameManager.Data.buildings[i].uiIndex], buildingParent);
            building.id = GameManager.Data.buildings[i].id;
            building.uiIndex = GameManager.Data.buildings[i].uiIndex;
            building.level = GameManager.Data.buildings[i].level;
            building.transform.position = GameManager.Data.buildings[i].position;
            building.transform.rotation = GameManager.Data.buildings[i].rotation;
            building.CompleteBuild(); // dont keep, but is used to complete for now
            buildings.Add(building);
        }

        // Fill UI - probably combine?
        omui.FillConstructionUI(this, buildingPrefabs);
        oui.Init();
    }
    private void Update()
    {
        HandlePlacing();
    }

    // selecting building UI
    internal void SelectBuilding(int index)
    {
        if (isPlacing)
        {
            Destroy(activeBuilding.gameObject);
        }
        isPlacing = true;

        Building prefab = buildingPrefabs[index];
        activeBuilding = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, buildingParent);
        activeBuilding.uiIndex = index; // sets type
    }
    // placing a building
    internal void SpawnBuilding(Building building, Vector3 position)
    {
        isPlacing = false;
        placedBuilding.Invoke();

        // Check if there are enough resources - possible move
        BuildingResources cost = building.Cost;
        if (GameManager.Data.resources.wood < cost.wood)
        {
            Destroy(building.gameObject);
            Debug.Log("Cannot build; Insufficient resources"); // UI
            return;
        }

        // Subtract resources - move to Building or bring CanBuild in here
        GameManager.data.resources.wood -= cost.wood;
        oui.UpdateWoodUI(GameManager.Data.resources.wood);

        // Create Building
        BuildingData data = new BuildingData();
        data.id = building.id;
        data.uiIndex = building.uiIndex;
        data.level = building.level;
        data.position = building.transform.position;
        data.rotation = building.transform.rotation;
        GameManager.AddBuilding(data);
        if (building.uiIndex == 0)
        {
            GameManager.data.resources.foodPerAP += 50;
            oui.UpdateFoodConsumptionUI(GameManager.Data.resources.foodPerAP);
        }
        else if (building.uiIndex == 1)
        {
            GameManager.data.outpostCrewCapacity += 8;
            oui.UpdateCrewCapacityUI(GameManager.Data.outpostCrewCapacity);
        }
        else if (building.uiIndex == 2)
        {
            // ?
        }
        buildings.Add(building);
        building.Place();

        onBuildingPlaced.Raise(this, building);
    }
    internal string UpgradeBuilding(int buildingID)
    {
        Building building = GetBuilding(buildingID);
        building.Upgrade();
        return building.GetStatus();
    }
    internal void DemolishBuilding(int buildingID)
    {
        Building building = GetBuilding(buildingID);
        GameManager.RemoveBuilding(buildingID);
        buildings.Remove(building);
        building.Demolish();
    }
    // Lookup table would be faster
    private Building GetBuilding(int buildingID)
    {
        foreach (Building building in buildings)
        {
            if(building.id == buildingID)
            {
                return building;
            }
        }
        return null;
    }

    // Handlers
    private void HandlePlacing()
    {
        // move functionallity outside of monobehavior
        if (isPlacing)
        {
            // Camera.main or Camera.current?
            Physics.Raycast(CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition),
                out RaycastHit info, 300, LayerMask.GetMask("Terrain"));

            activeBuilding.transform.position = info.point;
            //Graphics.DrawMesh(buildingMesh, position, buildingRotation, placingBuildingMat, 0);

            // check collision
            if (Input.GetMouseButtonDown(0) && !activeBuilding.IsColliding && !EventSystem.current.IsPointerOverGameObject())
            {
                SpawnBuilding(activeBuilding, info.point);
            }
        }
    }

    // Dev
    internal void BuildAll()
    {
        foreach (Building building in buildings)
        {
            building.CompleteBuild();
        }
    }

    // Update player data when scene is unloaded
    private void OnDestroy()
    {
        //realtimeData.resources = resources; // disable for now
        //GameManager.Instance.buildingAmount = buildings.Count;
        //GameManager.Instance.DataManager.Update(realtimeData);
    }


}
