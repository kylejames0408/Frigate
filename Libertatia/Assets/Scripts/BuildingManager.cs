﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Events")]
    public GameEvent onBuildingPlaced;

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
    }
    private void Start()
    {
        currentResources = new int[] { 1000, 1000 };

        buildings = new List<Building>();
        if (GameManager.Instance.Buildings.Count > 0)
        {
            foreach (Building b in GameManager.Instance.Buildings)
            {
                buildings.Add(b);
                b.Build(100);
            }
        }

        //ui = FindObjectOfType<BuildingUI>();
        //if (ui)
        //{
        //    ui.RefreshResources();
        //}
    }

    private void Update()
    {
        int assigned = 0;
        foreach (Building building in buildings)
        {
            if(building.IsAssigned)
            {
                assigned++;
            }
        }
        if(assigned >=2)
        {
            BuildingUI.Instance.attackBtn.SetActive(true);
        }
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
        GameManager.Instance.Buildings.Add(building);
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
}