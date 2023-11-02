using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public Dictionary<int, Crewmate> crewmates;
    public List<int> selectedCrewmateIDs;
    // Selection
    [SerializeField] private RectTransform selectionBoxTrans; // I am 99% sure we can just use the rectTrans. I dont know why it is not working however
    [SerializeField] private Rect selectionBoxRect;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isDragging = false;
    private bool isUIClicked = false;

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
        crewmates = new Dictionary<int, Crewmate>(); //GameManager.Data.crewmates.Count
        selectedCrewmateIDs = new List<int>();
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
                crewmates.Add(crewmate.id, crewmate);
                // Update UI
                crewmate.cardIndex = crewmates.Count - 1; // check
                crewmate.onSelect.AddListener(() => { ClickCrewmate(crewmate.cardIndex); });
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
    // TODO: make ifs into handler functions
    private void Update()
    {
        // Left mouse button PRESS handler
        if (Input.GetMouseButtonDown(0))
        {
            PressHandler();
        }

        // Left mouse button HOLD handler
        if (Input.GetMouseButton(0) && !isUIClicked)
        {
            HoldHandler();
        }

        // Left mouse button RELEASE handler - includes clicking
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseHandler();
        }
    }
    private void PressHandler()
    {
        // Checks if UI is clicked
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isUIClicked = true;
            return;
        }

        // Grab data to move to next step
        isUIClicked = false;
        startPosition = Input.mousePosition;
    }
    private void HoldHandler()
    {
        endPosition = Input.mousePosition;

        // Check for movement
        if (startPosition == endPosition)
        {
            return;
        }

        DraggingHandler();
    }
    private void DraggingHandler()
    {
        isDragging = true;

        // Update selection box UI
        Vector3 boxCenter = (startPosition + endPosition) * 0.5f;
        selectionBoxTrans.position = boxCenter;
        float width = Mathf.Abs(endPosition.x - startPosition.x);
        float height = Mathf.Abs(endPosition.y - startPosition.y);
        selectionBoxTrans.sizeDelta = new Vector2(width, height);
    }
    private void ReleaseHandler()
    {
        if (isDragging)
        {
            DragReleaseHandler();
        }
        // If not dragging yet and UI was not clicked, register click
        else if (!isUIClicked)
        {
            ClickReleaseHandler();
        }
    }
    private void DragReleaseHandler()
    {
        isDragging = false;
        if (selectionBoxTrans.sizeDelta == Vector2.zero)
        {
            return;
        }

        // Calculates screen space
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

        // Remove crewmates if left-control is HELD
        if (Input.GetKey(KeyCode.LeftControl))
        {
            foreach (Crewmate mate in crewmates.Values)
            {
                //if unit is within bounds of the selection rect
                if (selectionBoxRect.Contains(CameraManager.Instance.Camera.WorldToScreenPoint(mate.transform.position)))
                {
                    // Checks if it will be removing an already unselected crewmate
                    if (selectedCrewmateIDs.Contains(mate.id))
                    {
                        DeselectCrewmateShare(mate.id);
                    }
                }
            }
        }
        else
        {
            // Do not deselect if left-shift is HELD
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAllCrewmatesShare();
            }

            foreach (Crewmate mate in crewmates.Values)
            {
                //if unit is within bounds of the selection rect
                if (selectionBoxRect.Contains(CameraManager.Instance.Camera.WorldToScreenPoint(mate.transform.position)))
                {
                    // Checks if it will be adding an already selected crewmate
                    if (!selectedCrewmateIDs.Contains(mate.id))
                    {
                        ClickCrewmate(mate.id);
                    }
                }
            }
        }

        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        selectionBoxTrans.sizeDelta = endPosition;
    }
    private void ClickReleaseHandler()
    {
        bool crewmateWasClicked = false;
        // Click handle - select crewmate if clicked on
        foreach (Crewmate mate in crewmates.Values)
        {
            if (mate.isHovered)
            {
                crewmateWasClicked = true;

                // Deselect crewmate if left-control is HELD
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (selectedCrewmateIDs.Contains(mate.id))
                    {
                        DeselectCrewmateShare(mate.id);
                    }
                }
                else
                {
                    // Deselect all other crewmates if left-shift is not HELD
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAllCrewmatesShare();
                    }

                    if (!selectedCrewmateIDs.Contains(mate.id))
                    {
                        ClickCrewmate(mate.id);
                    }
                }
                break; // can only hover over one crewmate
            }
        }

        // If nothing was selected, deselect it
        if (!crewmateWasClicked)
        {
            DeselectAllCrewmatesShare();
        }
    }

    internal Crewmate GetCrewmate(int id)
    {
        return crewmates[id];
    }

    // Crewmate interactions
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
        data.id = crewmate.id;
        data.icon = crewmate.Icon;
        data.name = crewmate.Name;
        data.buildingID = crewmate.buildingID;
        GameManager.Data.crewmates.Add(data);
        crewmates.Add(crewmate.id, crewmate);

        // Update UI
        crewmate.cardIndex = crewmates.Count - 1; // check
        //crewmate.onSelect.AddListener(() => { ClickCrewmate(crewmate.id); });

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
    internal void RemoveCrewmate(int id)
    {
        DeselectCrewmateShare(id);
        GameManager.RemoveCrewmateData(id);
        crewmates.Remove(id);

        int cardIndex = crewmates[id].cardIndex;
        if (omui == null)
        {
            cmui.RemoveCrewmateCard(cardIndex);
        }
        else
        {
            omui.RemoveCrewmateCard(cardIndex);
        }
    }
    // Select the crewmate and update UI (share)
    private void ClickCrewmate(int id) // share
    {
        SelectCrewmate(id);
        int cardIndex = crewmates[id].cardIndex;
        if (omui == null)
        {
            cmui.SelectCrewmateCard(cardIndex);
        }
        else
        {
            omui.SelectCrewmateCard(cardIndex);
        }
    }
    internal void SelectCrewmate(int id)
    {
        crewmates[id].transform.GetChild(0).gameObject.SetActive(true);
        selectedCrewmateIDs.Add(id);
    }
    internal void DeselectCrewmate(int id)
    {
        crewmates[id].transform.GetChild(0).gameObject.SetActive(false);
        selectedCrewmateIDs.Remove(id);
    }
    private void DeselectCrewmateShare(int id)
    {
        DeselectCrewmate(id);
        int cardIndex = crewmates[id].cardIndex;
        if (omui == null)
        {
            cmui.DeselectCrewmateCard(cardIndex);
        }
        else
        {
            omui.DeselectCrewmateCard(cardIndex);
        }
    }
    internal void DeselectAllCrewmates()
    {
        for (int i = 0; i < selectedCrewmateIDs.Count; i++)
        {
            crewmates[selectedCrewmateIDs[i]].transform.GetChild(0).gameObject.SetActive(false);
        }
        selectedCrewmateIDs.Clear();
    }
    private void DeselectAllCrewmatesShare()
    {
        DeselectAllCrewmates();
        if (omui == null)
        {
            cmui.DeselectAllCrewmateCards();
        }
        else
        {
            omui.DeselectAllCrewmateCards();
        }
    }
}
