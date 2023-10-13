using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[Serializable]
public struct BuildingResources
{
    public int wood;
}

public class BuildingManager : MonoBehaviour
{
    // Game/Player Data
    private List<Building> buildings;
    private List<Crewmate> crewmates;
    private PlayerResourceData resources;
    private PlayerCrewData crewData;
    private PlayerBuildingData buildingData;
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent; // happens to be the object it is on
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
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
    public LayerMask buildingMask;

    private void Start()
    {
        // should check building folder for buildings
        resources = GameManager.Instance.GetResourceData();
        crewData = GameManager.Instance.GetCrewData();
        buildingData = GameManager.Instance.GetBuildingData();
        constructionUI = FindObjectOfType<ConstructionUI>(); // init both of these
        crewmateSpawn = crewmateParent.GetChild(0);

        if (crewmates == null)
        {
            crewmates = new List<Crewmate>(crewData.amount);
            for (int i = 0; i < crewmates.Capacity; i++)
            {
                GameObject crewmate = Instantiate(crewmatePrefab, crewmateParent);
                crewmate.transform.position = crewmateSpawn.position + new Vector3(
                    UnityEngine.Random.Range(-1.0f, 1.0f) * crewmateSpawnRadius, 0,
                    UnityEngine.Random.Range(-1.0f, 1.0f) * crewmateSpawnRadius);
                crewmates.Add(crewmate.GetComponent<Crewmate>());
            }
        }
        else if (crewmates.Count > 0)
        {
            foreach (Crewmate c in crewmates)
            {
                crewmates.Add(c);
            }
        }
        constructionUI.FillCrewmateUI(this, crewmates.ToArray());


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
        if (resources.wood < cost.wood)
        {
            Destroy(building.gameObject);
            Debug.Log("Cannot build; Insufficient resources"); // UI
            return;
        }

        // Subtract resources - move to Building or bring CanBuild in here
        resources.wood -= cost.wood;
        //resources.Print();

        outpostUI.UpdateWoodUI(resources.wood);
        outpostUI.UpdateDubloonUI(resources.doubloons);

        // Create Building
        building.Place();
        buildings.Add(building);

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
