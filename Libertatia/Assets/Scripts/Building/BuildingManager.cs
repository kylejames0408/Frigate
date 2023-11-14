using System;
using System.Collections.Generic;
using UnityEngine;
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
    public int space;

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
    [SerializeField] private CrewmateManager cm; // need this for selection data
    [Header("Tracking")]
    [SerializeField] private bool isPlacing = false;
    [SerializeField] private Building prospectiveBuilding;
    [SerializeField] private Dictionary<int, Building> buildings;
    [SerializeField] private BuildingResources totalProduction;
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    public GameEvent onCrewmateAssigned;

    private void Awake()
    {
        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }
        if (rui == null) { rui = FindObjectOfType<ResourcesUI>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }

        buildings = new Dictionary<int, Building>();
    }
    private void Start()
    {
        Debug.Log("Bldg Ct: " + GameManager.Data.buildings.Count);
        // Init Building (make own function)
        for (int i = 0; i < GameManager.Data.buildings.Count; i ++) // cache buildings list?
        {
            SpawnExistingBuilding(GameManager.Data.buildings[i]);
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
    private void SpawnNewBuilding(Building prospectiveBuilding)
    {
        isPlacing = false;
        omui.DeselectBuildingCard(prospectiveBuilding.Type);

        // TODO: Compile functions into one if possible
        prospectiveBuilding.Place();
        prospectiveBuilding.SetMaterial(stateData.matRecruiting);
        prospectiveBuilding.SetUI(stateData.iconRecruiting, stateData.iconEmptyAsssignment);

        // Save Data
        GameManager.AddBuilding(new BuildingData(prospectiveBuilding));
        Debug.Log("Ct Incr: "+ GameManager.Data.buildings.Count);

        // Set callbacks
        prospectiveBuilding.onFirstAssignment.AddListener(() => { OnFirstAssignmentCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onCrewmateAssigned.AddListener(() => { OnCrewmateAssignedCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onConstructionCompleted.AddListener(() => { OnConstructionCompletedCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onSelection.AddListener(() => { OnSelectionCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onCollision.AddListener(() => { OnCollisionCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onNoCollisions.AddListener(() => { OnNoCollisionsCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onFreeAssignees.AddListener(() => { OnFreeAssigneesCallback(prospectiveBuilding.ID); });

        // Set Game Event
        prospectiveBuilding.onCrewmateAssignedGE = onCrewmateAssigned;
        buildings.Add(prospectiveBuilding.ID, prospectiveBuilding);

        // Update Data
        GameManager.data.resources.wood -= buildingPrefabs[prospectiveBuilding.Type].Cost.wood; // Subtract resources
        GameManager.data.resources.foodProduction += buildingPrefabs[prospectiveBuilding.Type].Production.food; // will likely chance with level
        GameManager.data.outpostCrewCapacity += buildingPrefabs[prospectiveBuilding.Type].Production.space; // will be 8

        // Update UI - also make this one call
        rui.UpdateWoodUI(GameManager.Data.resources.wood);
        rui.UpdateFoodUI(GameManager.Data.resources);
        rui.UpdateCrewCapacityUI(GameManager.Data.outpostCrewCapacity);
        onBuildingPlaced.Raise(this, prospectiveBuilding);
    }
    private void SpawnExistingBuilding(BuildingData data)
    {
        Building building = Instantiate(buildingPrefabs[data.uiIndex], transform);
        building.Init(data);// if building, should check with resources

        // Maybe distro to completing construction?
        building.SetUI(stateData.iconRecruiting, stateData.iconEmptyAsssignment);

        // Set callbacks
        building.onCrewmateAssigned.AddListener(() => { OnCrewmateAssignedCallback(building.ID); });
        building.onConstructionCompleted.AddListener(() => { OnConstructionCompletedCallback(building.ID); });
        building.onSelection.AddListener(() => { OnSelectionCallback(building.ID); });
        building.onFreeAssignees.AddListener(() => { OnFreeAssigneesCallback(building.ID); });

        // Tracking
        buildings.Add(building.ID, building);

        // Will be checking for AP consumption
        building.CompleteConstruction(); // dont keep, but is used to complete for now, will be using AP

        building.onCrewmateAssignedGE = onCrewmateAssigned;
    }
    internal void SelectBuilding(int buildingIndex)
    {
        if (isPlacing)
        {
            Destroy(prospectiveBuilding.gameObject);
        }
        isPlacing = true;

        Building prefab = buildingPrefabs[buildingIndex];
        prospectiveBuilding = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, transform);
        prospectiveBuilding.SetType(buildingIndex); // stores type
        prospectiveBuilding.SetMaterial(stateData.matPlacing);
    }
    internal void UpgradeBuilding(int buildingID)
    {
        buildings[buildingID].Upgrade();
        buildingUI.SetStatusUI(buildings[buildingID].GetStatus());
    }
    internal void DemolishBuilding(int buildingID)
    {
        Building building = buildings[buildingID];

        // Update Data
        BuildingResources cost = buildingPrefabs[building.Type].Cost; // will eventially change with level
        if(building.IsBuilt)
        {
            GameManager.data.resources.wood += (cost.wood/2);
        }
        else
        {
            GameManager.data.resources.wood += cost.wood;
        }
        GameManager.data.resources.foodProduction -= buildingPrefabs[building.Type].Production.food;
        GameManager.data.outpostCrewCapacity -= buildingPrefabs[building.Type].Production.space;

        // Update UI
        rui.UpdateWoodUI(GameManager.Data.resources.wood);
        rui.UpdateFoodUI(GameManager.Data.resources);
        rui.UpdateCrewCapacityUI(GameManager.Data.outpostCrewCapacity);

        // Delete building
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

        if (cm.IsCrewmateSelected)
        {
            // Open UI - removed this since the crewmate UI will already be open
            //buildingUI.FillUI(building);
            //buildingUI.OpenMenu();

            // Get selected units
            Crewmate[] selectedCrewmates = cm.GetSelectedCrewmates();
            for (int i = 0; i < selectedCrewmates.Length; i++)
            {
                // Fill open positions
                if(!building.CanAssign())
                {
                    break;
                }

                // Assign them to the building
                building.AssignCrewmate(selectedCrewmates[i]); // assigns building to player too
            }
        }
        else
        {
            Debug.Log("No crewmate selected");
        }

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
        switch (building.State)
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
    private void OnFreeAssigneesCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        cm.FreeAssignees(building.Assignee1.id, building.Assignee2.id);
    }

    public void OnCrewmateDropAssign()
    {
        foreach (KeyValuePair<int, Building> kvp in buildings)
        {
            kvp.Value.HandleAssignmentDragDrop();
        }
    }

    // Utils
    internal Building GetBuilding(int buildingID)
    {
        return buildings[buildingID];
    }
    internal void UnassignBuilding(int buildingID, int crewmateID)
    {
        Building building = buildings[buildingID];
        building.UnassignCrewmate(crewmateID);

        if (building.State == BuildingState.WAITING_FOR_ASSIGNMENT)
        {
            building.SetMaterial(stateData.matRecruiting);
            building.onFirstAssignment.AddListener(() => { OnFirstAssignmentCallback(building.ID); }); // might be able to omit this and just embedd its func
        }
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
                    SpawnNewBuilding(prospectiveBuilding);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    isPlacing = false;
                    omui.DeselectBuildingCard(prospectiveBuilding.Type);
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