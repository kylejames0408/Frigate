using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum BuildingState
{
    PLACING,
    WAITING_FOR_ASSIGNMENT,
    CONSTRUCTING,
    BUILT,
    COUNT
}

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
    [Header("Building State Data")]
    [SerializeField] private BuildingStateData stateData;
    [Header("Building Types")]
    [SerializeField] private Building[] buildingPrefabs;
    [Header("Components")]
    [SerializeField] private OutpostManagementUI omui;
    [SerializeField] private ResourcesUI rui;
    [SerializeField] private BuildingUI buildingUI;
    [Header("Tracking")]
    [SerializeField] private bool isPlacing = false;
    [SerializeField] private Building prospectiveBuilding;
    // Tracking
    [SerializeField] private Dictionary<int, Building> buildings;
    [SerializeField] private BuildingResources totalProduction;


    private void Start()
    {
        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }// init both of these
        if (rui == null) { rui = FindObjectOfType<ResourcesUI>(); }

        // Init Building (make own function)
        buildings = new Dictionary<int, Building>(); //GameManager.Data.buildings.Count
        for (int i = 0; i < GameManager.Data.buildings.Count; i ++) // cache buildings list?
        {
            BuildingData data = GameManager.Data.buildings[i];
            Building building = Instantiate(buildingPrefabs[data.uiIndex], transform);
            building.id = data.id;
            building.uiIndex = data.uiIndex;
            building.level = data.level;
            building.transform.position = data.position;
            building.transform.rotation = data.rotation;
            building.CompleteConstruction(); // dont keep, but is used to complete for now, will be using AP
            buildings.Add(building.id, building);
        }

        // Fill UI - probably combine?
        omui.FillConstructionUI(this, buildingPrefabs);
    }
    private void Update()
    {
        HandlePlacing();
    }
    private void OnDestroy()
    {
        // Update player data when scene is unloaded
        //realtimeData.resources = resources; // disable for now
        //GameManager.Instance.buildingAmount = buildings.Count;
        //GameManager.Instance.DataManager.Update(realtimeData);
    }

    // Actions
    internal void SelectBuilding(int buildingIndex)
    {
        if (isPlacing)
        {
            Destroy(prospectiveBuilding.gameObject);
        }
        isPlacing = true;

        Building prefab = buildingPrefabs[buildingIndex];
        prospectiveBuilding = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, transform);
        prospectiveBuilding.uiIndex = buildingIndex; // stores type
        prospectiveBuilding.SetMaterial(stateData.matPlacing);
    }
    private void SpawnBuilding(Building prospectiveBuilding)
    {
        isPlacing = false;
        omui.DeselectBuildingCard(prospectiveBuilding.uiIndex);

        // Subtract resources - move to Building or bring CanBuild in here
        GameManager.data.resources.wood -= prospectiveBuilding.Cost.wood;
        rui.UpdateWoodUI(GameManager.Data.resources.wood);

        // Create Building data
        BuildingData data = new BuildingData();
        data.id = prospectiveBuilding.id;
        data.uiIndex = prospectiveBuilding.uiIndex;
        data.level = prospectiveBuilding.level;
        data.position = prospectiveBuilding.transform.position;
        data.rotation = prospectiveBuilding.transform.rotation;
        GameManager.AddBuilding(data);

        // TODO: turn into switch statement
        // Building type
        if (prospectiveBuilding.uiIndex == 0)
        {
            GameManager.data.resources.foodProduction += prospectiveBuilding.resourceProduction.food; // should probably use these buildingPrefabs[prospectiveBuilding.uiIndex], that would mean keeping the costs and production in the manager which might be better. This would just need the number of each type to update the UI
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
        // TODO: Compile functions into one if possible
        prospectiveBuilding.Place();
        prospectiveBuilding.SetMaterial(stateData.matRecruiting);
        prospectiveBuilding.SetUI(stateData.iconRecruiting, stateData.iconEmptyAsssignment);
        prospectiveBuilding.onFirstAssignment.AddListener(() => { OnFirstAssignmentCallback(prospectiveBuilding.id); });
        prospectiveBuilding.onCrewmateAssigned.AddListener(() => { OnCrewmateAssignedCallback(prospectiveBuilding.id); });
        prospectiveBuilding.onConstructionCompleted.AddListener(() => { OnConstructionCompletedCallback(prospectiveBuilding.id); });
        prospectiveBuilding.onSelection.AddListener(() => { OnSelectionCallback(prospectiveBuilding.id); });
        prospectiveBuilding.onCollision.AddListener(() => { OnCollisionCallback(prospectiveBuilding.id); });
        prospectiveBuilding.onNoCollisions.AddListener(() => { OnNoCollisionsCallback(prospectiveBuilding.id); });

        buildings.Add(prospectiveBuilding.id, prospectiveBuilding);
    }

    internal void UpgradeBuilding(int buildingID)
    {
        buildings[buildingID].Upgrade();
        buildingUI.SetStatusUI(buildings[buildingID].GetStatus());
    }
    internal void DemolishBuilding(int buildingID)
    {
        Building building = buildings[buildingID];

        // Add resources
        BuildingResources cost = prospectiveBuilding.Cost; // change to reference rules (prefabs)
        if(building.IsBuilt)
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

    // Callbacks
    private void OnFirstAssignmentCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        building.SetMaterial(stateData.matConstructing);
        building.SetStatusIcon(stateData.iconConstructing);
        building.onFirstAssignment.RemoveAllListeners();
    }
    private void OnCrewmateAssignedCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        buildingUI.FillUI(building);
        buildingUI.OpenMenu();
    }
    private void OnConstructionCompletedCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        building.SetStatusIcon(stateData.iconBuilt);
    }
    private void OnSelectionCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        buildingUI.FillUI(building);
        buildingUI.OpenMenu();
    }
    private void OnCollisionCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        building.SetMaterial(stateData.matColliding);
    }
    private void OnNoCollisionsCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        Material buildingMaterial = null;
        switch (building.state)
        {
            case BuildingState.PLACING:
                buildingMaterial = stateData.matPlacing;
                break;
            case BuildingState.WAITING_FOR_ASSIGNMENT:
                buildingMaterial = stateData.matRecruiting;
                break;
            case BuildingState.CONSTRUCTING:
                buildingMaterial = stateData.matConstructing;
                break;
        }
        building.SetMaterial(buildingMaterial);
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
                prospectiveBuilding.SetMaterial(stateData.matColliding);
            }
            else
            {
                if(prospectiveBuilding.IsColliding)
                {
                    prospectiveBuilding.SetMaterial(stateData.matColliding);
                }
                else
                {
                    prospectiveBuilding.SetMaterial(stateData.matPlacing);
                }

                // check collision
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !prospectiveBuilding.IsColliding)
                {
                    SpawnBuilding(prospectiveBuilding);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    isPlacing = false;
                    omui.DeselectBuildingCard(prospectiveBuilding.uiIndex);
                    Destroy(prospectiveBuilding.gameObject);
                }
            }
        }
    }

    // UI
    internal bool CanConstructBuilding(int buildingIndex)
    {
        return GameManager.Data.resources.wood >= buildingPrefabs[buildingIndex].Cost.wood;
    }

    // Dev
    internal void BuildAll()
    {
        foreach (Building building in buildings.Values)
        {
            building.CompleteConstruction();
        }
    }
}