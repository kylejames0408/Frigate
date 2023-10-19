using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutpostManagementUI : MonoBehaviour
{
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
    private List<GameObject> buildingCards;
    private List<GameObject> crewmateCards;
    private bool isOpen;

    private void Awake()
    {
        pages = pagesUIParent.GetComponentsInChildren<Transform>();
        if(pages.Length > 1 )
        {
            for( int i = 1; i < pages.Length; i++ )
            {
                pages[i - 1] = pages[i];
            }
        }
        tabs = tabUIParent.GetComponentsInChildren<Tab>();
    }

    void Start()
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

    // Minimizes menu
    private void OpenMenu()
    {
        isOpen = true;
        transform.DOMoveY(0, 0.6f);
        arrow.onClick.RemoveListener(OpenMenu);
        arrow.onClick.AddListener(CloseMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), 0.5f);
    }
    // Minimizes menu
    private void CloseMenu()
    {
        isOpen = false;
        transform.DOMoveY(-pagesUIParent.GetComponent<RectTransform>().rect.height, 0.6f); // cant get height in start
        arrow.onClick.RemoveListener(CloseMenu);
        arrow.onClick.AddListener(OpenMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), 0.5f);
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
        buildingCards = new List<GameObject>(buildings.Length);
        for (int i = 0; i < buildings.Length; i++)
        {
            GameObject card = Instantiate(buildingCardPrefab, pages[0]);
            int index = i; // needs to be destroyed after setting listener
            card.GetComponent<Button>().onClick.AddListener(() => { SelectBuildingCard(bm, index); });
            card.GetComponentsInChildren<Image>()[1].sprite = buildings[i].Icon;
            card.GetComponentInChildren<TextMeshProUGUI>().text = buildings[i].Name;
            buildingCards.Add(card);
        }
    }
    // Fills crewmate construction UI page
    public void FillCrewmateUI(BuildingManager bm, CrewmateData[] crewmates) // will take in a crewmanager script
    {
        crewmateCards = new List<GameObject>(crewmates.Length);
        for (int i = 0; i < crewmates.Length; i++)
        {
            GameObject card = Instantiate(crewmateCardPrefab, pages[1]);
            int index = i; // needs to be destroyed after setting listener
            card.GetComponent<Button>().onClick.AddListener(() => { SelectCrewmateCard(bm, index); }); // drag + drop func
            card.GetComponentsInChildren<Image>()[1].sprite = crewmates[i].icon;
            card.GetComponentInChildren<TextMeshProUGUI>().text = crewmates[i].name;
            crewmateCards.Add(card);
        }
    }

    private void SelectBuildingCard(BuildingManager bm, int index)
    {
        foreach (GameObject card in buildingCards)
        {
            card.GetComponent<Outline>().enabled = false;
        }
        buildingCards[index].GetComponent<Outline>().enabled = true;
        bm.SelectBuilding(index);
        bm.placedBuilding.AddListener(() => { DeselectBuildingCard(index); });
    }
    public void DeselectBuildingCard(int index)
    {
        buildingCards[index].GetComponent<Outline>().enabled = false;
    }
    private void SelectCrewmateCard(BuildingManager bm, int index)
    {
        foreach (GameObject card in crewmateCards)
        {
            card.GetComponent<Outline>().enabled = false;
        }
        crewmateCards[index].GetComponent<Outline>().enabled = true;
        Crewmate crewmate = bm.SelectCrewmate(index);
        crewmate.onAssign.AddListener(() => { DeselectCrewmateCard(index); });
    }
    public void DeselectCrewmateCard(int index)
    {
        crewmateCards[index].GetComponent<Outline>().enabled = false;
    }
}
