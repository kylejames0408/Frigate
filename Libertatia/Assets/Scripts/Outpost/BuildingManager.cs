using System;
using System.Collections.Generic;
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
    //
    public List<Crewmate> crewmates;
    public List<Building> buildings;
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent; // happens to be the object it is on
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    public UnityEvent placedBuilding;
    // Components
    private ConstructionUI constructionUI;
    private OutpostUI outpostUI;
    private bool isPlacing = false;
    private Building activeBuilding;
    // Crewmate Data
    public GameObject crewmatePrefab;
    public Transform crewmateParent;
    public Transform crewmateSpawn;
    public int crewmateSpawnRadius = 10;

    private void Start()
    {
        // should check building folder for buildings
        constructionUI = FindObjectOfType<ConstructionUI>(); // init both of these
        crewmateSpawn = crewmateParent.GetChild(0);
        crewmates = new List<Crewmate>(GameManager.Data.crewmates.Count);

        if(GameManager.Data.crewmates.Count == 0)
        {
            for (int i = 0; i < GameManager.Data.crewmates.Capacity; i++)
            {
                GameObject crewmate = Instantiate(crewmatePrefab, crewmateParent);
                crewmate.transform.position = crewmateSpawn.position + new Vector3(
                    UnityEngine.Random.Range(-1.0f, 1.0f) * crewmateSpawnRadius, 0,
                    UnityEngine.Random.Range(-1.0f, 1.0f) * crewmateSpawnRadius);
                Crewmate mate = crewmate.GetComponent<Crewmate>();
                CrewmateData data = new CrewmateData();
                data.icon = mate.Icon;
                data.name = mate.Name;
                data.buildingID = mate.buildingID;
                GameManager.Data.crewmates.Add(data);
                crewmates.Add(mate);
            }
        }
        else
        {
            for (int i = 0; i < GameManager.Data.crewmates.Count; i++)
            {
                CrewmateData data = GameManager.Data.crewmates[i];
                GameObject crewmate = Instantiate(crewmatePrefab, crewmateParent);
                Crewmate mate = crewmate.GetComponent<Crewmate>();
                mate.crewmateName = data.name;
                mate.icon = data.icon;
                mate.buildingID = data.buildingID;
                mate.transform.position = crewmateSpawn.position + new Vector3(
                    UnityEngine.Random.Range(-1.0f, 1.0f) * crewmateSpawnRadius, 0,
                    UnityEngine.Random.Range(-1.0f, 1.0f) * crewmateSpawnRadius);
                crewmates.Add(mate);
            }
        }

        constructionUI.FillCrewmateUI(this, GameManager.Data.crewmates.ToArray());

        buildings = new List<Building>();

        for (int i = 0; i < GameManager.Data.buildings.Count; i ++)
        {
            Building building = Instantiate(buildingPrefabs[GameManager.Data.buildings[i].uiIndex], buildingParent);
            building.id = GameManager.Data.buildings[i].id;
            building.uiIndex = GameManager.Data.buildings[i].uiIndex;
            building.level = GameManager.Data.buildings[i].level;
            building.builders = new List<Crewmate>(GameManager.Data.buildings[i].assignedCrewmateAmount);
            foreach(Crewmate mate in crewmates)
            {
                if(mate.buildingID == building.id)
                {
                    building.builders.Add(mate);
                }
            }
            building.transform.position = GameManager.Data.buildings[i].position;
            building.transform.rotation = GameManager.Data.buildings[i].rotation;
            building.CompleteBuild(); // dont keep, but is used to complete for now
            buildings.Add(building);
        }

        constructionUI.FillConstructionUI(this, buildingPrefabs);
        outpostUI = FindObjectOfType<OutpostUI>();
        outpostUI.Init();
    }
    private void Update()
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
            if (Input.GetMouseButtonDown(0) &&  !activeBuilding.IsColliding && !IsPointerOverUIObject())
            {
                SpawnBuilding(activeBuilding, info.point);
            }
        }
    }

    public Crewmate SelectCrewmate(int index)
    {
        CrewmateManager.Instance.ClickSelect(crewmates[index].gameObject);
        return crewmates[index];
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
        activeBuilding.uiIndex = index; // sets type
    }
    // placing a building
    public void SpawnBuilding(Building building, Vector3 position)
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
        //resources.Print();
        outpostUI.UpdateWoodUI(GameManager.Data.resources.wood);

        // Create Building
        BuildingData data = new BuildingData();
        data.id = building.id;
        data.uiIndex = building.uiIndex;
        data.level = building.level;
        data.assignedCrewmateAmount = building.builders.Count;
        data.position = building.transform.position;
        data.rotation = building.transform.rotation;
        GameManager.Data.buildings.Add(data);
        if (building.uiIndex == 0)
        {
            GameManager.data.resources.foodPerAP += 50;
            outpostUI.UpdateFoodConsumptionUI(GameManager.Data.resources.foodPerAP);
        }
        else if (building.uiIndex == 1)
        {
            GameManager.data.outpostCrewCapacity += 8;
            outpostUI.UpdateCrewCapacityUI(GameManager.Data.outpostCrewCapacity);
        }
        else if (building.uiIndex == 2)
        {
            // ?
        }
        buildings.Add(building);
        building.Place();

        onBuildingPlaced.Raise(this, building);
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
        //GameManager.Instance.buildingAmount = buildings.Count;
        //GameManager.Instance.DataManager.Update(realtimeData);
    }
}
