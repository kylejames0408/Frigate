using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class BuildingUI : MonoBehaviour
{
    public GameObject buildingCardPrefab;
    private List<GameObject> buildingCards;
    public GameObject attackBtn;
    [SerializeField] private Button arrow;
    // Ghost Building
    [SerializeField] private GameObject devMenu; // could move this into DevUI class/file
    private Button[] devButtons;
    public Transform tabUIParent;
    private Tab[] tabs;
    public Transform pagesUIParent;
    private Transform[] pages;

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
        for ( int i = 0; i < tabs.Length; i++ )
        {
            int index = i; // needs to be destroyed after setting listener
            tabs[i].GetComponent<Button>().onClick.AddListener(() => { SelectTab(index); });
        }
        SelectTab(0);

        // Dev tools
        //devButtons = devMenu.GetComponentsInChildren<Button>();
        //devButtons[0].onClick.AddListener(() => BuildingManager.Instance.BuildAll());
        //attackBtn.GetComponent<Button>().onClick.AddListener(() => CeneManager.NextScene());
        attackBtn.SetActive(false);
        arrow.onClick.AddListener(() => OpenMenu());
    }

    private void OpenMenu()
    {
        transform.DOMoveY(91.515f, 0.6f);
        arrow.onClick.RemoveAllListeners();
        arrow.onClick.AddListener(() => CloseMenu());
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), 0.5f);
    }

    private void CloseMenu()
    {
        transform.DOMoveY(-31, 0.6f);
        arrow.onClick.RemoveAllListeners();
        arrow.onClick.AddListener(() => OpenMenu());
        arrow.transform.GetChild(0).DORotate(new Vector3(0, 0, 0), 0.5f);
    }

    public void FillUI(BuildingManager bm, Building[] buildings)
    {
        buildingCards = new List<GameObject>(buildings.Length);
        for (int i = 0; i < buildings.Length; i++)
        {
            GameObject card = Instantiate(buildingCardPrefab, pages[0]);
            int index = i; // needs to be destroyed after setting listener
            card.GetComponent<Button>().onClick.AddListener(() => { bm.SelectBuilding(index); });
            card.GetComponentsInChildren<Image>()[1].sprite = buildings[i].Icon;
            card.GetComponentInChildren<TextMeshProUGUI>().text = buildings[i].Name;
            buildingCards.Add(card);
        }
    }

    public void SelectTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++) // assumes tabs and pages length are equal
        {
            if(i == index)
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
}
