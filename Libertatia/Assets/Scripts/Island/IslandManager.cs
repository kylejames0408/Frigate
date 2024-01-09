using System;
using System.Collections.Generic;
using System.Linq;
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
    [Header("Data")]
    [SerializeField] private Sprite iconDefaultIsland;
    [Header("References")]
    [SerializeField] private ResourceManager rm;
    [SerializeField] private Ship ship;
    [SerializeField] private PlayerPathfind pathfinder;
    [Header("UI")]
    [SerializeField] private IslandUI intIsland;
    [SerializeField] private ConfirmationUI intConfirmation;
    [Header("Tracking")]
    [SerializeField] private Dictionary<int, Island> islands;
    [SerializeField] private int selectedIslandID = -1;
    [SerializeField] private int dockedIslandID = -1;
    [SerializeField] private bool isShipMoving = false;
    [Header("Events")]
    public UnityEvent<IslandResources> onIslandCompleted;
    public UnityEvent<IslandType> onIslandSelected;

    private void Awake()
    {
        if (rm == null) { rm = FindObjectOfType<ResourceManager>(); }
        if (ship == null) { ship = FindObjectOfType<Ship>(); }
        if (intIsland == null) { intIsland = FindObjectOfType<IslandUI>(); }
        if (intConfirmation == null) { intConfirmation = FindObjectOfType<ConfirmationUI>(); }

        intConfirmation.gameObject.SetActive(false);
    }
    private void Start()
    {
        ExplorationData explorationData = PlayerDataManager.LoadExplorationData();
        dockedIslandID = explorationData.dockedIslandID;
        if(explorationData.islands.Length > 0)
        {
            // Regenerate map
            //explorationData.islands
        }
        else
        {
            // generate map
            // - generate islands - to-be proc gen
            // - place on map
            Island[] allIslands = GetComponentsInChildren<Island>();
            islands = new Dictionary<int, Island>(allIslands.Length);
            foreach (Island island in allIslands)
            {
                island.SetIcon(iconDefaultIsland);
                island.onSelect.AddListener(OnIslandSelectedCallback);
                islands.Add(island.ID, island);
            }
        }

        isShipMoving = false;

        intIsland.onDepart.AddListener(OnDepartCallback);
        onIslandCompleted.AddListener(OnIslandCompletedCallback);
    }
    private void OnDestroy()
    {
        PlayerDataManager.SaveExplorationData(islands.Values.ToArray(), dockedIslandID);
    }

    // Callbacks
    private void OnIslandSelectedCallback(int islandID)
    {
        if(!isShipMoving)
        {
            selectedIslandID = islandID;
            Island island = islands[islandID];
            // Calculate distance and AP
            int ap = pathfinder.GetDistance(ship.transform.position); // distance needs better calc
            // Open Island Interface
            intIsland.Fill(island, ap);
            intIsland.OpenInterface();
            onIslandSelected.Invoke(island.Type);
        }
    }
    private void OnIslandCompletedCallback(IslandResources islandResources)
    {
        rm.CompleteIsland(islandResources);
    }
    private void OnDepartCallback()
    {
        intConfirmation.btnApprove.onClick.AddListener(OnApproveDepartureCallback);
        intConfirmation.btnDecline.onClick.AddListener(OnDeclineDepartureCallback);
        intConfirmation.gameObject.SetActive(true);
    }
    private void OnApproveDepartureCallback()
    {
        intConfirmation.btnApprove.onClick.RemoveListener(OnApproveDepartureCallback);
        intConfirmation.btnDecline.onClick.RemoveListener(OnDeclineDepartureCallback);
        intConfirmation.gameObject.SetActive(false);

        intIsland.CloseInterface();

        // Travel to the island
        pathfinder.onNavFinish.AddListener(OnNavFinishCallback);
        isShipMoving = true;
        pathfinder.Depart(ship.transform.position);
    }
    private void OnDeclineDepartureCallback()
    {
        intConfirmation.btnApprove.onClick.RemoveListener(OnApproveDepartureCallback);
        intConfirmation.btnDecline.onClick.RemoveListener(OnDeclineDepartureCallback);
        intConfirmation.gameObject.SetActive(false);
    }
    private void OnNavFinishCallback()
    {
        isShipMoving = false;
        Island island = islands[selectedIslandID];
        dockedIslandID = island.ID;
        PlayerDataManager.SaveShipData(ship);

        // Do we want it to Enter imediately? I think we decided yes but still should be discussed
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
