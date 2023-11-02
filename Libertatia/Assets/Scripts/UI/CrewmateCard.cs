using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CrewmateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int crewmateID = -1;
    public UnityEvent onHover;
    public UnityEvent onHoverExit;

    public void Init(int crewmateID)
    {
        this.crewmateID = crewmateID;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //onHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //onHoverExit.Invoke();
    }
}