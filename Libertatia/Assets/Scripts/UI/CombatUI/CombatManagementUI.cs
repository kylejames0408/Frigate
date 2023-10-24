using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManagementUI : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;
    // Animation related
    [SerializeField] private float arrowAnimTime = 0.5f;
    [SerializeField] private float interfaceAnimTime = 0.6f;
    // Prefabs
    [SerializeField] private GameObject crewmateCardPrefab;
    // Interface object references
    [SerializeField] private Button arrow;
    // Parent objects
    [SerializeField] private Transform tabUIParent;
    [SerializeField] private Transform pagesUIParent;
    // Data tracking
    private Tab[] tabs;
    private Transform[] pages;

    private List<GameObject> crewmateCards;
    private bool isOpen;

    private void Awake()
    {
        if(cm == null) { cm = FindObjectOfType<CrewmateManager>(); }

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


    // Fills crewmate construction UI page
    public void FillCrewmateUI(Crewmate[] crewmates)
    {
        crewmateCards = new List<GameObject>(crewmates.Length);
        for (int i = 0; i < crewmates.Length; i++)
        {
            AddCrewmateCard(crewmates[i]);
        }
    }
    public void AddCrewmateCard(Crewmate mate)
    {
        GameObject card = Instantiate(crewmateCardPrefab, pages[1]);
        crewmateCards.Add(card);
        int index = crewmateCards.IndexOf(card); // needs to be destroyed after setting listener
        card.GetComponent<Button>().onClick.AddListener(() => { SelectCrewmateCard(index); }); // drag + drop func
        card.GetComponentsInChildren<Image>()[1].sprite = mate.Icon;
        card.GetComponentInChildren<TextMeshProUGUI>().text = mate.Name;
    }


    public void SelectCrewmateCard(int index)
    {
        foreach (GameObject card in crewmateCards)
        {
            card.GetComponent<Outline>().enabled = false;
        }
        crewmateCards[index].GetComponent<Outline>().enabled = true;
        Crewmate crewmate = cm.SelectCrewmate(index);
        crewmate.onAssign.AddListener(() => { DeselectCrewmateCard(index); });
    }
    public void DeselectCrewmateCard(int index)
    {
        crewmateCards[index].GetComponent<Outline>().enabled = false;
    }

    // Minimizes menu
    private void OpenMenu()
    {
        isOpen = true;
        transform.DOMoveY(0, interfaceAnimTime);
        arrow.onClick.RemoveListener(OpenMenu);
        arrow.onClick.AddListener(CloseMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), arrowAnimTime);
    }
    // Minimizes menu
    private void CloseMenu()
    {
        isOpen = false;
        transform.DOMoveY(-pagesUIParent.GetComponent<RectTransform>().rect.height, interfaceAnimTime); // cant get height in start
        arrow.onClick.RemoveListener(CloseMenu);
        arrow.onClick.AddListener(OpenMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), arrowAnimTime);
    }
}
