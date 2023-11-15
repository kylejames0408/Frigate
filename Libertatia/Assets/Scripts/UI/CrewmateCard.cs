using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrewmateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int crewmateID = -1;
    private Outline outline;
    public UnityEvent onHover;
    public UnityEvent onHoverExit;

    public int ID
    {
        get { return crewmateID; }
    }

    public void Init(int crewmateID)
    {
        this.crewmateID = crewmateID;
        outline = GetComponent<Outline>();
        Deselect();
    }

    public void Select()
    {
        outline.enabled = true;
    }
    public void Deselect()
    {
        outline.enabled = false;
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