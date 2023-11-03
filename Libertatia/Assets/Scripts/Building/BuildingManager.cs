using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum BuildingState
{
    PLACING,
    WAITING_FOR_ASSIGNMENT,
    BUILDING,
    COMPLETE,
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
    [Header("Building Data")]
    // Placing
    [SerializeField] private Material matPlacing;
    // Colliding
    [SerializeField] private Material matColliding;
    // Recruiting
    [SerializeField] private Sprite iconRecruiting;
    [SerializeField] private Material matRecruiting;
    // Constructing
    [SerializeField] private Sprite iconConstructing;
    [SerializeField] private Material matConstructing;
    [SerializeField] private ParticleSystem particleConstructing;
    // Built
    [SerializeField] private Sprite iconBuilt;
    [SerializeField] private ParticleSystem particleBuilt;
    // Buildings
    [SerializeField] private Building[] buildingPrefabs;
    [Header("Components")]
    [SerializeField] private OutpostManagementUI omui;
    [SerializeField] private ResourcesUI rui;
    [Header("Tracking")]
    [SerializeField] private bool isPlacing = false;
    [SerializeField] private Building prospectiveBuilding;
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    public UnityEvent placedBuilding;
    public UnityEvent cancelBuilding;
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
            building.CompleteBuild(); // dont keep, but is used to complete for now, will be using AP
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
    }
    private void SpawnBuilding(Building prospectiveBuilding)
    {
        isPlacing = false;
        placedBuilding.Invoke();

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
                    SpawnBuilding(prospectiveBuilding);
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
            building.CompleteBuild();
        }
    }
}
