using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrewmateCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int crewmateID = -1;
    private Outline outline;
    // Crewmate UI Components
    [SerializeField] private Image iconCrewmate;
    [SerializeField] private string firstName;
    [SerializeField] private Image iconsStatus;
    [SerializeField] private Slider sliderHealth;
    // Events
    public UnityEvent onHover;
    public UnityEvent onHoverExit;

    public int ID
    {
        get { return crewmateID; }
    }

    public void Init(Crewmate mate)
    {
        outline = GetComponent<Outline>();
        Set(mate);
        Deselect();
    }
    internal void Set(Crewmate mate)
    {
        crewmateID = mate.ID;
        iconCrewmate.sprite = mate.Icon;
        firstName = mate.FirstName;
        iconsStatus.sprite = mate.StatusIcon;
        sliderHealth.value = mate.Health;

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