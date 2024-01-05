using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// Note
// I had outline logic (now in set) in awake, but awake does not run when GO's are instantiated to an inactive parent

public class CrewmateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int crewmateID = -1;
    private Outline outline;
    // Crewmate UI Components
    [SerializeField] private TextMeshProUGUI firstName;
    [SerializeField] private Image icon;
    [SerializeField] private Image iconsStatus;
    [SerializeField] private Slider sliderHealth;
    // Events
    public UnityEvent onHover;
    public UnityEvent onHoverExit;

    internal int ID
    {
        get { return crewmateID; }
    }
    public ObjectData CrewmateData
    {
        get
        {
            return new ObjectData(crewmateID, icon.sprite);
        }
    }

    internal void Set(Crewmate mate)
    {
        outline = GetComponent<Outline>();
        Deselect();

        crewmateID = mate.ID;
        icon.sprite = mate.Icon;
        firstName.text = mate.FirstName;
        iconsStatus.sprite = mate.StateIcon;
        sliderHealth.value = mate.Health;
    }

    internal void SetStatus(Sprite statusIcon)
    {
        iconsStatus.sprite = statusIcon;
    }
    internal void SetFirstName(string firstName)
    {
        this.firstName.text = firstName;
    }

    public void Select()
    {
        outline.effectColor = new Color(255, 187, 00);
        outline.enabled = true;
    }
    public void Deselect()
    {
        outline.enabled = false;
    }

    public void Highlight()
    {
        outline.effectColor = new Color(0, 217, 255);
        outline.enabled = true;
    }

    // Stays if we are adding hovering for crewmates
    public void OnPointerEnter(PointerEventData eventData)
    {
        //onHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //onHoverExit.Invoke();
    }
}