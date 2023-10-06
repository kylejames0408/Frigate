using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BuildingManager : MonoBehaviour
{
    // Game/Player Data
    private List<Building> buildings;
    private Resources resources;
    private PlayerData realtimeData;
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent; // happens to be the object it is on
    // - Building Triggerables
    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    // Components
    private BuildingUI ui;
    private bool isPlacing = false;
    private Building activeBuilding;

    private void Start()
    {
        // should check building folder for buildings
        realtimeData = GameManager.Instance.DataManager.Data;
        if(buildings == null) // GameManager would fill buildings if any, unless we just stick to the scene
        {
            buildings = new List<Building>();
        }
        else if (buildings.Count > 0)
        {
            foreach (Building b in buildings)
            {
                buildings.Add(b);
                b.Build(100);
            }
        }
        ui = FindObjectOfType<BuildingUI>();
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
    public void SelectBuilding(int index)
    {
        if (isPlacing)
        {
            Destroy(activeBuilding.gameObject);
        }
        isPlacing = true;

        activeBuilding = CreateBuilding(index, Vector3.zero);
        Rigidbody rb = activeBuilding.GetComponent<Rigidbody>();
    }
    private Building CreateBuilding(int index, Vector3 position)
    {
        Building prefab = buildingPrefabs[index];
        return Instantiate(prefab, position, prefab.transform.rotation, buildingParent);
    }
    private void PurchaseBuilding(Building building)
    {
        int[] cost = building.Cost;
        resources.wood -= cost[0];
        resources.gold -= cost[1];
    }
    // figure out this function
    public void SpawnBuilding(Building building, Vector3 position)
    {
        Destroy(building.gameObject);
        isPlacing = false;

        // Check if there are enough resources - possible move
        //Building buildingPrefab = buildingPrefabs[index];
        if (!building.CanBuild(resources))
        {
            Debug.Log("Cannot build; Insufficient resources"); // UI
            return;
        }

        // Create Building
        //Building building = CreateBuilding(buildingPrefab, position);
        buildings.Add(building);

        // Subtract resources
        PurchaseBuilding(building);
    }
    public Building GetBuildingPrefab(int index)
    {
        return buildingPrefabs[index];
    }
    // Dev functions
    public void BuildAll(int work)
    {
        foreach (Building building in buildings)
        {
            building.Build(work);
        }

    }

    // Update player data when scene is unloaded
    private void OnDestroy()
    {
        realtimeData.resources = resources;
        realtimeData.buildingAmount = buildings.Count;
        //GameManager.Instance.DataManager.Update(realtimeData);
    }
}
