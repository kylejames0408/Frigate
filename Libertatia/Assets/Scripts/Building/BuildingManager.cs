using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum BuildingState
{
    PLACING,
    RECRUIT,
    CONSTRUCTING,
    BUILT,
    COUNT
}

// Might need to separate into cost and production
[Serializable]
public struct BuildingResources
{
    public int ap; // required
    public int wood; // boost in production
    public int food; // boost in production
    public int space; // boost in housing 1x

    public static BuildingResources operator /(BuildingResources resources, int divisor)
    {
        resources.wood /= divisor;
        resources.food /= divisor;
        resources.space /= divisor;
        resources.ap /= divisor;
        return resources;
    }
    public override string ToString()
    {
        if (wood > 0)
        {
            return wood + " wood";
        }
        else if (food > 0)
        {
            return food + " food";
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
    [SerializeField] private BuildingStateData stateData; // Remove if not used elsewhere or copies are not needed
    [Header("Building Types")]
    [SerializeField] private Building[] buildingPrefabs;
    [Header("UI")]
    [SerializeField] private OutpostManagementUI omUI;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private ConfirmationUI confirmationUI;
    [Header("Components")] // need this for selection data
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ResourceManager rm;
    [SerializeField] private Ship ship;
    [Header("Placing")]
    [SerializeField] private bool isDragging = false;
    [SerializeField] private bool canPlace = false;
    [SerializeField] private Building prospectiveBuilding;
    [Header("Tracking")]
    [SerializeField] private Dictionary<int, Building> buildings;
    [SerializeField] private int housingSpace;
    [SerializeField] private int selectedBuildingID;
    [Header("Events")]
    public UnityEvent<int, string> onBuildingPlaced;
    public UnityEvent<int> onCrewmateAssigned;

    public Building[] Buildings
    {
        get { return buildings.Values.ToArray(); }
    }

    private void Awake()
    {
        if (omUI == null) { omUI = FindObjectOfType<OutpostManagementUI>(); }
        if (buildingUI == null) { buildingUI = FindObjectOfType<BuildingUI>(); }
        if (confirmationUI == null) { confirmationUI = FindObjectOfType<ConfirmationUI>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rm == null) { rm = FindObjectOfType<ResourceManager>(); }

        confirmationUI.gameObject.SetActive(false);
        buildings = new Dictionary<int, Building>();
    }
    private void Start()
    {
        buildingUI.onUnassign.AddListener(OnUnassignCrewmate);
        buildingUI.btnUpgrade.onClick.AddListener(OnUpgradeBuildingCallback);
        buildingUI.btnDemolish.onClick.AddListener(OnDemolishBuildingCallback);
        buildingUI.onClickBuilding.AddListener(OnClickBuildingIconCallback);
        buildingUI.onClickAssignee.AddListener(OnClickAssigneeIconCallback);

        // Combine - check if default? dont think so since player data inits it
        SpawnExistingBuildings(PlayerDataManager.LoadOutpostBuildings());
        housingSpace = PlayerDataManager.LoadOutpostCrewCapacity();

        // Fill UI - probably combine?
        omUI.onBeginDraggingBuildingCard.AddListener(OnBeginDraggingBuildingCardCallback);
        omUI.onEndDraggingBuildingCard.AddListener(OnEndDraggingBuildingCardCallback);
        omUI.FillConstructionUI(buildingPrefabs);
    }
    private void Update()
    {
        if(isDragging)
        {
            HandleDragging();
        }
    }
    private void OnDestroy()
    {
        PlayerDataManager.SaveOutpostData(buildings.Values.ToArray(), housingSpace);
    }

    // Actions
    private void SpawnExistingBuildings(BuildingData[] buildingData)
    {
        for (int i = 0; i < buildingData.Length; i++)
        {
            BuildingData data = buildingData[i];
            Building building = Instantiate(buildingPrefabs[data.uiIndex], transform);
            building.Init(data);// if building, should check with resources

            // Maybe distro to completing construction?
            if (building.IsBuilt)
            {
                building.SetUI(stateData.iconBuilt, stateData.iconEmptyAsssignment);
            }
            else if (building.State == BuildingState.RECRUIT)
            {
                building.SetUI(stateData.iconRecruiting, stateData.iconEmptyAsssignment);
            }
            else if (building.State == BuildingState.CONSTRUCTING)
            {
                building.SetUI(stateData.iconConstructing, stateData.iconEmptyAsssignment);
            }

            // Set callbacks
            building.onCrewmateAssigned.AddListener(() => { OnCrewmateAssignedCallback(building.ID); });
            building.onConstructionCompleted.AddListener(() => { OnConstructionCompletedCallback(building.ID); });
            building.onSelection.AddListener(() => { OnSelectionCallback(building.ID); });
            building.onFreeAssignees.AddListener(() => { OnUnassignCrewmatesCallback(building.ID); });

            // Tracking
            buildings.Add(building.ID, building);

            // Will be checking for AP consumption
            if (!building.IsBuilt)
            {
                building.CompleteConstruction(); // dont keep, but is used to complete for now, will be using AP
            }
        }
    }
    private void SpawnNewBuilding(Building prospectiveBuilding)
    {
        isDragging = false;
        omUI.DeselectBuildingCard(prospectiveBuilding.Type);

        // TODO: Compile functions into one if possible
        prospectiveBuilding.Place();
        prospectiveBuilding.SetMaterial(stateData.matRecruiting);
        prospectiveBuilding.SetUI(stateData.iconRecruiting, stateData.iconEmptyAsssignment);

        // Set callbacks
        prospectiveBuilding.onFirstAssignment.AddListener(() => { OnFirstAssignmentCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onCrewmateAssigned.AddListener(() => { OnCrewmateAssignedCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onConstructionCompleted.AddListener(() => { OnConstructionCompletedCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onSelection.AddListener(() => { OnSelectionCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onCollision.AddListener(() => { OnCollisionCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onNoCollisions.AddListener(() => { OnNoCollisionsCallback(prospectiveBuilding.ID); });
        prospectiveBuilding.onFreeAssignees.AddListener(() => { OnUnassignCrewmatesCallback(prospectiveBuilding.ID); });

        // Log building locally
        buildings.Add(prospectiveBuilding.ID, prospectiveBuilding);

        // Update outpost and resource data
        rm.AddBuilding(buildingPrefabs[prospectiveBuilding.Type].Cost, buildingPrefabs[prospectiveBuilding.Type].Production);
        housingSpace += buildingPrefabs[prospectiveBuilding.Type].Production.space;
        rm.UpdateHousingSpace(housingSpace);

        // Trigger events
        onBuildingPlaced.Invoke(prospectiveBuilding.Type, prospectiveBuilding.Name);
    }
    private void UpgradeBuilding(int buildingID)
    {
        buildings[buildingID].Upgrade();
        buildingUI.SetStatusUI(buildings[buildingID].GetStatus());
    }
    private void DemolishBuilding(int buildingID)
    {
        Building building = buildings[buildingID];

        // Update Data
        BuildingResources cost = buildingPrefabs[building.Type].Cost; // will eventially change with level
        if (building.IsBuilt)
        {
            rm.RemoveBuilding(cost / 2, buildingPrefabs[building.Type].Production); // check if this works
        }
        else
        {
            rm.RemoveBuilding(cost, buildingPrefabs[building.Type].Production);
        }
        housingSpace -= buildingPrefabs[building.Type].Production.space;
        rm.UpdateHousingSpace(housingSpace);

        // Delete building
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
                if (!building.CanAssign())
                {
                    break;
                }

                // Assign them to the building
                building.AssignCrewmate(selectedCrewmates[i]); // assigns building to player too
                onCrewmateAssigned.Invoke(selectedCrewmates[i].ID);
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
        selectedBuildingID = buildingID;
        buildingUI.FillAndOpenInterface(building);
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
            case BuildingState.RECRUIT:
                buildingMaterial = stateData.matRecruiting;
                break;
            case BuildingState.CONSTRUCTING:
                buildingMaterial = stateData.matConstructing;
                break;
        }
        building.SetMaterial(buildingMaterial);
    }
    private void OnUnassignCrewmatesCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        if (!building.Assignee1.IsEmpty())
        {
            cm.UnassignCrewmate(building.Assignee1.id);
        }
        if (!building.Assignee2.IsEmpty())
        {
            cm.UnassignCrewmate(building.Assignee2.id);
        }
    }
    private void OnUpgradeBuildingCallback()
    {
        UpgradeBuilding(selectedBuildingID);
    }
    private void OnDemolishBuildingCallback()
    {
        Building building = buildings[selectedBuildingID];
        confirmationUI.SetDemolishState(building.IsBuilt);
        confirmationUI.btnApprove.onClick.AddListener(OnApproveDemolishCallback);
        confirmationUI.btnDecline.onClick.AddListener(OnDeclineDemolishCallback);
        confirmationUI.gameObject.SetActive(true);
    }
    private void OnApproveDemolishCallback() // might need the building ID as param for consistancy
    {
        confirmationUI.btnApprove.onClick.RemoveListener(OnApproveDemolishCallback);
        confirmationUI.btnDecline.onClick.RemoveListener(OnDeclineDemolishCallback);
        confirmationUI.gameObject.SetActive(false);
        DemolishBuilding(selectedBuildingID);
        buildingUI.CloseInterface();
        selectedBuildingID = -1;
    }
    private void OnDeclineDemolishCallback()
    {
        confirmationUI.btnApprove.onClick.RemoveListener(OnApproveDemolishCallback);
        confirmationUI.btnDecline.onClick.RemoveListener(OnDeclineDemolishCallback);
        confirmationUI.gameObject.SetActive(false);
    }
    public void OnCrewmateDropAssign()
    {
        foreach (Building building in buildings.Values)
        {
            building.HandleAssignmentDragDrop();
        }
    }
    private void OnUnassignCrewmate(int crewmateID)
    {
        UnassignBuilding(selectedBuildingID, crewmateID);
        cm.UnassignCrewmate(crewmateID);
    }
    internal void OnClickBuildingIconCallback(int buildingID)
    {
        Building building = buildings[buildingID];
        CameraManager.Instance.PanTo(building.transform.position);
    }
    internal void PanToShip()
    {
        CameraManager.Instance.PanTo(ship.transform.position);
    }
    internal void OnClickAssigneeIconCallback(int buildingID, int assigneeIndex)
    {
        Building building = buildings[buildingID];
        cm.OnClickCrewmateIconCallback(building.Assignees[assigneeIndex].id);
    }
    private void OnBeginDraggingBuildingCardCallback(int buildingIndex)
    {
        isDragging = true;
        canPlace = false;
        // Creates building to follow cursor
        Building prefab = buildingPrefabs[buildingIndex];
        prospectiveBuilding = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, transform);
        prospectiveBuilding.SetType(buildingIndex);
        prospectiveBuilding.SetMaterial(stateData.matPlacing);
    }
    private void OnEndDraggingBuildingCardCallback(int buildingIndex)
    {
        isDragging = false;
        // Checks building placement validity
        if (!EventSystem.current.IsPointerOverGameObject() && canPlace)
        {
            SpawnNewBuilding(prospectiveBuilding);
        }
        else
        {
            Destroy(prospectiveBuilding.gameObject);
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

        if (building.State == BuildingState.RECRUIT)
        {
            building.SetMaterial(stateData.matRecruiting);
            building.onFirstAssignment.AddListener(() => { OnFirstAssignmentCallback(building.ID); }); // might be able to omit this and just embedd its func
        }
    }

    // Handlers
    private void HandleDragging()
    {
        // Camera.main or Camera.current?
        Physics.Raycast(CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition),
            out RaycastHit info, 300, LayerMask.GetMask("Terrain"));

        prospectiveBuilding.transform.position = info.point;

        // angle of incline check
        if (prospectiveBuilding.IsColliding || info.normal.y < .9f)
        {
            prospectiveBuilding.SetMaterial(stateData.matColliding);
            canPlace = false;
        }
        else
        {
            prospectiveBuilding.SetMaterial(stateData.matPlacing);
            canPlace = true;
        }
    }

    // UI
    internal bool CanConstructBuilding(int buildingIndex)
    {
        return rm.CanConstruct(buildingPrefabs[buildingIndex].Cost);
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