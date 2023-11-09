using DG.Tweening;
using EasyDragAndDrop.Core;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class OutpostManagementUI : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private BuildingManager bm;
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
    private List<BuildingCard> buildingCards; // make into dict
    private Dictionary<int, CrewmateCard> crewmateCards;
    private List<int> selectedCrewmateCardIndicies;
    private bool isOpen;

    //Tutorial stuff
    [SerializeField] private GameObject ClickHereBuildingTab;
    [SerializeField] private GameObject ClickHereBuildingCard;
    [SerializeField] private GameObject ClickHereCrewmateTab;
    [SerializeField] private GameObject DragHereCrewmateCard;

    private void Awake()
    {
        if(cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if(bm == null) { bm = FindObjectOfType<BuildingManager>(); }

        pages = pagesUIParent.GetComponentsInChildren<Transform>();
        if(pages.Length > 1 )
        {
            for( int i = 1; i < pages.Length; i++ )
            {
                pages[i - 1] = pages[i];
            }
        }
        tabs = tabUIParent.GetComponentsInChildren<Tab>();
        crewmateCards = new Dictionary<int, CrewmateCard>();
        selectedCrewmateCardIndicies = new List<int>();
        popupUI.GetComponent<CanvasGroup>().alpha = 0;
    }
    private void Start()
    {
        isOpen = false;
        // Sets tab triggers
        for ( int i = 0; i < tabs.Length; i++ )
        {
            int index = i; // needs to be destroyed after setting listener
            tabs[i].GetComponent<Button>().onClick.AddListener(() => { SelectTab(index); });
        }
        // Init building UI as start tab
        //SelectTab(0);
        // Sets arrow initial onclick callback
        if(isOpen)
            arrow.onClick.AddListener(CloseMenu);
        else
            arrow.onClick.AddListener(OpenMenu);
    }
    private void Update()
    {
        UpdateBuildingCardAvailability();
    }

    // this should probably be in building manager
    private void UpdateBuildingCardAvailability()
    {
        for (int i = 0; i < buildingCards.Count; i++)
        {
            if(bm.CanConstructBuilding(i))
            {
                buildingCards[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                buildingCards[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    // Select tab callback - changes tab interface and adds interface content
    public void SelectTab(int index)
    {
        if(!isOpen)
        {
            OpenMenu();
        }

        for (int i = 0; i < tabs.Length; i++) // assumes tabs and pages length are equal
        {
            if (i == index)
            {
                tabs[i].Select();
                pages[i].gameObject.SetActive(true);
                if (i == 0)
                {
                    if (GameManager.outpostVisitNumber == 1)
                    {
                        HideBuildingTabArrow();
                        ShowBuildingCardArrow();
                    }                    
                }
                    
                if (i == 1)
                {
                    if(GameManager.outpostVisitNumber == 1)
                    {
                        HideCrewmateTabArrow();
                        ShowCrewmateCardArrow();
                    }                    
                }
            }
            else
            {
                tabs[i].Deselect();
                pages[i].gameObject.SetActive(false);
            }
        }
    }

    // Fills building construction UI page
    public void FillConstructionUI(BuildingManager bm, Building[] buildings)
    {
        buildingCards = new List<BuildingCard>(buildings.Length);
        for (int i = 0; i < buildings.Length; i++)
        {
            AddBuildingCard(bm, i, buildings[i]);
        }
    }

    // Building (construction) cards
    public void AddBuildingCard(BuildingManager bm, int index, Building building)
    {
        GameObject cardObj = Instantiate(buildingCardPrefab, pages[0]);

        BuildingCard card = cardObj.GetComponent<BuildingCard>();
        card.Init(building.Cost, building.Production);
        card.onHover.AddListener(()=> { BuildingCardHoveredCallback(index); });
        card.onHoverExit.AddListener(BuildingCardHoveredExitCallback);
        buildingCards.Add(card);

        // Callbacks
        cardObj.GetComponent<Button>().onClick.AddListener(() => { ClickBuildingCard(bm, index); });
        // Fill UI
        cardObj.GetComponentsInChildren<Image>()[1].sprite = building.Icon;
        cardObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Name;
    }
    private void ClickBuildingCard(BuildingManager bm, int cardIndex)
    {
        foreach (BuildingCard card in buildingCards)
        {
            card.GetComponent<Outline>().enabled = false;
        }
        buildingCards[cardIndex].GetComponent<Outline>().enabled = true;
        bm.SelectBuilding(cardIndex);
        HideBuildingCardArrow();
    }
    public void DeselectBuildingCard(int cardIndex)
    {
        buildingCards[cardIndex].GetComponent<Outline>().enabled = false;
    }
    private void BuildingCardHoveredCallback(int cardIndex)
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
        else
        {
            buildingProduction.text = "None";
        }
    }
    private void BuildingCardHoveredExitCallback()
    {
        popupUI.GetComponent<CanvasGroup>().DOFade(0, animTimeHoverInterface);
    }

    // Crewmate card interactions
    internal void AddCrewmateCard(Crewmate mate)
    {
        GameObject cardObj = Instantiate(crewmateCardPrefab, pages[1]);

        CrewmateCard card = cardObj.GetComponentInChildren<CrewmateCard>();
        card.Init(mate.ID);
        crewmateCards.Add(card.ID, card);

        // Callbacks
        card.GetComponent<Button>().onClick.AddListener(() => { ClickCrewmateCard(card.ID); }); // drag + drop func
        // Fill UI
        card.GetComponentsInChildren<Image>()[1].sprite = mate.Icon;
        card.GetComponentInChildren<TextMeshProUGUI>().text = mate.Name;

        card.GetComponent<DragObj2D>().onBeginDrag.AddListener(delegate { ClickCrewmateCard(card.ID); });
        card.GetComponent<DragObj2D>().onEndDrag.AddListener(delegate { bm.OnCrewmateDropAssign(); });
        card.GetComponent<DragObj2D>().onEndDrag.AddListener(delegate { DeselectCrewmateCard(card.ID); });
    }
    internal void RemoveCrewmateCard(int cardID)
    {
        DeselectCrewmateCard(cardID);
        Destroy(crewmateCards[cardID].gameObject);
        crewmateCards.Remove(cardID);
    }
    // Clicking handler
    private void ClickCrewmateCard(int cardID) // share
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectCrewmateCardShare(cardID);
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAllCrewmateCardsShare();
            }
            SelectCrewmateCard(cardID);
            cm.SelectCrewmate(cardID);
        }
    }
    internal void SelectCrewmateCard(int cardID)
    {
        crewmateCards[cardID].GetComponent<Outline>().enabled = true;
        selectedCrewmateCardIndicies.Add(cardID);
    }
    internal void DeselectCrewmateCard(int cardID)
    {
        crewmateCards[cardID].GetComponent<Outline>().enabled = false;
        selectedCrewmateCardIndicies.Remove(cardID);
    }
    private void DeselectCrewmateCardShare(int cardID)
    {
        DeselectCrewmateCard(cardID);
        cm.DeselectCrewmate(cardID);
    }
    internal void DeselectAllCrewmateCards()
    {
        for (int i = 0; i < selectedCrewmateCardIndicies.Count; i++)
        {
            crewmateCards[selectedCrewmateCardIndicies[i]].GetComponent<Outline>().enabled = false;
        }
        selectedCrewmateCardIndicies.Clear();
    }
    private void DeselectAllCrewmateCardsShare()
    {
        DeselectAllCrewmateCards();
        cm.DeselectAllCrewmates();
    }


    // Menu Functions
    private void OpenMenu()
    {
        isOpen = true;
        transform.DOMoveY(transform.position.y + pagesUIParent.GetComponent<RectTransform>().rect.height, animTimeInterface);
        arrow.onClick.RemoveListener(OpenMenu);
        arrow.onClick.AddListener(CloseMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), animTimeArrow);
    }
    private void CloseMenu()
    {
        isOpen = false;
        transform.DOMoveY(transform.position.y-pagesUIParent.GetComponent<RectTransform>().rect.height, animTimeInterface); // cant get height in start
        arrow.onClick.RemoveListener(CloseMenu);
        arrow.onClick.AddListener(OpenMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), animTimeArrow);
    }

    public void ShowBuildingTabArrow()
    {
        if (!ClickHereBuildingTab.activeSelf)
            ClickHereBuildingTab.SetActive(true);
    }

    public void HideBuildingTabArrow()
    {
        if (ClickHereBuildingTab.activeSelf)
            ClickHereBuildingTab.SetActive(false);
    }

    public void ShowBuildingCardArrow()
    {
        if (!ClickHereBuildingCard.activeSelf)
            ClickHereBuildingCard.SetActive(true);
    }

    public void HideBuildingCardArrow()
    {
        if (ClickHereBuildingCard.activeSelf)
            ClickHereBuildingCard.SetActive(false);
    }

    public void ShowCrewmateTabArrow()
    {
        if (!ClickHereCrewmateTab.activeSelf)
            ClickHereCrewmateTab.SetActive(true);
    }

    public void HideCrewmateTabArrow()
    {
        if (ClickHereCrewmateTab.activeSelf)
            ClickHereCrewmateTab.SetActive(false);
    }

    public void ShowCrewmateCardArrow()
    {
        if (!DragHereCrewmateCard.activeSelf)
            DragHereCrewmateCard.SetActive(true);
    }

    public void HideCrewmateCardArrow()
    {
        if (DragHereCrewmateCard.activeSelf)
            DragHereCrewmateCard.SetActive(false);
    }
}
