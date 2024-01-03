using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Might need to separate into cost and production
[Serializable]
public struct IslandResources
{
    public int wood;
    public int food;
    public int doubloons;

    public override string ToString()
    {
        string resources = string.Empty;
        if(wood > 0)
        {
            resources += "Wood";
        }
        else if(food > 0)
        {
            resources += "Food";
        }
        else
        {
            resources += "None";
        }
        return resources;
    }
}

// might be able to get away with tags, but this should work for now
public enum IslandType
{
    OUTPOST,
    ENEMY,
    DEFAULT,
    COUNT
}

public class IslandManager : MonoBehaviour
{
    // Components
    [SerializeField] private ResourceManager rm;
    [SerializeField] private Ship ship;
    [SerializeField] private PlayerPathfind pathfinder;
    // UI
    [SerializeField] private IslandUI uiIsland;
    [SerializeField] private ConfirmationUI confirmationUI;
    // Data
    [SerializeField] private Dictionary<int, Island> islands;
    [SerializeField] private int selectedIslandID = -1;
    [SerializeField] private bool isShipMoving = false;
    // Events
    public UnityEvent<IslandResources> onIslandCompleted;

    private void Awake()
    {
        if (rm == null) { rm = FindObjectOfType<ResourceManager>(); }
        if (ship == null) { ship = FindObjectOfType<Ship>(); }
        if (uiIsland == null) { uiIsland = FindObjectOfType<IslandUI>(); }
        if (confirmationUI == null) { confirmationUI = FindObjectOfType<ConfirmationUI>(); }

        confirmationUI.gameObject.SetActive(false);
    }
    private void Start()
    {
        // Get all islands
        Island[] allIslands = GetComponentsInChildren<Island>();
        islands = new Dictionary<int, Island>(allIslands.Length);
        foreach (Island island in allIslands)
        {
            // Add callbacks
            island.onSelect.AddListener(OnIslandSelectedCallback);

            islands.Add(island.ID, island);
        }
        uiIsland.onDepart.AddListener(OnDepartCallback);
        isShipMoving = false;

        onIslandCompleted.AddListener(OnIslandCompletedCallback);
    }

    private void OnIslandSelectedCallback(int islandID)
    {
        if(!isShipMoving)
        {
            //if(islandID != GameManager.Data.IslandID)
            //{

            //}

            selectedIslandID = islandID;
            Island island = islands[islandID];
            // Calculate distance and AP
            int ap = pathfinder.GetDistance(ship.transform.position);
            // Open Island Interface
            uiIsland.Fill(island, ap);
            uiIsland.OpenInterface();
        }
    }
    private void OnIslandCompletedCallback(IslandResources islandResources)
    {
        rm.CompleteIsland(islandResources);
    }
    private void OnDepartCallback()
    {
        confirmationUI.btnApprove.onClick.AddListener(OnApproveDepartureCallback);
        confirmationUI.btnDecline.onClick.AddListener(OnDeclineDepartureCallback);
        confirmationUI.gameObject.SetActive(true);
    }
    private void OnApproveDepartureCallback()
    {
        confirmationUI.btnApprove.onClick.RemoveListener(OnApproveDepartureCallback);
        confirmationUI.btnDecline.onClick.RemoveListener(OnDeclineDepartureCallback);
        confirmationUI.gameObject.SetActive(false);

        uiIsland.CloseInterface();

        // Travel to the island
        pathfinder.onNavFinish.AddListener(OnNavFinishCallback);
        isShipMoving = true;
        pathfinder.Depart(ship.transform.position);
    }
    private void OnDeclineDepartureCallback()
    {
        confirmationUI.btnApprove.onClick.RemoveListener(OnApproveDepartureCallback);
        confirmationUI.btnDecline.onClick.RemoveListener(OnDeclineDepartureCallback);
        confirmationUI.gameObject.SetActive(false);
    }
    private void OnNavFinishCallback()
    {
        isShipMoving = false;
        Island island = islands[selectedIslandID];
        PlayerDataManager.SaveShipData(ship);
        if (island.Type == IslandType.OUTPOST)
        {
            CeneManager.LoadOutpost();
        }
        else if (island.Type == IslandType.ENEMY)
        {
            CeneManager.LoadCombat();
        }
    }
}
