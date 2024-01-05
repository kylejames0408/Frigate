using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrewmateManager : MonoBehaviour
{
    // Will likely have crewmate state data
    [SerializeField] private Sprite iconEmptyAsssignment;
    [SerializeField] private Sprite[] crewmateIcons;
    // UI
    [SerializeField] private OutpostManagementUI omui;
    [SerializeField] private CombatManagementUI cmui;
    [SerializeField] private CrewmateUI crewmateUI;
    // Components
    [SerializeField] private ResourceManager rm;
    [SerializeField] private BuildingManager bm;
    // Crewmate Data
    [SerializeField] private GameObject crewmatePrefab;
    [SerializeField] private Transform crewmateSpawn;
    [SerializeField] private int crewmateSpawnRadius = 10;
    // Tracking
    [SerializeField] private Dictionary<int, Crewmate> crewmates;
    [SerializeField] private List<int> selectedCrewmateIDs;
    private int shipID;
    // Combat
    [SerializeField] private bool eventTriggered = false;
    [SerializeField] private Material[] materials;
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Zone[] zones;
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
    public Crewmate[] Crewmates
    {
        get { return crewmates.Values.ToArray(); }
    }

    private void Awake()
    {
        if (omui == null) { omui = FindObjectOfType<OutpostManagementUI>(); }
        if (cmui == null) { cmui = FindObjectOfType<CombatManagementUI>(); }
        if (crewmateUI == null) { crewmateUI = FindObjectOfType<CrewmateUI>(); }
        if (rm == null) { rm = FindObjectOfType<ResourceManager>(); }
        if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }

        // Init Crewmates (make own function)
        crewmateSpawn = transform.GetChild(0);
        crewmates = new Dictionary<int, Crewmate>();
        if(GameManager.Phase == GamePhase.PLUNDERING)
        {
            enemies = FindObjectsOfType<Enemy>().ToList();
        }
        selectedCrewmateIDs = new List<int>();
    }
    private void Start()
    {
        crewmateUI.onClose.AddListener(OnCloseIbterfaceCallback);
        crewmateUI.onClickCrewmate.AddListener(OnClickCrewmateIconCallback);
        crewmateUI.onClickLocation.AddListener(OnClickBuildingIconCallback);

        if (GameManager.Phase == GamePhase.BUILDING)
        {
            ShipData shipData = PlayerDataManager.LoadShipData();
            if (shipData.isInitialized) // First appearance
            {
                shipID = shipData.id;
                SpawnExistingCrewmates(shipData.crew);

                OutpostData outpostData = PlayerDataManager.LoadOutpostData();
                if (outpostData.crew.Length == 0)
                {
                    SpawnNewCrewmates(outpostData.crewCapacity);
                }
                else
                {
                    SpawnExistingCrewmates(outpostData.crew);
                }
            }
            else
            {
                shipID = FindObjectOfType<Ship>().ID;
                SpawnNewCrewmates(PlayerDataManager.STARTING_CREW_AMOUNT);
            }
        }
    }

    private void Update()
    {
        // TODO: make ifs into handler functions?

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
            if(crewMember)
            {
                crewMember.lineRenderer.SetPosition(0, crewMember.transform.position);
            }
        }

        // Move Crewmate - TODO: move to function
        //if(isCombat && Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = CameraManager.Instance.Camera.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit, 500f, mask, QueryTriggerInteraction.Ignore))
        //    {
        //        //Moves units to the center of zone when it is clicked on
        //        if (hit.collider.tag == "Zone")
        //        {
        //            Zone zone = hit.transform.gameObject.GetComponent<Zone>();
        //            foreach (int id in selectedCrewmateIDs)
        //            {
        //                Vector3 movePos = (zone.zoneCenter + (Vector3)UnityEngine.Random.insideUnitSphere * 7f);
        //                Vector3 updatedMovePos = new Vector3(movePos.x, 0, movePos.z);
        //
        //                //makes crewmates move to a random position within a sphere around the center of the zone
        //                crewmates[id].SetDestination(updatedMovePos);
        //
        //                //creates a line to indicate where the units are moving to
        //                CrewMember crewMember = crewmates[id].GetComponent<CrewMember>();
        //                crewMember.targetPos = updatedMovePos;
        //                ShowLineRenderer(updatedMovePos, id);
        //
        //                //updates "BOTH" enum states and updates card status in combat
        //                crewMember.characterState = Character.State.Moving;
        //                crewmates[id].State = CrewmateState.MOVING;
        //                cmui.UpdateCard(id, crewmates[id].StateIcon);
        //            }
        //        }
        //        else
        //        {
        //            foreach (int id in selectedCrewmateIDs)
        //            {
        //                crewmates[id].SetDestination(hit.point);
        //
        //                CrewMember crewMember = crewmates[id].GetComponent<CrewMember>();
        //                crewMember.targetPos = hit.point;
        //
        //                //updates "BOTH" enum states and updates card status in combat
        //                crewMember.characterState = Character.State.Moving;
        //                crewmates[id].State = CrewmateState.MOVING;
        //                cmui.UpdateCard(id, crewmates[id].StateIcon);
        //            }
        //        }
        //    }
        //}
    }
    private void OnDestroy()
    {
        if (GameManager.Phase == GamePhase.BUILDING)
        {
            PlayerDataManager.SaveOutpostCrewmates(crewmates.Values.ToArray());
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
    private void SpawnExistingCrewmates(CrewmateData[] crewmateData)
    {
        // Spawn existing crewmates
        for (int i = 0; i < crewmateData.Length; i++)
        {
            GameObject crewmateObj = Instantiate(crewmatePrefab, transform);
            Crewmate mate = crewmateObj.GetComponent<Crewmate>();
            mate.Set(crewmateData[i]);

            // Set callbacks
            mate.onSelect.AddListener(() => { ClickCrewmate(mate.ID); });
            mate.onAssign.AddListener(() => { OnAssignCallback(mate.ID); });
            mate.onReassign.AddListener(() => { OnReassignCallback(mate.ID); });
            mate.onDestroy.AddListener(() => { DeselectCard(mate.ID); });

            // If they are not assigned - move to random pos around spawn
            if (crewmateData[i].building.id == -1)
            {
                Vector2 circleLocation = Random.insideUnitCircle;
                Vector3 spawnPosition = new Vector3(circleLocation.x * crewmateSpawnRadius, 0, circleLocation.y * crewmateSpawnRadius);
                crewmateObj.transform.position = crewmateSpawn.position + spawnPosition;
            }

            // Tracking
            if (!crewmates.ContainsKey(mate.ID))
            {
                crewmates.Add(mate.ID, mate);
            }

            // Add card
            AddCard(mate);
        }
    }
    // used for dev
    internal void SpawnNewCrewmate()
    {
        GameObject crewmateObj = Instantiate(crewmatePrefab, transform);

        // Set Position - maybe move to util func since same thing is in Building
        Vector2 circleLocation = Random.insideUnitCircle;
        Vector3 spawnPosition = new Vector3(circleLocation.x * crewmateSpawnRadius, 0, circleLocation.y * crewmateSpawnRadius);
        crewmateObj.transform.position = crewmateSpawn.position + spawnPosition;

        Crewmate mate = crewmateObj.GetComponent<Crewmate>();
        crewmateObj.name = mate.FirstName + mate.ID;

        mate.SetUI(crewmateIcons[Random.Range(0, crewmateIcons.Length)], iconEmptyAsssignment);

        // Set callbacks
        mate.onSelect.AddListener(() => { ClickCrewmate(mate.ID); });
        mate.onAssign.AddListener(() => { OnAssignCallback(mate.ID); }); // dont need for combat
        mate.onReassign.AddListener(() => { OnReassignCallback(mate.ID); }); // dont need for combat
        mate.onDestroy.AddListener(() => { DeselectCard(mate.ID); });

        // Tracking
        crewmates.Add(mate.ID, mate);
        rm.SpawnCrewmate(crewmates.Count);

        // Update UI
        AddCard(mate);

    }
    private void SpawnNewCrewmates(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnNewCrewmate();
        }
    }
    private void RemoveCrewmate(int crewmateID)
    {
        DeselectCrewmateShare(crewmateID);
        Destroy(crewmates[crewmateID].gameObject);
        crewmates.Remove(crewmateID);
        rm.RemoveCrewmate(crewmates.Count);
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
        //crewmates[crewmateID].transform.GetChild(0).gameObject.SetActive(true);

        if (!selectedCrewmateIDs.Contains(crewmateID))
        {
            crewmates[crewmateID].GetComponent<Renderer>().sharedMaterial = materials[1];
            selectedCrewmateIDs.Add(crewmateID);
        }

        if (selectedCrewmateIDs.Count > 1)
        {
            crewmateUI.CloseInterface(); // maybe check if is already closed
        }
        else
        {
            crewmateUI.FillAndOpenInterface(crewmates[crewmateID]);
        }

        if (GameManager.Phase == GamePhase.PLUNDERING)
        {
            //unit line renderer
            CrewMember crewMember = crewmates[crewmateID].GetComponent<CrewMember>();
            ShowLineRenderer(crewMember.targetPos, crewmateID);
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
        if (cmui == null)
        {
            omui.SelectCrewmateCard(cardID);
        }
        else
        {
            cmui.SelectCrewmateCard(cardID);
        }
    }
    private void AddCard(Crewmate mate)
    {
        if (cmui == null)
        {
            omui.AddCrewmateCard(mate);
        }
        else
        {
            cmui.AddCrewmateCard(mate);
        }
    }
    private void DeselectCard(int cardID)
    {
        if (cmui == null)
        {
            omui.DeselectCrewmateCard(cardID);
        }
        else
        {
            cmui.DeselectCrewmateCard(cardID);
        }
    }
    private void DeselectAllCards()
    {
        if (cmui == null)
        {
            omui.DeselectAllCrewmateCards();
        }
        else
        {
            cmui.DeselectAllCrewmateCards();
        }
    }
    private void RemoveCard(int cardID)
    {
        if (cmui == null)
        {
            omui.RemoveCrewmateCard(cardID);
        }
        else
        {
            cmui.RemoveCrewmateCard(cardID);
        }
    }
    internal void UnassignCrewmate(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        mate.Unassign(); // will UI need to be updated as well?
        if (cmui == null)
        {
            omui.UpdateCrewmateCard(crewmateID, mate.StateIcon);
        }
        else
        {
            cmui.UpdateCard(crewmateID, mate.StateIcon);
        }

    }

    // Callbacks
    private void OnAssignCallback(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        crewmateUI.FillAndOpenInterface(mate);
        if (cmui == null)
        {
            omui.UpdateCrewmateCard(crewmateID, mate.StateIcon);
        }
        else
        {
            cmui.UpdateCard(crewmateID, mate.StateIcon);
        }
    }
    private void OnReassignCallback(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        bm.UnassignBuilding(mate.Building.id, mate.ID);
    }
    private void OnCloseIbterfaceCallback()
    {
        DeselectAllCrewmatesShare();
    }
    internal void OnClickCrewmateIconCallback(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        CameraManager.Instance.PanTo(mate.transform.position);
    }
    private void OnClickBuildingIconCallback(int crewmateID)
    {
        Crewmate mate = crewmates[crewmateID];
        if(mate.IsAssigned)
        {
            if(mate.Building.id != shipID)
            {
                bm.OnClickBuildingIconCallback(mate.Building.id);
            }
            else
            {
                bm.PanToShip();
            }
        }

        if(GameManager.Phase == GamePhase.PLUNDERING)
        {
            //Pans the camera to the zone that the crewmate is currently in
            foreach(Zone zone in zones)
            {
                if(zone.crewMembersInZone.Contains(mate.gameObject))
                {
                    CameraManager.Instance.PanTo(zone.centerObject.transform.position);
                }
            }
        }
    }

    // Hides the crewmate's line renderer
    private void HideLineRenderer()
    {
        foreach (int id in selectedCrewmateIDs)
        {
            CrewMember crewMember = crewmates[id].GetComponent<CrewMember>();
            if(crewMember)
            {
                crewMember.lineRenderer.enabled = false;
            }
        }
    }

    /// <summary>
    /// Shows and updates unit's line renderer
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="crewMemberID"></param>
    private void ShowLineRenderer(Vector3 targetPos, int crewMemberID)
    {
        CrewMember crewMember = crewmates[crewMemberID].GetComponent<CrewMember>();

        crewMember.lineRenderer.enabled = true;

        crewMember.lineRenderer.SetPosition(0, crewMember.transform.position);
        crewMember.lineRenderer.SetPosition(1, targetPos);

    }

    internal void EditCrewmates(int amount)
    {
        if(amount > 0)
        {
            SpawnNewCrewmates(amount);
        }
        else
        {
            int lastKey = crewmates.Keys.ElementAt(crewmates.Keys.Count - 1);
            RemoveCrewmate(lastKey);
        }
    }
}
