using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // Game/Player Data
    private List<Building> buildings; // might need building transform info for loading
    private Resources resources;
    // Building Data
    public Building[] buildingPrefabs;
    public Transform buildingParent;
    // - Building Triggerables
    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;
    // - Building Events
    [Header("Events")]
    public GameEvent onBuildingPlaced;
    // Components
    private BuildingUI ui;

    private void Start()
    {
        resources = GameManager.Instance.Resources;
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
    }

    public void SpawnBuilding(int index, Vector3 position)
    {
        Building buildingPrefab = buildingPrefabs[index];
        if (!buildingPrefab.CanBuild(resources))
        {
            Debug.Log("Insufficient resources; cannot build");
            return;
        }

        // Create Building
        Building building = Instantiate(buildingPrefab, position, buildingPrefab.transform.rotation); //buildingParent
        building.Place();
        AddBuilding(building);
        if (building.isAttackable)
        {
            building.DamageScript.onDestroy.AddListener(() => RemoveBuilding(building));
        }

        onBuildingPlaced.Raise(this, index);

        // Give builders build task if they are free
        // TODO: Make external call and feed in building, return bool: bool AssignBuilder(Building bdg)
        //foreach (Actor actor in ActorManager.instance.Actors)
        //{
        //    if (actor is Builder)
        //    {
        //        Builder builder = actor as Builder;
        //        if (!builder.HasTask())
        //        {
        //            builder.GiveJob(building);
        //        }
        //    }
        //}

        // Subtract resources
        //int[] cost = building.Cost;
        //for (int i = 0; i < cost.Length; i++)
        //{
        //    currentResources[i] -= cost[i];
        //    //if (ui)
        //    //{
        //    //    ui.RefreshResources();
        //    //}
        //}
    }
    public void AddBuilding(Building building)
    {
        buildings.Add(building);
        DontDestroyOnLoad(building.gameObject);
        //GameManager.Instance.Buildings.Add(building);
    }
    public void RemoveBuilding(Building building)
    {
        buildings.Remove(building);
    }

    public void BuildAll(int work)
    {
        foreach (Building building in buildings)
        {
            building.Build(work);
            building.FreeBuilders();
        }

    }

    // Utility Functions
    public Building GetBuildingPrefab(int index)
    {
        return buildingPrefabs[index];
    }

    public void PlayParticle(Vector3 position)
    {
        if (buildParticle)
        {
            buildParticle.transform.position = position;
            buildParticle.Play();
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.SavePlayerData(resources);
    }
}
