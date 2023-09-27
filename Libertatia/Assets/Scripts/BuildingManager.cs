using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Resource1,
    Resource2
}
public class BuildingManager : MonoBehaviour
{
    private static BuildingManager instance;
    private List<Building> buildings;
    public Building[] buildingPrefabs = default;
    public int[] currentResources = default; // this will be replaced
    public Transform buildingParent;

    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;
    private BuildingUI ui;
    public GameObject buildingDir;

    public static BuildingManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        buildings = new List<Building>();
    }
    private void Start()
    {
        currentResources = new int[] { 1000, 1000 };
        //ui = FindObjectOfType<BuildingUI>();
        //if (ui)
        //{
        //    ui.RefreshResources();
        //}
    }

    public void SpawnBuilding(int index, Vector3 position)
    {
        Building buildingPrefab = buildingPrefabs[index];
        if (!buildingPrefab.CanBuild(currentResources))
        {
            Debug.Log("Insufficient resources; cannot build");
            return;
        }

        // Create Building
        Building building = Instantiate(buildingPrefab, position, buildingPrefab.transform.rotation, buildingParent);
        buildings.Add(building);
        if(building.isAttackable)
        {
            building.DamageScript.onDestroy.AddListener(() => RemoveBuilding(building));
        }

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
    }
    public void RemoveBuilding(Building building)
    {
        buildings.Remove(building);
    }
    public void AddResource(ResourceType resourceType, int amount)
    {
        currentResources[(int)resourceType] += amount;

        //if(ui)
        //{
        //    ui.RefreshResources();
        //}
    }

    // Temp
    public void BuildAll(int work)
    {
        foreach (Building building in buildings)
        {
            building.Build(work);
        }
    }

    // Utility Functions
    public Building GetBuildingPrefab(int index)
    {
        return buildingPrefabs[index];
    }
    public Building GetRandomBuilding()
    {
        if (buildings.Count > 0)
        {
            return buildings[Random.Range(0, buildings.Count)];
        }
        else
        {
            return null;
        }
    }
    public void PlayParticle(Vector3 position)
    {
        if (buildParticle)
        {
            buildParticle.transform.position = position;
            buildParticle.Play();
        }
    }
}
