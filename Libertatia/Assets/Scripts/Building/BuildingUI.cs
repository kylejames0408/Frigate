using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class BuildingUI : MonoBehaviour
{
    public GameObject buildingCardPrefab;
    public Transform buildingUIParent;
    private List<GameObject> buildingCards;
    public GameObject attackBtn;
    // Ghost Building
    [SerializeField] private GameObject devMenu; // could move this into DevUI class/file
    private Button[] devButtons;

    void Start()
    {
        // Dev tools
        //devButtons = devMenu.GetComponentsInChildren<Button>();
        //devButtons[0].onClick.AddListener(() => BuildingManager.Instance.BuildAll());
        //attackBtn.GetComponent<Button>().onClick.AddListener(() => CeneManager.NextScene());
        attackBtn.SetActive(false);
    }

    private void OpenMenu()
    {
        // raise
        // flip arrow
    }

    private void CloseMenu()
    {
        // lower
        // flip arrow
    }

    public void FillUI(BuildingManager bm, Building[] buildings)
    {
        buildingCards = new List<GameObject>(buildings.Length);
        for (int i = 0; i < buildings.Length; i++)
        {
            GameObject card = Instantiate(buildingCardPrefab, buildingUIParent);
            card.GetComponent<Button>().onClick.AddListener(() => { bm.SelectBuilding(i); });
            card.GetComponentsInChildren<Image>()[1].sprite = buildings[i].Icon;
            card.GetComponentInChildren<TextMeshProUGUI>().text = buildings[i].Name;
            buildingCards.Add(card);
        }
    }
}
