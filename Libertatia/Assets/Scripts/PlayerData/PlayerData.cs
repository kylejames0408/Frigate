using System;
using UnityEngine;

[Serializable]
public struct ObjectData
{
    public int id;
    public Sprite icon;

    public ObjectData(int id, Sprite icon)
    {
        this.id = id;
        this.icon = icon;
    }

    public void Reset(Sprite emptyAssigneeIcon)
    {
        id = -1;
        icon = emptyAssigneeIcon;
    }

    internal bool IsEmpty()
    {
        return id == -1;
    }
}
[Serializable]
public struct BuildingData
{
    // Tracking / State
    public int id;
    public ObjectData assignee1;
    public ObjectData assignee2;
    public BuildingState state;
    // Characteristics
    public string name;
    public int uiIndex;
    public int level;
    public string output;
    // UI
    public Sprite icon;
    // Spacial
    public Vector3 position;
    public Quaternion rotation;

    public BuildingData(Building building)
    {
        id = building.ID;
        assignee1 = building.Assignee1;
        assignee2 = building.Assignee2;
        state = building.State;
        name = building.Name;
        uiIndex = building.Type;
        level = building.Level;
        output = building.Output;
        icon = building.Icon;
        position = building.transform.position;
        rotation = building.transform.rotation;
    }
}
[Serializable]
public struct CrewmateData
{
    // Tracking / State
    public int id;
    public ObjectData building;
    public CrewmateState state;
    // Characteristics
    public string name;
    public int health;
    public int strength;
    public int agility;
    public int stamina;
    // UI
    public Sprite icon;
    // Spacial
    public Vector3 position;
    public Quaternion rotation;

    public CrewmateData(Crewmate mate)
    {
        id = mate.ID;
        building = mate.Building;
        state = mate.State;
        name = mate.FirstName;
        health = mate.Health;
        strength = mate.Strength;
        agility = mate.Agility;
        stamina = mate.Stamina;
        icon = mate.Icon;
        position = mate.transform.position;
        rotation = mate.transform.rotation;
    }
}
[Serializable]
public struct ResourceProduction
{
    public int wood;
    public int food;
}
[Serializable]
public struct ResourceConsumption
{
    public int wood;
    public int food;
}
[Serializable]
public struct ResourceData
{
    // Misc
    public int loyalty;
    // Storage
    public int wood;
    public int doubloons;
    public int food; // how much the player owns
    // Production per AP
    public ResourceProduction production;
    // Cost per AP
    public ResourceConsumption consumption;

    public ResourceData(int wood, int doubloons, int food, int loyalty)
    {
        this.wood = wood;
        this.doubloons = doubloons;
        this.food = food;
        this.loyalty = loyalty;
        production = new ResourceProduction();
        consumption = new ResourceConsumption();
    }
}
[Serializable]
public struct OutpostData
{
    public int crewCapacity; // housing space
    public CrewmateData[] crew;
    public BuildingData[] buildings;

    public OutpostData(int startingCrewCapacity)
    {
        crewCapacity = startingCrewCapacity;
        crew = new CrewmateData[0];
        buildings = new BuildingData[0];
    }
}
[Serializable]
public struct ShipData
{
    // Tracking / State
    public bool isInitialized; // will likely be turned into # of islands traveled to
    public int id;
    public int islandID;
    public int crewCcapacity;
    public CrewmateData[] crew;
    public ShipState state; // might turn into bigger enum
    // UI
    public Sprite icon;
    // Spacial
    public Vector3 position;
    public Quaternion rotation;

    public ShipData(int startingCrewCapacity)
    {
        isInitialized = false;
        id = -1;
        islandID = -1;
        crewCcapacity = startingCrewCapacity;
        crew = new CrewmateData[0];
        state = ShipState.OFFBOARDING;
        icon = null; // probably init
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    public ShipData(Ship ship)
    {
        id = ship.ID;
        islandID = ship.IslandID;
        crewCcapacity = ship.Capacity;
        crew = ship.Crewmates;
        state = ship.State;
        icon = ship.Icon;
        position = ship.transform.position;
        rotation = ship.transform.rotation;
        isInitialized = true;
    }
}
[Serializable]
public struct Progress
{
    public bool hasCompletedTutorial;
    public int outpostVisitCount;
    public int combatVisitCount;
    public int explorationVisitCount;
}
[Serializable]
public struct IslandMapData // maybe map data - we will need island data for combat saving (zones, enemies, etc)
{
    public int id;
    public string name;
    public Sprite icon;
    public IslandType type;
    public int difficulty;
    public string resources;
    // Will probably want a position, rotation, and mesh/island mesh generation id when map is proc gen

    public IslandMapData(Island island)
    {
        id = island.ID;
        name = island.Name;
        icon = island.Icon;
        type = island.Type;
        difficulty = island.Difficulty;
        resources = island.Resources;
    }
}
[Serializable]
public struct ExplorationData // MapData
{
    public IslandMapData[] islands;
    public int dockedIslandID;

    public ExplorationData(int islandCount) // dont really need to pass anything through. Might be worth a init method instead
    {
        islands = new IslandMapData[0];
        dockedIslandID = -1;
    }
}
[Serializable]
public struct PlayerData
{
    // Game data
    public float elapsedTime;
    // Player data
    public ResourceData resources;
    public OutpostData outpost;
    public ShipData ship;
    public Progress progress;
    public ExplorationData exploration;
}