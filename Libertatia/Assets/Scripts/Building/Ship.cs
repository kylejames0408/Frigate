using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();
    public int enemyCount;

    public int detectionRange;
    private bool inRange;

    public CombatResourcesUI resourceUI;
    [SerializeField] private GameObject battleLootUI;
    [SerializeField] private GameObject leaveButton;

    // Components
    [SerializeField] private ResourcesUI rUI;
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private MeshRenderer buildingRender;
    // Tracking
    [SerializeField] private int id = -1;
    [SerializeField] private int capacity = 12;
    [SerializeField] private List<ObjectData> crewmates;
    [SerializeField] private int level = 0;
    // Characteristics
    [SerializeField] private string buildingName;
    [SerializeField] private float radius = 10.0f;
    // UI
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isHovered = false;
    // Emissions
    [SerializeField] private Color normalEmission = Color.black;
    [SerializeField] private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);

    private void Awake()
    {
        if(buildingRender == null) { buildingRender = GetComponentInChildren<MeshRenderer>(); }
        if (resourceUI == null) { resourceUI = FindObjectOfType<CombatResourcesUI>(); }
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rUI == null) { rUI = FindObjectOfType<ResourcesUI>(); }
        if (shipUI == null) { shipUI = FindObjectOfType<ShipUI>(); }

        id = gameObject.GetInstanceID();
        level = 0;
        isHovered = false;
    }
    void Start()
    {
        crewmates = new List<ObjectData>(capacity); // new ObjectData(-1, iconEmptyAsssignment)
        shipUI.Set(capacity);

        detectionRange = 25;
        inRange = false;

        unitList.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));
        enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject unit in unitList)
        {
            if (unit.activeSelf != false)
            {
                //If a unit is within range of the ship, the player can return to the outpost
                if (Vector3.Distance(transform.position, unit.transform.position) <= detectionRange)
                {
                    //Debug.Log("Go home");
                    inRange = true;

                    //sets the leave button to be active if within range
                    leaveButton.SetActive(true);

                }
                else
                {
                    inRange = false;
                    leaveButton.SetActive(false);
                }

            }

        }

        //if (inRange)
        //{
        //    //sets the leave button to be active if within range
        //    leaveButton.SetActive(true);
        //}
        //else
        //{
        //    leaveButton.SetActive(false);
        //}

    }
    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            isHovered = true;
            buildingRender.material.SetColor("_EmissionColor", hoveredEmission);

            if (Input.GetMouseButtonDown(0))
            {
                shipUI.OpenMenu();

            }
            if (Input.GetMouseButtonDown(1))
            {
                if (cm.IsCrewmateSelected && crewmates.Count < crewmates.Capacity)
                {
                    // Get selected units
                    Crewmate[] selectedCrewmates = cm.GetSelectedCrewmates();
                    for (int i = 0; i < selectedCrewmates.Length; i++)
                    {
                        Crewmate mate = selectedCrewmates[i];

                        // Assign them to the building
                        crewmates.Add(new ObjectData(mate.ID, mate.Icon));

                        // Calculate target destination
                        mate.Assign(id, icon, GetDestination());

                        // Update UI
                        shipUI.AddCrewmate(crewmates.Count, new ObjectData(mate.ID, mate.Icon));
                    }
                }
                else
                {
                    Debug.Log("Ship assignments are full");
                }
            }
        }
    }
    private void OnMouseExit()
    {
        if (isHovered)
        {
            buildingRender.material.SetColor("_EmissionColor", normalEmission);
        }
        isHovered = false;
    }

    private Vector3 GetDestination()
    {
        // Get random position around unit circle
        Vector2 circlePosition = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 destinationOffset = new Vector3(circlePosition.x * radius, 0, Mathf.Abs(circlePosition.y) * radius); // keeps crewmate in-front of the building, maybe tweak
        return transform.position + destinationOffset;
    }
    public void OpenBattleLootUI()
    {
        //GameManager.data.resources.wood += 50;
        //GameManager.data.resources.doubloons += 10;
        //GameManager.data.resources.food += 100;

        GameManager.data.resources.wood += resourceUI.woodAmount;
        GameManager.data.resources.doubloons += resourceUI.doubloonAmount;
        GameManager.data.resources.food += resourceUI.foodAmount;

        //Opens the battle loot ui
        battleLootUI.SetActive(true);
        //CeneManager.LoadOutpostFromCombat();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    private void OnGUI()
    {
        // the rect that is the canvas
        //GameObject canvas = GameObject.Find("BoxSelectCanvas");
        //RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        //// the style used to set the text size and
        //GUIStyle GUIBoxStyle = new GUIStyle(GUI.skin.box);
        //GUIBoxStyle.fontSize = (int)(canvasRect.rect.height * 0.023f);
        //GUIBoxStyle.alignment = TextAnchor.MiddleCenter;

        //if (inRange)
        //{
        //    GUI.Box(new Rect(canvasRect.rect.width * 0.35f, canvasRect.rect.height * 0.05f, canvasRect.rect.width * 0.21f, canvasRect.rect.height * 0.05f),
        //        "Press E to return to outpost.", GUIBoxStyle);
        //}

    }
}
