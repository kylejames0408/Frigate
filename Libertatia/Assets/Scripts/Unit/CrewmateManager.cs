using System;
using System.Collections.Generic;
using UnityEngine;

public class CrewmateManager : MonoBehaviour
{
    private static CrewmateManager instance;
    // Components
    private OutpostManagementUI omui;
    private CombatManagementUI cmui;
    private ResourcesUI rui;
    // Crewmate Data
    [SerializeField] private GameObject crewmatePrefab;
    [SerializeField] private Transform crewmateSpawn;
    public int crewmateSpawnRadius = 10;
    // Tracking
    public List<Crewmate> crewmates;
    public List<Crewmate> selectedCrewmates;
    // Selection
    [SerializeField] RectTransform selectionBoxTrans;
    private Rect selectionBoxRect;
    private Vector2 startPosition;
    private Vector2 endPosition;

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

        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }
        if (cmui == null) { cmui = FindObjectOfType<CombatManagementUI>(); }
        if (rui == null) { rui = FindObjectOfType<ResourcesUI>(); }

        // Init Crewmates (make own function)
        crewmateSpawn = transform.GetChild(0);
        crewmates = new List<Crewmate>(); //GameManager.Data.crewmates.Count
        selectedCrewmates = new List<Crewmate>();
        if (GameManager.Data.crewmates == null)
        {
            Debug.Log("You might need to add the game manager to the scene; likely through PlayerData scene");
            return;
        }
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
                //GameObject crewmateObj = Instantiate(crewmatePrefab, transform);
                Vector3 position = new Vector3(-5 - 5, 0) + new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * 5, -5, UnityEngine.Random.Range(-1.0f, 1.0f) * 5);
                //Debug.Log(position); // this actually makes no sense
                GameObject crewmateObj = Instantiate(crewmatePrefab, position, Quaternion.identity);

                Crewmate crewmate = crewmateObj.GetComponent<Crewmate>();
                crewmate.crewmateName = data.name;
                crewmate.icon = data.icon;
                crewmate.buildingID = data.buildingID;

                // they will be freed anyways, however might be worth spawning them near the front of the building
                if(data.buildingID != -1)
                {
                    // Reassign crewmate to building
                    //foreach (Building building in buildings)
                    //{
                    //    if (mate.buildingID == building.id)
                    //    {
                    //        building.builder = mate;
                    //        return;
                    //    }
                    //}
                }

                Vector2 circleLocation = UnityEngine.Random.insideUnitCircle;
                Vector3 spawnPosition = new Vector3(circleLocation.x * crewmateSpawnRadius, 0, circleLocation.y * crewmateSpawnRadius);
                crewmateObj.transform.position = crewmateSpawn.position + spawnPosition;
                crewmates.Add(crewmate);
                // Update UI
                crewmate.cardIndex = crewmates.Count - 1; // check
                crewmate.onSelect.AddListener(() => { SelectionCallback(crewmate); });
                // Add card
                if(omui == null)
                {
                    //cmui.AddCrewmateCard(crewmate);
                }
                else
                {
                    omui.AddCrewmateCard(crewmate);
                }

            }
        }
    }

    private void Start()
    {
        rui.Init(); // inits here since crewmate manager inits in both outpost and combat scene
        // Update UI
        if (rui != null)
        {
            rui.UpdateFoodUI(GameManager.Data.resources);
        }
    }
    private void Update()
    {
        //when clicked
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }

        //when dragging
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;

            Vector3 boxCenter = (startPosition + endPosition) * 0.5f;
            Vector2 boxSize = new Vector2(Mathf.Abs(endPosition.x - startPosition.x), Mathf.Abs(endPosition.y - startPosition.y));
            selectionBoxTrans.position = boxCenter;
            selectionBoxTrans.sizeDelta = boxSize;
        }

        // when released
        if (Input.GetMouseButtonUp(0))
        {
            // Calculates screen sp
            if (endPosition.x < startPosition.x)
            {
                selectionBoxRect.xMin = endPosition.x;
                selectionBoxRect.xMax = startPosition.x;
            }
            else
            {
                selectionBoxRect.xMin = startPosition.x;
                selectionBoxRect.xMax = endPosition.x;
            }

            if (endPosition.y < startPosition.y)
            {
                selectionBoxRect.yMin = endPosition.y;
                selectionBoxRect.yMax = startPosition.y;
            }
            else
            {
                selectionBoxRect.yMin = startPosition.y;
                selectionBoxRect.yMax = endPosition.y;
            }
            DeselectAll();
            foreach (Crewmate mate in crewmates)
            {
                //if unit is within bounds of the selection rect
                if (selectionBoxRect.Contains(CameraManager.Instance.Camera.WorldToScreenPoint(mate.transform.position)))
                {
                    if (!selectedCrewmates.Contains(mate))
                    {
                        AddSelection(mate);
                    }
                }
            }
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            selectionBoxTrans.sizeDelta = endPosition;
        }
    }

    internal void SpawnNewCrewmate()
    {
        GameObject crewmateObj = Instantiate(crewmatePrefab, transform);
        Crewmate crewmate = crewmateObj.GetComponent<Crewmate>();
        crewmateObj.name = crewmate.Name + crewmate.id;

        // Position
        Vector2 circleLocation = UnityEngine.Random.insideUnitCircle;
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
        crewmate.cardIndex = crewmates.Count - 1; // check
        crewmate.onSelect.AddListener(() => { SelectionCallback(crewmate); });

        // Add card
        if (omui == null)
        {
            cmui.AddCrewmateCard(crewmate);
        }
        else
        {
            omui.AddCrewmateCard(crewmate);
        }

        // Update UI
        GameManager.data.resources.foodConsumption += 10; // crewmate food consumption
        rui.UpdateFoodUI(GameManager.Data.resources);
    }
    internal Crewmate SelectCrewmate(int index)
    {
        ClickSelect(crewmates[index]);
        return crewmates[index];
    }
    internal void ClickSelect(Crewmate mate)
    {
        DeselectAll();
        AddSelection(mate);
    }
    internal void DeselectAll()
    {
        foreach(Crewmate mate in selectedCrewmates)
        {
            mate.transform.GetChild(0).gameObject.SetActive(false);
        }
        selectedCrewmates.Clear();
    }
    private void AddSelection(Crewmate mate)
    {
        if (mate.IsBuilding)
        {
            return;
        }

        selectedCrewmates.Add(mate);
        mate.transform.GetChild(0).gameObject.SetActive(true);
    }
    internal void RemoveSelection(Crewmate mate)
    {
        selectedCrewmates.Remove(mate);
        mate.transform.GetChild(0).gameObject.SetActive(false);
    }
    // TODO only pass in index somehow
    private void SelectionCallback(Crewmate mate)
    {
        ClickSelect(mate);
        if (omui == null)
        {
            cmui.SelectCrewmateCard(mate.cardIndex);
        }
        else
        {
            omui.SelectCrewmateCard(mate.cardIndex); // will likely break when crewmates die. Will need to have another method
        }
    }
}
