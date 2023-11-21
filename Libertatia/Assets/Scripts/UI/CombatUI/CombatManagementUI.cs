using DG.Tweening;
using EasyDragAndDrop.Core;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManagementUI : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ZoneManager zm;
    // Animation related
    [SerializeField] private float animTimeArrow = 0.5f;
    [SerializeField] private float animTimeInterface = 0.6f;
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

    private Dictionary<int, CrewmateCard> crewmateCards;
    private List<int> selectedCrewmateCardIDs;
    private bool isOpen;

    private void Awake()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }

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
        if (!isOpen)
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

    // Crewmate card interactions
    internal void AddCrewmateCard(Crewmate mate)
    {
        GameObject cardObj = Instantiate(crewmateCardPrefab, pages[1]);

        CrewmateCard card = cardObj.GetComponentInChildren<CrewmateCard>();
        card.Set(mate);
        crewmateCards.Add(card.ID, card);

        // Callbacks
        card.GetComponent<Button>().onClick.AddListener(() => { ClickCrewmateCard(card.ID); }); // drag + drop func
        // Fill UI
        card.GetComponentsInChildren<Image>()[1].sprite = mate.Icon;
        card.GetComponentInChildren<TextMeshProUGUI>().text = mate.FirstName;

        card.GetComponent<DragObj2D>().onBeginDrag.AddListener(delegate { ClickCrewmateCard(card.ID); });
        card.GetComponent<DragObj2D>().onEndDrag.AddListener(delegate { zm.OnCrewmateDropAssign(); });
        card.GetComponent<DragObj2D>().onEndDrag.AddListener(delegate { DeselectCrewmateCard(card.ID); });
    }
    internal void RemoveCrewmateCard(int cardID)
    {
        DeselectCrewmateCard(cardID);
        Destroy(crewmateCards[cardID].gameObject);
        crewmateCards.Remove(cardID);
    }
    internal void UpdateCard(int cardID, Sprite stateIcon)
    {
        CrewmateCard card = crewmateCards[cardID];
        card.SetStatus(stateIcon);
    }

    // Clicking handler
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
        SelectCrewmateCard(cardID);
        cm.SelectCrewmate(cardID);
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
        transform.DOMoveY(transform.position.y - pagesUIParent.GetComponent<RectTransform>().rect.height, animTimeInterface); // cant get height in start
        arrow.onClick.RemoveListener(CloseMenu);
        arrow.onClick.AddListener(OpenMenu);
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), animTimeArrow);
    }
}
