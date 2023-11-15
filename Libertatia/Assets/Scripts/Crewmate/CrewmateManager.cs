using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class CrewmateManager : MonoBehaviour
{
    // Will likely have crewmate state data
    [SerializeField] private Sprite iconEmptyAsssignment;
    [SerializeField] private Sprite[] crewmateIcons;
    // Components
    [SerializeField] private OutpostManagementUI omui;
    [SerializeField] private CombatManagementUI cmui;
    [SerializeField] private ResourcesUI orui;
    [SerializeField] private CombatResourcesUI crui;
    [SerializeField] private CrewmateUI crewmateUI;
    [SerializeField] private BuildingManager bm;
    // Crewmate Data
    [SerializeField] private GameObject crewmatePrefab;
    [SerializeField] private Transform crewmateSpawn;
    [SerializeField] private int crewmateSpawnRadius = 10;
    [SerializeField] private const int CREWMATE_FOOD_CONSUMPTION = 10;
    // Tracking
    [SerializeField] private Dictionary<int, Crewmate> crewmates;
    [SerializeField] private List<int> selectedCrewmateIDs;
    // Combat
    [SerializeField] private bool isCombat = false;
    [SerializeField] private bool eventTriggered = false;
    [SerializeField] private Material[] materials;
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private LayerMask mask;
    // Selection
    [SerializeField] private RectTransform selectionBoxTrans; // I am 99% sure we can just use the rectTrans. I dont know why it is not working however
    private Rect selectionBoxRect;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isDragging = false;
    private bool isUIClicked = false;

    public bool IsCrewmateSelected
    {
        get { return selectedCrewmateIDs.Count > 0; }
    }
    public List<Enemy> Enemies
    {
        get { return enemies; }
    }
    public List<Crewmate> Crewmates
    {
        get { return crewmates.Values.ToList(); }
    }

    private void Awake()
    {
        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }
        if (cmui == null) { cmui = FindObjectOfType<CombatManagementUI>(); }
        if (orui == null) { orui = FindObjectOfType<ResourcesUI>(); }
        if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }

        // Init Crewmates (make own function)
        crewmateSpawn = transform.GetChild(0);
        crewmates = new Dictionary<int, Crewmate>();
        if(isCombat)
        {
            enemies = FindObjectsOfType<Enemy>().ToList();
        }
        selectedCrewmateIDs = new List<int>();

        // Can probably move this to start - similar to building manager
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
            // Spawn existing crewmates
            for (int i = 0; i < GameManager.Data.crewmates.Count; i++)
            {
                SpawnExistingCrewmate(GameManager.Data.crewmates[i]);
            }
        }
    }
    private void Start()
    {
        // Update UI
        if (orui != null)
        {
            orui.Init(); // inits here since crewmate manager inits in both outpost and combat scene
            orui.UpdateFoodUI(GameManager.Data.resources);
        }
    }
    private void Update()
    {
        // TODO: make ifs into handler functions

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

        //constantly updates the movement line
        foreach (int id in selectedCrewmateIDs)
        {
            CrewMember crewMember = crewmates[id].GetComponent<CrewMember>();

            crewMember.lineRenderer.SetPosition(0, crewMember.transform.position);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            HideLineRenderer();
        }

        // Move Crewmate - TODO: move to function
        if(isCombat && Input.GetMouseButtonDown(1))
        {
            Ray ray = CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f, mask, QueryTriggerInteraction.Ignore))
            {
                //Moves units to the center of zone when it is clicked on
                if (hit.collider.tag == "Zone")
                {
                    Zone zone = hit.transform.gameObject.GetComponent<Zone>();
                    foreach (int id in selectedCrewmateIDs)
                    {
                        Vector3 movePos = (zone.zoneCenter + (Vector3)UnityEngine.Random.insideUnitSphere * 7f);

                        //makes crewmates move to a random position within a sphere around the center of the zone
                        crewmates[id].SetDestination(movePos);

                        //creates a line to indicate where the units are moving to
                        CrewMember crewMember = crewmates[id].GetComponent<CrewMember>();
                        crewMember.lineRenderer.enabled = true;
                        crewMember.lineRenderer.SetPosition(0,crewMember.transform.position);

                        Vector3 updatedMovePos = new Vector3(movePos.x, 0, movePos.z);
                        crewMember.lineRenderer.SetPosition(1, updatedMovePos);
                    }
                }
                else
                {
                    foreach (int id in selectedCrewmateIDs)
                    {
                        crewmates[id].SetDestination(hit.point);
                    }
                }
            }
        }
    }

    // Handlers
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
                    if (selectedCrewmateIDs.Contains(mate.ID))
                    {
                        DeselectCrewmateShare(mate.ID);
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
                    if (!selectedCrewmateIDs.Contains(mate.ID))
                    {
                        ClickCrewmate(mate.ID);
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
            if (mate.IsHovered)
            {
                crewmateWasClicked = true;

                // Deselect crewmate if left-control is HELD
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (selectedCrewmateIDs.Contains(mate.ID))
                    {
                        DeselectCrewmateShare(mate.ID);
                    }
                }
                else
                {
                    // Deselect all other crewmates if left-shift is not HELD
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAllCrewmatesShare();
                    }

                    if (!selectedCrewmateIDs.Contains(mate.ID))
                    {
                        ClickCrewmate(mate.ID);
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

    // Utility
    internal Crewmate GetCrewmate(int crewmateID)
    {
        return crewmates[crewmateID];
    }
    internal Crewmate[] GetSelectedCrewmates()
    {
        List<Crewmate> selectedCrewmates = new List<Crewmate>(selectedCrewmateIDs.Count);
        foreach (int id in selectedCrewmateIDs)
        {
            selectedCrewmates.Add(crewmates[id]);
        }
        return selectedCrewmates.ToArray();
    }

    // Interactions
    internal void SpawnNewCrewmate()
    {
        GameObject crewmateObj = Instantiate(crewmatePrefab, transform);

        // Set Position - maybe move to util func since same thing is in Building
        Vector2 circleLocation = UnityEngine.Random.insideUnitCircle;
        Vector3 spawnPosition = new Vector3(circleLocation.x * crewmateSpawnRadius, 0, circleLocation.y * crewmateSpawnRadius);
        crewmateObj.transform.position = crewmateSpawn.position + spawnPosition;

        Crewmate mate = crewmateObj.GetComponent<Crewmate>();
        crewmateObj.name = mate.Name + mate.ID;

        mate.SetUI(crewmateIcons[UnityEngine.Random.Range(0, crewmateIcons.Length)], iconEmptyAsssignment);

        // Save Data
        GameManager.AddCrewmate(new CrewmateData(mate));

        // Set callbacks
        mate.onSelect.AddListener(() => { ClickCrewmate(mate.ID); });
        mate.onAssign.AddListener(() => { OnAssignCallback(mate.ID); }); // dont need for combat
        mate.onReassign.AddListener(() => { OnReassignCallback(mate.ID); }); // dont need for combat
        mate.onDestroy.AddListener(() => { DeselectCard(mate.ID); });

        // Tracking
        crewmates.Add(mate.ID, mate);

        // Update UI
        AddCard(mate);
        if (!isCombat)
        {
            GameManager.data.resources.foodConsumption += CREWMATE_FOOD_CONSUMPTION; // will need to make functions for this sometime
            orui.UpdateFoodUI(GameManager.Data.resources); // including this
        }
    }
    private void SpawnExistingCrewmate(CrewmateData data)
    {
        GameObject crewmateObj = Instantiate(crewmatePrefab, transform);
        Crewmate mate = crewmateObj.GetComponent<Crewmate>();
        mate.Init(data, iconEmptyAsssignment);

        if (data.building.id == -1)
        {
            // Set Position - this actually makes no sense
            Vector2 circleLocation = UnityEngine.Random.insideUnitCircle;
            Vector3 spawnPosition = new Vector3(circleLocation.x * crewmateSpawnRadius, 0, circleLocation.y * crewmateSpawnRadius);
            crewmateObj.transform.position = crewmateSpawn.position + spawnPosition;
        }

        // Set callbacks
        mate.onSelect.AddListener(() => { ClickCrewmate(mate.ID); });
        mate.onAssign.AddListener(() => { OnAssignCallback(mate.ID); });
        mate.onReassign.AddListener(() => { OnReassignCallback(mate.ID); });
        mate.onDestroy.AddListener(() => { DeselectCard(mate.ID); });

        // Tracking
        crewmates.Add(mate.ID, mate);

        // Add card
        AddCard(mate);
    }
    internal void RemoveCrewmate(int crewmateID)
    {
        DeselectCrewmateShare(crewmateID);
        GameManager.RemoveCrewmateData(crewmateID);
        crewmates.Remove(crewmateID);
        RemoveCard(crewmateID);
    }
    // Select the crewmate and update UI (share) // rename or create wrapper for callback
    private void ClickCrewmate(int crewmateID) // share
    {
        SelectCrewmate(crewmateID);
        SelectCard(crewmateID);
    }
    internal void SelectCrewmate(int crewmateID)
    {
        crewmates[crewmateID].GetComponent<Renderer>().sharedMaterial = materials[1];
        //crewmates[crewmateID].transform.GetChild(0).gameObject.SetActive(true);
        selectedCrewmateIDs.Add(crewmateID);

        if(!isCombat)
        {
            if (selectedCrewmateIDs.Count > 1)
            {
                crewmateUI.CloseMenu();
            }
            else
            {
                OpenSlider(crewmates[crewmateID]);
            }
        }
    }
    internal void DeselectCrewmate(int crewmateID)
    {
        HideLineRenderer();

        crewmates[crewmateID].GetComponent<Renderer>().sharedMaterial = materials[0];
        crewmates[crewmateID].transform.GetChild(0).gameObject.SetActive(false);
        selectedCrewmateIDs.Remove(crewmateID);
    }
    private void DeselectCrewmateShare(int crewmateID)
    {
        HideLineRenderer();

        DeselectCrewmate(crewmateID);
        DeselectCard(crewmateID);
    }
    internal void DeselectAllCrewmates()
    {
        for (int i = 0; i < selectedCrewmateIDs.Count; i++)
        {
            crewmates[selectedCrewmateIDs[i]].transform.GetChild(0).gameObject.SetActive(false);
            crewmates[selectedCrewmateIDs[i]].GetComponent<Renderer>().sharedMaterial = materials[0];
        }

        HideLineRenderer();

        selectedCrewmateIDs.Clear();
    }
    internal void DeselectAllCrewmatesShare()
    {

        HideLineRenderer();

        DeselectAllCrewmates();
        DeselectAllCards();

    }

    // UI
    // Manager
    private void SelectCard(int cardID)
    {
        if (omui == null)
        {
            cmui.SelectCrewmateCard(cardID);
        }
        else
        {
            omui.SelectCrewmateCard(cardID);
        }
    }
    private void AddCard(Crewmate mate)
    {
        if (omui == null)
        {
            cmui.AddCrewmateCard(mate);
        }
        else
        {
            omui.AddCrewmateCard(mate);
        }
    }
    private void DeselectCard(int cardID)
    {
        if (omui == null)
        {
            cmui.DeselectCrewmateCard(cardID);
        }
        else
        {
            omui.DeselectCrewmateCard(cardID);
        }
    }
    private void DeselectAllCards()
    {
        if (omui == null)
        {
            cmui.DeselectAllCrewmateCards();
        }
        else
        {
            omui.DeselectAllCrewmateCards();
        }
    }
    private void RemoveCard(int cardID)
    {
        if (omui == null)
        {
            cmui.RemoveCrewmateCard(cardID);
        }
        else
        {
            omui.RemoveCrewmateCard(cardID);
        }
    }
    // Slide
    private void OpenSlider(Crewmate mate)
    {
        crewmateUI.FillUI(mate);
        crewmateUI.OpenMenu();
    }
    internal void FreeAssignees(int assignee1ID, int assignee2ID)
    {
        if(assignee1ID != -1)
        {
            Crewmate mate1 = crewmates[assignee1ID];
            mate1.Free(); // will UI need to be updated as well?
        }
        if (assignee2ID != -1)
        {
            Crewmate mate2 = crewmates[assignee2ID];
            mate2.Free(); // will UI need to be updated as well?
        }
    }

    // Callbacks
    private void OnAssignCallback(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        OpenSlider(mate);
    }
    private void OnReassignCallback(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        bm.UnassignBuilding(mate.Building.id, mate.ID);
    }

    /// <summary>
    /// Hides the crewmate's line renderer
    /// </summary>
    private void HideLineRenderer()
    {
        foreach (int id in selectedCrewmateIDs)
        {
            CrewMember crewMember = crewmates[id].GetComponent<CrewMember>();

            crewMember.lineRenderer.enabled = false;
        }
    }
}
