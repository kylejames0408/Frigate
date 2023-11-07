using DG.Tweening;
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
    private List<BuildingCard> buildingCards;
    private List<CrewmateCard> crewmateCards;
    private List<int> selectedCrewmateCardIndicies;
    private bool isOpen;

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
        crewmateCards = new List<CrewmateCard>();
        selectedCrewmateCardIndicies = new List<int>();
        popupUI.GetComponent<CanvasGroup>().alpha = 0;
    }
    private void Start()
    {
        isOpen = true;
        // Sets tab triggers
        for ( int i = 0; i < tabs.Length; i++ )
        {
            int index = i; // needs to be destroyed after setting listener
            tabs[i].GetComponent<Button>().onClick.AddListener(() => { SelectTab(index); });
        }
        // Init building UI as start tab
        SelectTab(0);
        // Sets arrow initial onclick callback
        arrow.onClick.AddListener(CloseMenu);
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
        card.Init(building.resourceCost, building.resourceProduction);
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

        CrewmateCard card = cardObj.GetComponent<CrewmateCard>();
        card.Init(mate.id);
        crewmateCards.Add(card);

        int index = crewmateCards.IndexOf(card); // needs to be destroyed after setting listener
        // Callbacks
        card.GetComponent<Button>().onClick.AddListener(() => { ClickCrewmateCard(index); }); // drag + drop func
        // Fill UI
        card.GetComponentsInChildren<Image>()[1].sprite = mate.Icon;
        card.GetComponentInChildren<TextMeshProUGUI>().text = mate.Name;
    }
    internal void RemoveCrewmateCard(int cardIndex)
    {
        DeselectCrewmateCard(cardIndex);
        Destroy(crewmateCards[cardIndex].gameObject);
        crewmateCards.RemoveAt(cardIndex);
    }
    // Clicking handler
    private void ClickCrewmateCard(int cardIndex) // share
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectCrewmateCardShare(cardIndex);
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAllCrewmateCardsShare();
            }
            SelectCrewmateCard(cardIndex);
            cm.SelectCrewmate(crewmateCards[cardIndex].crewmateID);
        }
    }
    internal void SelectCrewmateCard(int cardIndex)
    {
        crewmateCards[cardIndex].GetComponent<Outline>().enabled = true;
        selectedCrewmateCardIndicies.Add(cardIndex);
    }
    internal void DeselectCrewmateCard(int cardIndex)
    {
        crewmateCards[cardIndex].GetComponent<Outline>().enabled = false;
        selectedCrewmateCardIndicies.Remove(cardIndex);
    }
    private void DeselectCrewmateCardShare(int cardIndex)
    {
        DeselectCrewmateCard(cardIndex);
        cm.DeselectCrewmate(crewmateCards[cardIndex].crewmateID);
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
}
