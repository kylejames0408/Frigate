using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IslandUI : MonoBehaviour
{
    [SerializeField] private int islandID = -1;
    [SerializeField] private float animSpeedInterface = 0.6f;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private RectTransform bounds;
    // References
    [SerializeField] private ShipUI shipUI;
    // Interface
    [SerializeField] private Image uiIslandIcon;
    [SerializeField] private TextMeshProUGUI uiName;
    [SerializeField] private TextMeshProUGUI uiResources;
    [SerializeField] private Image[] dotsDifficulty;
    [SerializeField] private Image[] dotsTravelAP;
    [SerializeField] private Image[] dotsCombatAP;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnDepart;

    public UnityEvent onDepart;

    private void Awake()
    {
        if (shipUI == null) { shipUI = FindObjectOfType<ShipUI>(); }
        if (bounds == null) { bounds = GetComponent<RectTransform>(); }
        isOpen = false;

        foreach (Image dot in dotsDifficulty)
        {
            dot.gameObject.SetActive(false);
        }
        foreach (Image dot in dotsTravelAP)
        {
            dot.gameObject.SetActive(false);
        }
        foreach (Image dot in dotsCombatAP)
        {
            dot.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        btnClose.onClick.AddListener(CloseInterface);
        btnDepart.onClick.AddListener(DepartCallback);
    }

    internal void Fill(Island island, int ap)
    {
        islandID = island.ID;
        uiIslandIcon.sprite = island.Icon;
        uiName.text = island.Name;
        uiResources.text = island.Resources;

        for (int i = 0; i < 5; i++)
        {
            if (i < island.Difficulty)
            {
                dotsDifficulty[i].gameObject.SetActive(true);
            }
            else
            {
                dotsDifficulty[i].gameObject.SetActive(false);
            }

            if (i < ap)
            {
                dotsTravelAP[i].gameObject.SetActive(true);
            }
            else
            {
                dotsTravelAP[i].gameObject.SetActive(false);
            }

            if (i < ap)  // will be something else
            {
                dotsCombatAP[i].gameObject.SetActive(true);
            }
            else
            {
                dotsCombatAP[i].gameObject.SetActive(false);
            }
        }
    }
    private void DepartCallback()
    {
        onDepart.Invoke();
    }
    internal void OpenInterface()
    {
        transform.DOMoveX(690, animSpeedInterface);
        shipUI.CloseMenu();
        isOpen = true;
    }
    internal void CloseInterface()
    {
        transform.DOMoveX(-10, animSpeedInterface);
        isOpen = false;
    }
}