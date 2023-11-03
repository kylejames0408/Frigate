using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Might need to separate into cost and production
[Serializable]
public struct BuildingResources
{
    public int wood;
    public int food;
    public int ap;

    public override string ToString()
    {
        if(wood > 0)
        {
            return wood + " wood";
        }
        else if(food > 0)
        {
            return food + " wood";
        }
        else
        {
            return "None";
        }
    }
}

public class BuildingManager : MonoBehaviour
{
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent; // happens to be the object it is on
    // Components
    private OutpostManagementUI omui;
    private ResourcesUI rui;
    private bool isPlacing = false;
    private int activeBuildingID;
    private Building prospectiveBuilding;
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    public UnityEvent placedBuilding;
    public UnityEvent cancelBuilding;
    // Tracking
    public Dictionary<int, Building> buildings;
    public BuildingResources totalProduction;

    private void Start()
    {
        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }// init both of these
        if (rui == null) { rui = FindObjectOfType<ResourcesUI>(); }

        // Init Building (make own function)
        buildings = new Dictionary<int, Building>(); //GameManager.Data.buildings.Count
        for (int i = 0; i < GameManager.Data.buildings.Count; i ++) // cache buildings list?
        {
            Building building = Instantiate(buildingPrefabs[GameManager.Data.buildings[i].uiIndex], buildingParent);
            building.id = GameManager.Data.buildings[i].id;
            building.uiIndex = GameManager.Data.buildings[i].uiIndex;
            building.level = GameManager.Data.buildings[i].level;
            building.transform.position = GameManager.Data.buildings[i].position;
            building.transform.rotation = GameManager.Data.buildings[i].rotation;
            building.CompleteBuild(); // dont keep, but is used to complete for now
            buildings.Add(building.id, building);
        }

        // Fill UI - probably combine?
        omui.FillConstructionUI(this, buildingPrefabs);
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
            Destroy(prospectiveBuilding.gameObject);
        }
        isPlacing = true;

        Building prefab = buildingPrefabs[index];
        prospectiveBuilding = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, buildingParent);
        prospectiveBuilding.uiIndex = index; // stores type
    }
    // placing a building
    private void SpawnBuilding(Building prospectiveBuilding, Vector3 position)
    {
        isPlacing = false;
        placedBuilding.Invoke();

        // Check if there are enough resources - possible move
        BuildingResources cost = prospectiveBuilding.Cost;
        if (GameManager.Data.resources.wood < cost.wood)
        {
            Destroy(prospectiveBuilding.gameObject);
            Debug.Log("Cannot build; Insufficient resources"); // UI
            return;
        }

        // Subtract resources - move to Building or bring CanBuild in here
        GameManager.data.resources.wood -= cost.wood;
        rui.UpdateWoodUI(GameManager.Data.resources.wood);

        // Create Building data
        BuildingData data = new BuildingData();
        data.id = prospectiveBuilding.id;
        data.uiIndex = prospectiveBuilding.uiIndex;
        data.level = prospectiveBuilding.level;
        data.position = prospectiveBuilding.transform.position;
        data.rotation = prospectiveBuilding.transform.rotation;
        GameManager.AddBuilding(data);

        // Building type
        if (prospectiveBuilding.uiIndex == 0)
        {
            GameManager.data.resources.foodProduction += prospectiveBuilding.resourceProduction.food;
            rui.UpdateFoodUI(GameManager.Data.resources);
        }
        else if (prospectiveBuilding.uiIndex == 1)
        {
            GameManager.data.outpostCrewCapacity += 8;
            rui.UpdateCrewCapacityUI(GameManager.Data.outpostCrewCapacity);
        }
        else if (prospectiveBuilding.uiIndex == 2)
        {
            // ?
        }
        prospectiveBuilding.Place();

        buildings.Add(prospectiveBuilding.id, prospectiveBuilding);
        onBuildingPlaced.Raise(this, prospectiveBuilding);
    }
    internal string UpgradeBuilding(int buildingID)
    {
        return buildings[buildingID].Upgrade();
    }
    internal void DemolishBuilding(int buildingID)
    {
        Building building = buildings[buildingID];

        // Add resources
        BuildingResources cost = prospectiveBuilding.Cost;
        if(building.IsComplete)
        {
            GameManager.data.resources.wood += (cost.wood/2);
        }
        else
        {
            GameManager.data.resources.wood += cost.wood;
        }
        rui.UpdateWoodUI(GameManager.Data.resources.wood);

        // Building type
        if (building.uiIndex == 0)
        {

            GameManager.data.resources.foodProduction -= building.resourceProduction.food;
            rui.UpdateFoodUI(GameManager.Data.resources);
        }
        else if (building.uiIndex == 1)
        {
            GameManager.data.outpostCrewCapacity -= 8;
            rui.UpdateCrewCapacityUI(GameManager.Data.outpostCrewCapacity);
        }
        else if (building.uiIndex == 2)
        {
            // ...?
        }

        GameManager.RemoveBuilding(buildingID);
        buildings[buildingID].Demolish();
        buildings.Remove(buildingID);
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

            prospectiveBuilding.transform.position = info.point;

            // angle of incline check
            if(info.normal.y < .9f)
            {
                prospectiveBuilding.PlacementInvalid();
            }
            else
            {
                prospectiveBuilding.PlacementValid();
                // check collision
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && prospectiveBuilding.isPlacementValid)
                {
                    SpawnBuilding(prospectiveBuilding, info.point);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    isPlacing = false;
                    Destroy(prospectiveBuilding.gameObject);
                    cancelBuilding.Invoke();
                }
            }
        }
    }

    // Dev
    internal void BuildAll()
    {
        foreach (Building building in buildings.Values)
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
