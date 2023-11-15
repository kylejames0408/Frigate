using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CrewmateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int crewmateID = -1;
    public UnityEvent onHover;
    public UnityEvent onHoverExit;

    public int ID
    {
        get { return crewmateID; }
    }

    public void Init(int crewmateID)
    {
        this.crewmateID = crewmateID;
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