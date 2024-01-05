using DG.Tweening;
using EasyDragAndDrop.Core;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutpostManagementUI : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private BuildingManager bm;
    [SerializeField] private Ship ship;
    // popup UI - maybe separate
    [SerializeField] private GameObject popupUI;
    [SerializeField] private TextMeshProUGUI buildingResourceCost;
    [SerializeField] private TextMeshProUGUI buildingAPCost;
    [SerializeField] private TextMeshProUGUI buildingProduction;
    [SerializeField] private float animTimeHoverInterface = 0.1f;
    // Animation related
    [SerializeField] private float animTimeArrow = 0.5f;
    [SerializeField] private float animTimeInterface = 0.6f;
    // Prefabs
    [SerializeField] private GameObject buildingCardPrefab;
    [SerializeField] private GameObject crewmateCardPrefab;
    // Interface object references
    [SerializeField] private Button arrow;
    // Parent objects
    [SerializeField] private Transform tabUIParent;
    [SerializeField] private Transform pagesUIParent;
    // Data tracking
    private Tab[] tabs;
    private Transform[] pages;
    public List<BuildingCard> buildingCards; // make into dict
    public Dictionary<int, CrewmateCard> crewmateCards;
    private List<int> selectedCrewmateCardIDs; // Im thinking this could be a stack
    [SerializeField] private bool isOpen;
    // Events
    public UnityEvent<int> onBeginDraggingBuildingCard;
    public UnityEvent<int> onEndDraggingBuildingCard;

    private void Awake()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }

        pages = pagesUIParent.GetComponentsInChildren<Transform>();
        if (pages.Length > 1)
        {
            for (int i = 1; i < pages.Length; i++)
            {
                pages[i - 1] = pages[i];
            }
        }
        tabs = tabUIParent.GetComponentsInChildren<Tab>();
        crewmateCards = new Dictionary<int, CrewmateCard>();
        selectedCrewmateCardIDs = new List<int>();
        popupUI.GetComponent<CanvasGroup>().alpha = 0;
    }
    private void Start()
    {
        // Sets tab triggers
        for (int i = 0; i < tabs.Length; i++)
        {
            int tabIndex = i; // needs to be destroyed after setting listener
            tabs[i].GetComponent<Button>().onClick.AddListener(() => { SelectTabCallback(tabIndex); });
        }

        // Init building UI as start tab
        SelectTab(0);

        // Sets arrow initial onclick callback
        if (isOpen)
        {
            arrow.onClick.AddListener(CloseInterface);
        }
        else
        {
            arrow.onClick.AddListener(OpenInterface);
        }
    }
    private void Update()
    {
        BuildingCardAvailabilityHandler();
    }

    // Interactions- General
    private void SelectTab(int tabIndex)
    {
        for (int i = 0; i < tabs.Length; i++) // assumes tabs and pages length are equal
        {
            if (i == tabIndex)
            {
                tabs[i].Select();
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                tabs[i].Deselect();
                pages[i].gameObject.SetActive(false);
            }
        }
    }
    internal void FillConstructionUI(Building[] buildings)
    {
        buildingCards = new List<BuildingCard>(buildings.Length);
        for (int i = 0; i < buildings.Length; i++)
        {
            AddBuildingCard(i, buildings[i]);
        }
    }
    // Interactions - Building Cards
    private void AddBuildingCard(int index, Building building)
    {
        GameObject cardObj = Instantiate(buildingCardPrefab, pages[0]);

        BuildingCard card = cardObj.GetComponentInChildren<BuildingCard>();
        card.Set(index, building.Cost, building.Production);
        card.onHover.AddListener(OnBeginHoverBuildingCardCallback);
        card.onHoverExit.AddListener(OnEndHoverBuildingCardCallback);
        buildingCards.Add(card);

        // Fill UI
        cardObj.GetComponentsInChildren<Image>()[2].sprite = building.Icon;
        cardObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Name;

        cardObj.GetComponentInChildren<DragObj2D>().onBeginDrag.AddListener(OnBeginDraggingBuildingCardCallback);
        cardObj.GetComponentInChildren<DragObj2D>().onEndDrag.AddListener(OnEndDraggingBuildingCardCallback);
    }
    internal void DeselectBuildingCard(int cardIndex)
    {
        buildingCards[cardIndex].Deselect();
    }

    // Interactions - Crewmate Cards
    internal void AddCrewmateCard(Crewmate mate)
    {
        GameObject cardObj = Instantiate(crewmateCardPrefab, pages[1]);

        CrewmateCard card = cardObj.GetComponentInChildren<CrewmateCard>();
        card.Set(mate);
        crewmateCards.Add(card.ID, card);

        // Callbacks
        card.GetComponentInChildren<Button>().onClick.AddListener(() => { ClickCrewmateCard(card.ID); }); // drag + drop func
        // Fill UI
        card.GetComponentsInChildren<Image>()[2].sprite = mate.Icon;
        card.GetComponentInChildren<TextMeshProUGUI>().text = mate.FirstName;

        cardObj.GetComponentInChildren<DragObj2D>().onBeginDrag.AddListener(OnBeginDraggingCrewmateCardCallback);
        cardObj.GetComponentInChildren<DragObj2D>().onEndDrag.AddListener(OnEndDraggingCrewmateCardCallback);
    }
    internal void RemoveCrewmateCard(int cardID)
    {
        DeselectCrewmateCard(cardID);
        Destroy(crewmateCards[cardID].transform.parent.gameObject); // needs to delete the parent object
        crewmateCards.Remove(cardID);
    }
    internal void UpdateCrewmateCard(int cardID, Sprite stateIcon)
    {
        CrewmateCard card = crewmateCards[cardID];
        card.SetStatus(stateIcon);
    }

    // Handlers - Crewmate Cards
    private void ClickCrewmateCard(int cardID) // share
    {
        // Ctrl: select & deselect
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (selectedCrewmateCardIDs.Contains(cardID))
            {
                DeselectCrewmateCardShare(cardID);
            }
            else
            {
                SelectCrewmateCardShare(cardID);
            }
        }
        // Shift: selects all inbetween
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && selectedCrewmateCardIDs.Count > 0)
        {
            List<int> ids = new List<int>(crewmateCards.Keys);
            int prevSelectedCardID = selectedCrewmateCardIDs[selectedCrewmateCardIDs.Count - 1];
            int firstSelectedIndex = -1;
            int secondSelectedIndex = -1;

            // Makes sure last selected is not itself
            if (cardID != prevSelectedCardID)
            {
                // Gets first and second selection indices
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i] == prevSelectedCardID)
                    {
                        firstSelectedIndex = i;
                    }
                    else if (ids[i] == cardID)
                    {
                        secondSelectedIndex = i;
                    }
                }

                // Orders selection from left to right and selects first card
                int leftIndex = -1;
                int rightIndex = -1;
                // click left then right
                if (firstSelectedIndex < secondSelectedIndex)
                {
                    leftIndex = firstSelectedIndex;
                    rightIndex = secondSelectedIndex;
                }
                // click right then left
                else
                {
                    rightIndex = firstSelectedIndex;
                    leftIndex = secondSelectedIndex;
                }

                // Selects all the cards inbetween
                for (int i = leftIndex; i <= rightIndex; i++)
                {
                    if (!selectedCrewmateCardIDs.Contains(ids[i]))
                    {
                        SelectCrewmateCardShare(ids[i]);
                    }
                }
            }
        }
        else
        {
            DeselectAllCrewmateCardsShare(); // might not want it to deselect if clicking on selected card
            SelectCrewmateCardShare(cardID);
        }
    }
    internal void SelectCrewmateCard(int cardID)
    {
        crewmateCards[cardID].Select();
        selectedCrewmateCardIDs.Add(cardID);
    }
    private void SelectCrewmateCardShare(int cardID)
    {
        if (!selectedCrewmateCardIDs.Contains(cardID))
        {
            SelectCrewmateCard(cardID);
            cm.SelectCrewmate(cardID);
        }
    }
    internal void DeselectCrewmateCard(int cardID)
    {
        crewmateCards[cardID].Deselect();
        selectedCrewmateCardIDs.Remove(cardID);
    }
    private void DeselectCrewmateCardShare(int cardID)
    {
        DeselectCrewmateCard(cardID);
        cm.DeselectCrewmate(cardID);
    }
    internal void DeselectAllCrewmateCards()
    {
        for (int i = 0; i < selectedCrewmateCardIDs.Count; i++)
        {
            crewmateCards[selectedCrewmateCardIDs[i]].Deselect();
        }
        selectedCrewmateCardIDs.Clear();
    }
    private void DeselectAllCrewmateCardsShare()
    {
        DeselectAllCrewmateCards();
        cm.DeselectAllCrewmates();
    }

    // Handlers - Building Card
    // this should probably be in building manager
    private void BuildingCardAvailabilityHandler()
    {
        for (int i = 0; i < buildingCards.Count; i++)
        {
            if (bm.CanConstructBuilding(i))
            {
                buildingCards[i].GetComponentInChildren<Button>().interactable = true;
                buildingCards[i].GetComponent<DragObj2D>().currentlyDragable = true;
            }
            else
            {
                buildingCards[i].GetComponentInChildren<Button>().interactable = false;
                buildingCards[i].GetComponent<DragObj2D>().currentlyDragable = false;
            }
        }
    }


    // Callbacks - General
    public void SelectTabCallback(int tabIndex)
    {
        SelectTab(tabIndex);

        // Separate menu opening
        if (!isOpen)
        {
            OpenInterface();
        }
    }
    // Callbacks - Crewmate Cards
    private void OnBeginDraggingCrewmateCardCallback(DragObj2D card)
    {
        CrewmateCard crewmateCard = card.GetComponent<CrewmateCard>();
        SelectCrewmateCardShare(crewmateCard.ID);
        CameraManager.SetEdgeScrolling(true);
    }
    private void OnEndDraggingCrewmateCardCallback(DragObj2D card)
    {
        CameraManager.SetEdgeScrolling(false);
        bm.OnCrewmateDropAssign();
        ship.OnCrewmateDropAssign();
        DeselectAllCrewmateCardsShare();
    }
    // Callbacks - Building Cards
    private void OnBeginDraggingBuildingCardCallback(DragObj2D card)
    {
        BuildingCard buildingCard = card.GetComponent<BuildingCard>();
        buildingCard.Select(); // UI
        CameraManager.SetEdgeScrolling(true);
        onBeginDraggingBuildingCard.Invoke(buildingCard.Index);
    }
    private void OnEndDraggingBuildingCardCallback(DragObj2D card)
    {
        CameraManager.SetEdgeScrolling(false);
        BuildingCard buildingCard = card.GetComponent<BuildingCard>();
        DeselectBuildingCard(buildingCard.Index);
        onEndDraggingBuildingCard.Invoke(buildingCard.Index);
    }
    private void OnBeginHoverBuildingCardCallback(int cardIndex)
    {
        BuildingCard card = buildingCards[cardIndex];
        // Move to be above/aligned with the card
        Vector3 aboveCard = popupUI.transform.position;
        aboveCard.x = card.transform.position.x;
        popupUI.transform.position = aboveCard;
        popupUI.GetComponent<CanvasGroup>().DOFade(1, animTimeHoverInterface);

        // Populate UI
        buildingResourceCost.text = card.resourceCost.wood.ToString(); // cost
        buildingAPCost.text = card.resourceCost.ap.ToString(); // AP
        // Production
        if (card.resourceProduction.food > 0)
        {
            buildingProduction.text = "Food";
        }
        else if (card.resourceProduction.wood > 0)
        {
            buildingProduction.text = "Wood";
        }
        else
        {
            buildingProduction.text = "None";
        }
    }
    private void OnEndHoverBuildingCardCallback()
    {
        popupUI.GetComponent<CanvasGroup>().DOFade(0, animTimeHoverInterface);
    }

    // Menu Functions
    private void OpenInterface()
    {
        isOpen = true;
        transform.DOMoveY(transform.position.y + pagesUIParent.GetComponent<RectTransform>().rect.height, animTimeInterface);
        arrow.onClick.RemoveListener(OpenInterface);
        arrow.onClick.AddListener(CloseInterface);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), animTimeArrow);
    }
    private void CloseInterface()
    {
        isOpen = false;
        transform.DOMoveY(transform.position.y - pagesUIParent.GetComponent<RectTransform>().rect.height, animTimeInterface); // cant get height in start
        arrow.onClick.RemoveListener(CloseInterface);
        arrow.onClick.AddListener(OpenInterface);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), animTimeArrow);
    }
}