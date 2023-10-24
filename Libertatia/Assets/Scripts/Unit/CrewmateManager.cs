using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrewmateManager : MonoBehaviour
{
    private static CrewmateManager instance;
    // Components
    private OutpostManagementUI omui;
    // Crewmate Data
    [SerializeField] private GameObject crewmatePrefab;
    [SerializeField] private Transform crewmateSpawn;
    public int crewmateSpawnRadius = 10;
    // Tracking
    public List<Crewmate> crewmates;
    public List<GameObject> unitsSelected = new List<GameObject>();

    public static CrewmateManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }// init both of these

        // Init Crewmates (make own function)
        crewmateSpawn = transform.GetChild(0);
        crewmates = new List<Crewmate>(); //GameManager.Data.crewmates.Count
        if (GameManager.Data.crewmates.Count == 0)
        {
            for (int i = 0; i < GameManager.Data.crewmates.Capacity; i++)
            {
                SpawnNewCrewmate();
            }
        }
        else
        {
            for (int i = 0; i < GameManager.Data.crewmates.Count; i++)
            {
                CrewmateData data = GameManager.Data.crewmates[i];
                GameObject crewmate = Instantiate(crewmatePrefab, transform);
                Crewmate mate = crewmate.GetComponent<Crewmate>();
                mate.crewmateName = data.name;
                mate.icon = data.icon;
                mate.buildingID = data.buildingID;

                if(data.buildingID != -1)
                {
                    // Reassign crewmate
                    //foreach (Building building in buildings)
                    //{
                    //    if (mate.buildingID == building.id)
                    //    {
                    //        building.builder = mate;
                    //        return;
                    //    }
                    //}
                }

                Vector3 spawnPosition = Random.insideUnitCircle * crewmateSpawnRadius;
                mate.transform.position = crewmateSpawn.position + spawnPosition;
                crewmates.Add(mate);
            }
        }
    }
    private void Start()
    {
        //omui.FillCrewmateUI(crewmates.ToArray());
    }

    internal void SpawnNewCrewmate()
    {
        GameObject crewmateObj = Instantiate(crewmatePrefab, transform);
        Crewmate crewmate = crewmateObj.GetComponent<Crewmate>();

        // Position
        Vector2 circleLocation = Random.insideUnitCircle;
        Vector3 spawnPosition = new Vector3(circleLocation.x * crewmateSpawnRadius, 0, circleLocation.y * crewmateSpawnRadius);
        crewmateObj.transform.position = crewmateSpawn.position + spawnPosition;
        // Save Data
        CrewmateData data = new CrewmateData();
        data.icon = crewmate.Icon;
        data.name = crewmate.Name;
        data.buildingID = crewmate.buildingID;
        GameManager.Data.crewmates.Add(data);
        crewmates.Add(crewmate);
        // Update UI
        crewmate.cardIndex = GameManager.Data.crewmates.Count - 1; // check
        crewmate.onSelect.AddListener(() => { SelectionCallback(crewmate); });

        // Add card
        omui.AddCrewmateCard(crewmate);
    }
    internal Crewmate SelectCrewmate(int index)
    {
        ClickSelect(crewmates[index].gameObject);
        return crewmates[index];
    }
    internal void ClickSelect(GameObject unitToAdd)
    {
        DeselectAll();
        AddSelection(unitToAdd);
    }
    internal void DeselectAll()
    {
        foreach(var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
    }
    private void AddSelection(GameObject unitToAdd)
    {
        if(unitToAdd.GetComponent<Crewmate>().IsBuilding)
        {
            return;
        }

        unitsSelected.Add(unitToAdd);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
    }
    internal void RemoveSelection(GameObject unitToRemove)
    {
        unitsSelected.Remove(unitToRemove);
        //sets the first child to be active: an indicator showing that the unit is selected
        unitToRemove.transform.GetChild(0).gameObject.SetActive(false);
    }
    // TODO only pass in index somehow
    private void SelectionCallback(Crewmate mate)
    {
        ClickSelect(mate.gameObject);
        omui.SelectCrewmateCard(mate.cardIndex); // will likely break when crewmates die. Will need to have another method
    }
}
