using DG.Tweening;
using EasyDragAndDrop.Core;
using System.Collections.Generic;
using TMPro;
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
    private List<GameObject> crewmateCards;
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
        crewmateCards = new List<GameObject>();
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

    // Building cards
    private void AddBuildingCard(BuildingManager bm, int index, Building building)
    {
        GameObject cardObj = Instantiate(buildingCardPrefab, pages[0]);

        BuildingCard card = cardObj.GetComponent<BuildingCard>();
        card.Init(building.resourceCost, building.resourceProduction);
        card.onHover.AddListener(()=> { BuildingCardHoveredCallback(index); });
        card.onHoverExit.AddListener(BuildingCardHoveredExitCallback);
        buildingCards.Add(card);

        cardObj.GetComponent<Button>().onClick.AddListener(() => { SelectBuildingCard(bm, index); });
        cardObj.GetComponentsInChildren<Image>()[1].sprite = building.Icon;
        cardObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Name;
    }
    private void SelectBuildingCard(BuildingManager bm, int cardIndex)
    {
        foreach (BuildingCard card in buildingCards)
        {
            card.GetComponent<Outline>().enabled = false;
        }
        buildingCards[cardIndex].GetComponent<Outline>().enabled = true;
        bm.SelectBuilding(cardIndex);
        bm.placedBuilding.AddListener(() => { DeselectBuildingCard(cardIndex); }); // might be able to merge these 2
        bm.cancelBuilding.AddListener(() => { DeselectBuildingCard(cardIndex); });
    }
    public void DeselectBuildingCard(int cardIndex)
    {
        buildingCards[cardIndex].GetComponent<Outline>().enabled = false;
    }
    // Crew cards
    public void AddCrewmateCard(Crewmate mate)
    {
        GameObject card = Instantiate(crewmateCardPrefab, pages[1]);
        crewmateCards.Add(card);
        int index = crewmateCards.IndexOf(card); // needs to be destroyed after setting listener
        card.GetComponent<Button>().onClick.AddListener(() => { SelectCrewmateCard(index); }); // drag + drop func
        card.GetComponentsInChildren<Image>()[1].sprite = mate.Icon;
        card.GetComponentInChildren<TextMeshProUGUI>().text = mate.Name;

        card.GetComponent<DragObj2D>().onBeginDrag.AddListener(delegate{ SelectCrewmateCard(index); });
        card.GetComponent<DragObj2D>().onEndDrag.AddListener(delegate { bm.OnCrewmateDropAssign(); });
        card.GetComponent<DragObj2D>().onEndDrag.AddListener(delegate{ DeselectCrewmateCard(index); });
    }
    public void SelectCrewmateCard(int cardIndex)
    {
        foreach (GameObject card in crewmateCards)
        {
            card.GetComponent<Outline>().enabled = false;
        }
        crewmateCards[cardIndex].GetComponent<Outline>().enabled = true;
        Crewmate crewmate = cm.SelectCrewmate(cardIndex);
        crewmate.onAssign.AddListener(() => { DeselectCrewmateCard(cardIndex); });
    }
    public void DeselectCrewmateCard(int cardIndex)
    {
        crewmateCards[cardIndex].GetComponent<Outline>().enabled = false;
    }
    // Callbacks
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
        if (card.resourceProduction.food>0)
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


    // Minimizes menu
    private void OpenMenu()
    {
        isOpen = true;
        transform.DOMoveY(0, animTimeInterface);
        arrow.onClick.RemoveListener(OpenMenu);
        arrow.onClick.AddListener(CloseMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), animTimeArrow);
    }
    // Minimizes menu
    private void CloseMenu()
    {
        isOpen = false;
        transform.DOMoveY(-pagesUIParent.GetComponent<RectTransform>().rect.height, animTimeInterface); // cant get height in start
        arrow.onClick.RemoveListener(CloseMenu);
        arrow.onClick.AddListener(OpenMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), animTimeArrow);
    }
}
